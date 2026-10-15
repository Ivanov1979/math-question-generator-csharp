
using System;
using System.Collections.Concurrent;
using System.Globalization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;

public static class FractionWebServer
{
    public static void Run(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var app = builder.Build();

        // Keep correct answers on the server.
        var generatedAnswers =
            new ConcurrentDictionary<Guid, StoredAnswer>();

        app.UseDefaultFiles();
        app.UseStaticFiles();

        // =====================================================
        // 1. LOAD CURRICULUM FROM MYSQL
        // =====================================================

        app.MapGet("/api/fractions/curriculum", () =>
        {
            try
            {
                var rules =
                    CurriculumRepository.GetFractionGenerationRules();

                return Results.Ok(new
                {
                    total = rules.Count,
                    curriculum = rules
                });
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(
                    $"Curriculum database error: {ex}");

                return Results.Problem(
                    title: "Unable to load fraction curriculum",
                    detail: "Check MySQL connection and server logs.",
                    statusCode: 500);
            }
        });

        // =====================================================
        // 2. ORIGINAL GENERATOR — GRADE 3 OA11
        // =====================================================

        app.MapPost("/api/fractions/generate",
            (GenerationRequest request) =>
            {
                if (!string.Equals(
                    request.Content,
                    "Fractions",
                    StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest(new
                    {
                        message = "Only fraction activities are supported."
                    });
                }

                Activity activity =
                    new QuestionGenerator().GenerateQuestion(request);

                Guid activityId = Guid.NewGuid();

                string answer = activity.GetAnswer();

                generatedAnswers[activityId] = new StoredAnswer(
                    answer,
                    InferResponseType(answer));

                return Results.Ok(new
                {
                    activityId,
                    course = activity.GetCourse(),
                    oa = activity.GetOA(),
                    indicator = activity.GetIndicator(),
                    competency = activity.GetCompetency(),
                    difficulty = activity.GetDifficulty(),
                    question = activity.GetQuestion()
                });
            });

        // =====================================================
        // 3. GENERATE USING MYSQL LEARNING DETAIL
        // =====================================================

        app.MapPost("/api/fractions/generate-by-detail",
            (CurriculumActivityRequest request) =>
            {
                if (request.LearningDetailId <= 0)
                {
                    return Results.BadRequest(new
                    {
                        message = "Please select a valid learning detail."
                    });
                }

                string difficulty =
                    request.Difficulty?.Trim() ?? "";

                string mathematicalSkill =
                    request.MathematicalSkill?.Trim() ?? "";

                if (difficulty != "Easy" &&
                    difficulty != "Medium" &&
                    difficulty != "Hard")
                {
                    return Results.BadRequest(new
                    {
                        message =
                            "Difficulty must be Easy, Medium, or Hard."
                    });
                }

                FractionGenerationRule? rule;

                try
                {
                    rule = CurriculumRepository
                        .GetFractionGenerationRuleByDetailId(
                            request.LearningDetailId);
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(
                        $"Learning detail lookup error: {ex}");

                    return Results.Problem(
                        title: "Unable to retrieve learning detail",
                        detail: "Check MySQL connection and server logs.",
                        statusCode: 500);
                }

                if (rule == null)
                {
                    return Results.NotFound(new
                    {
                        message = "Learning detail not found."
                    });
                }

                // =================================================
                // VALIDATE MATHEMATICAL SKILL FOR ALL GRADES
                // =================================================

                if (string.IsNullOrWhiteSpace(mathematicalSkill))
                {
                    return Results.BadRequest(new
                    {
                        message = "Please select a mathematical skill."
                    });
                }

                string[] allowedSkills =
                {
                    "Representar",
                    "Modelar",
                    "Resolver problemas",
                    "Argumentar y comunicar"
                };

                if (!Array.Exists(
                    allowedSkills,
                    skill => string.Equals(
                        skill,
                        mathematicalSkill,
                        StringComparison.OrdinalIgnoreCase)))
                {
                    return Results.BadRequest(new
                    {
                        message = "Invalid mathematical skill."
                    });
                }

                // =================================================
                // GENERATE ACTIVITY WITH SELECTED SKILL
                // =================================================

                Activity activity;

                try
                {
                    activity = new FractionQuestionGenerator()
                        .GenerateFromRule(
                            rule,
                            difficulty,
                            mathematicalSkill);
                }
                catch (NotSupportedException ex)
                {
                    return Results.Json(new
                    {
                        message = ex.Message,
                        learningDetailId = rule.LearningDetailId,
                        activityType = rule.ActivityType
                    }, statusCode: 422);
                }
                catch (ArgumentException ex)
                {
                    return Results.BadRequest(new
                    {
                        message = ex.Message
                    });
                }

                Guid activityId = Guid.NewGuid();

                generatedAnswers[activityId] = new StoredAnswer(
                    activity.GetAnswer(),
                    rule.ResponseType);

                return Results.Ok(new
                {
                    activityId,
                    learningDetailId = rule.LearningDetailId,
                    grade = rule.Grade,
                    oa = rule.OACode,
                    oaDescription = rule.OADescription,
                    learningDetail = rule.LearningDetail,
                    mathematicalField = rule.MathematicalField,
                    content = rule.Content,
                    activityType = rule.ActivityType,
                    mathematicalSkill,
                    responseType = rule.ResponseType,
                    allowedDenominators = rule.AllowedDenominators,
                    difficulty,
                    question = activity.GetQuestion()
                });
            });

        // =====================================================
        // 4. CHECK GENERATED STUDENT ANSWERS
        // =====================================================

        app.MapPost("/api/fractions/check-generated",
            (GeneratedFractionAnswerRequest request) =>
            {
                if (!generatedAnswers.TryGetValue(
                    request.ActivityId,
                    out StoredAnswer? stored))
                {
                    return Results.NotFound(new
                    {
                        message =
                            "Activity not found. Generate a new activity."
                    });
                }

                string studentAnswer =
                    (request.StudentAnswer ?? "").Trim();

                if (studentAnswer.Length == 0)
                {
                    return Results.BadRequest(new
                    {
                        message = "Please enter your answer."
                    });
                }

                if (stored.ResponseType == "Text" ||
                    stored.ResponseType == "VisualModel")
                {
                    return Results.Json(new
                    {
                        message =
                            "This response requires a dedicated " +
                            "assessment method."
                    }, statusCode: 422);
                }

                bool correct = AnswersMatch(
                    studentAnswer,
                    stored.Answer,
                    stored.ResponseType);

                return Results.Ok(new
                {
                    correct,
                    message = correct
                        ? "Correct! Well done."
                        : "Try again. Review your calculation."
                });
            });

        // =====================================================
        // 5. ORIGINAL GRADE 6 FRACTION CHECKER
        // =====================================================

        app.MapPost("/api/fractions/check",
            (FractionAnswerRequest request) =>
            {
                if (request.Denominator <= 0 ||
                    request.Numerator < 0 ||
                    request.Numerator > request.Denominator)
                {
                    return Results.BadRequest(new
                    {
                        message = "Invalid fraction."
                    });
                }

                bool correct =
                    request.SelectedNumerator == request.Numerator;

                return Results.Ok(new
                {
                    correct,
                    message = correct
                        ? "Correct! Well done."
                        : "Try again. Count the shaded parts."
                });
            });

        app.Run();
    }

    // =====================================================
    // HELPERS
    // =====================================================

    // Retained for compatibility with existing grade formats.
    private static bool IsFourthGrade(object? grade)
    {
        string value = Convert.ToString(
            grade,
            CultureInfo.InvariantCulture)?.Trim() ?? "";

        return value == "4" ||
               value == "4°" ||
               value.Equals(
                   "4th Grade",
                   StringComparison.OrdinalIgnoreCase) ||
               value.Equals(
                   "4th",
                   StringComparison.OrdinalIgnoreCase) ||
               value.Equals(
                   "Cuarto Básico",
                   StringComparison.OrdinalIgnoreCase);
    }

    // Store the answer and expected response format.
    private record StoredAnswer(
        string Answer,
        string ResponseType);

    // Determine response type for legacy activities.
    private static string InferResponseType(string answer)
    {
        string trimmed = answer.Trim();

        if (trimmed == "<" ||
            trimmed == ">" ||
            trimmed == "=")
        {
            return "Comparison";
        }

        if (trimmed.Contains('/'))
        {
            return trimmed.Contains(' ')
                ? "MixedNumber"
                : "Fraction";
        }

        return "Text";
    }

    // Compare student answers with the server-side answer.
    private static bool AnswersMatch(
        string student,
        string expected,
        string responseType)
    {
        switch (responseType)
        {
            case "Comparison":
                return student == expected;

            case "Fraction":
            case "FractionBarSelection":
            case "MixedNumber":
            case "Decimal":
                return TryParseNumber(student, out decimal s) &&
                       TryParseNumber(expected, out decimal e) &&
                       s == e;

            default:
                return string.Equals(
                    student.Trim(),
                    expected.Trim(),
                    StringComparison.OrdinalIgnoreCase);
        }
    }

    // Convert fractions, mixed numbers, and decimals
    // into numeric values for answer comparison.
    private static bool TryParseNumber(
        string input,
        out decimal value)
    {
        value = 0;
        string text = input.Trim();

        if (text.Length == 0)
            return false;

        if (text.Contains('/'))
        {
            string[] mixedParts = text.Split(
                ' ',
                StringSplitOptions.RemoveEmptyEntries);

            decimal whole = 0;
            string fractionText;

            if (mixedParts.Length == 2)
            {
                if (!decimal.TryParse(
                    mixedParts[0],
                    NumberStyles.Integer,
                    CultureInfo.InvariantCulture,
                    out whole))
                {
                    return false;
                }

                fractionText = mixedParts[1];
            }
            else if (mixedParts.Length == 1)
            {
                fractionText = mixedParts[0];
            }
            else
            {
                return false;
            }

            string[] parts = fractionText.Split('/');

            if (parts.Length != 2)
                return false;

            if (!decimal.TryParse(
                parts[0],
                NumberStyles.Integer,
                CultureInfo.InvariantCulture,
                out decimal numerator))
            {
                return false;
            }

            if (!decimal.TryParse(
                parts[1],
                NumberStyles.Integer,
                CultureInfo.InvariantCulture,
                out decimal denominator))
            {
                return false;
            }

            if (denominator <= 0 || numerator < 0)
                return false;

            decimal fractionalPart = numerator / denominator;

            value = mixedParts.Length == 2
                ? whole < 0
                    ? whole - fractionalPart
                    : whole + fractionalPart
                : fractionalPart;

            return true;
        }

        return decimal.TryParse(
            text,
            NumberStyles.AllowLeadingSign |
            NumberStyles.AllowDecimalPoint,
            CultureInfo.InvariantCulture,
            out value);
    }
}

// =====================================================
// EXISTING REQUEST CONTRACTS
// =====================================================

public record FractionAnswerRequest(
    int Numerator,
    int Denominator,
    int SelectedNumerator);

public record GeneratedFractionAnswerRequest(
    Guid ActivityId,
    string StudentAnswer);
