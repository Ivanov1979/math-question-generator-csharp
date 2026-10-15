
using System;
using System.Globalization;
using System.Linq;
using System.Text;

public class FractionQuestionGenerator
{
    private readonly Random random = new();

    private readonly (int n, int d)[] commonFractions =
    {
        (1, 4), (1, 3), (1, 2), (2, 3), (3, 4)
    };

    private static readonly string[] MathematicalSkills =
    {
        "Representar",
        "Modelar",
        "Resolver problemas",
        "Argumentar y comunicar"
    };

    // =====================================================
    // ORIGINAL GENERATION REQUEST: GRADE 3 OA11
    // =====================================================

    public Activity GenerateQuestion(GenerationRequest request)
    {
        string grade = Normalize(request.Grade);
        string oa = Normalize(request.OA);

        if ((grade.Contains("3rd") ||
             grade.Contains("3°") ||
             grade.Contains("tercero") ||
             grade == "3") &&
            (oa == "oa11" || oa == "oa011"))
        {
            return GenerateThirdGrade(request);
        }

        return CreateActivity(
            request,
            "Fraction generator for this grade and OA is not implemented yet.",
            "N/A");
    }

    // =====================================================
    // MYSQL CURRICULUM GENERATOR
    // =====================================================

    // Backward-compatible overload.
    public Activity GenerateFromRule(
        FractionGenerationRule rule,
        string difficulty)
    {
        return GenerateFromRule(
            rule,
            difficulty,
            rule.MathematicalSkill);
    }

    // New method: receives the teacher's selected skill.
    public Activity GenerateFromRule(
        FractionGenerationRule rule,
        string difficulty,
        string mathematicalSkill)
    {
        if (rule == null)
            throw new ArgumentNullException(nameof(rule));

        if (difficulty != "Easy" &&
            difficulty != "Medium" &&
            difficulty != "Hard")
        {
            throw new ArgumentException(
                "Difficulty must be Easy, Medium, or Hard.");
        }

        mathematicalSkill = mathematicalSkill?.Trim() ?? "";

        if (!MathematicalSkills.Contains(mathematicalSkill))
        {
            throw new ArgumentException(
                "Please select a valid mathematical skill.");
        }

        string type = rule.ActivityType.Trim();

        int[] denominators = type == "EqualSharing"
            ? Array.Empty<int>()
            : ParseDenominators(rule.AllowedDenominators);

        (string question, string answer) result =
            type switch
            {
                "EqualSharing" =>
                    GenerateEqualSharing(difficulty),

                "EquivalentFractions" =>
                    GenerateEquivalent(denominators, difficulty),

                "SameDenominatorComparison" =>
                    GenerateComparisonRule(
                        denominators, difficulty, true),

                "DifferentDenominatorComparison" =>
                    GenerateComparisonRule(
                        denominators, difficulty, false),

                "ImproperToMixed" =>
                    GenerateImproperToMixed(
                        denominators, difficulty),

                "MixedToImproper" =>
                    GenerateMixedToImproper(
                        denominators, difficulty),

                "FractionToDecimal" =>
                    GenerateFractionToDecimal(
                        denominators, difficulty),

                "SameDenominatorOperations" =>
                    GenerateOperations(
                        denominators, difficulty, true),

                "DifferentDenominatorOperations" =>
                    GenerateOperations(
                        denominators, difficulty, false),

                "FractionOperations" =>
                    GenerateOperations(
                        denominators, difficulty, false),

                "MixedNumberOperations" =>
                    GenerateMixedOperations(
                        denominators, difficulty),

                _ => throw new NotSupportedException(
                    $"LearningDetailId {rule.LearningDetailId}: " +
                    $"activity type '{type}' is not implemented yet.")
            };

        // Skill-specific instructional prompts.
        // These preserve the original numerical answer.
        string question = mathematicalSkill switch
        {
            "Representar" =>
                "REPRESENTAR\n" +
                "Use a drawing, fraction bar, or number line " +
                "to represent the quantities.\n\n" +
                result.question,

            "Modelar" =>
                "MODELAR\n" +
                "Identify the mathematical relationship " +
                "and construct a model before answering.\n\n" +
                result.question,

            "Resolver problemas" =>
                "RESOLVER PROBLEMAS\n" +
                "Choose a strategy, solve the exercise, " +
                "and check whether your answer makes sense.\n\n" +
                result.question,

            "Argumentar y comunicar" =>
                "ARGUMENTAR Y COMUNICAR\n" +
                "Explain your reasoning and justify " +
                "the mathematical result.\n\n" +
                result.question,

            _ => result.question
        };

        return new Activity(
            rule.Grade,
            rule.OACode,
            rule.LearningDetail,
            mathematicalSkill,
            difficulty,
            question,
            result.answer);
    }

    // =====================================================
    // GRADE 4 — EQUAL SHARING
    // =====================================================

    private (string, string) GenerateEqualSharing(
        string difficulty)
    {
        int groups;
        int objectsPerGroup;

        switch (difficulty)
        {
            case "Easy":
                groups = random.Next(2, 5);
                objectsPerGroup = random.Next(2, 5);
                break;

            case "Medium":
                groups = random.Next(3, 7);
                objectsPerGroup = random.Next(3, 9);
                break;

            default:
                groups = random.Next(4, 9);
                objectsPerGroup = random.Next(5, 13);
                break;
        }

        int totalObjects = groups * objectsPerGroup;

        return (
            $"There are {totalObjects} counters.\n\n" +
            $"Share them equally among {groups} students.\n\n" +
            "How many counters does each student receive?\n\n" +
            "Enter a whole number.",
            objectsPerGroup.ToString(CultureInfo.InvariantCulture)
        );
    }

    // =====================================================
    // EQUIVALENT FRACTIONS
    // =====================================================

    private (string, string) GenerateEquivalent(
        int[] ds, string difficulty)
    {
        int d = Pick(ds);
        int n = random.Next(1, d);

        int factor = difficulty switch
        {
            "Easy" => 2,
            "Medium" => random.Next(2, 5),
            _ => random.Next(4, 8)
        };

        return (
            $"Find a fraction equivalent to {n}/{d} " +
            $"by multiplying its numerator and denominator by {factor}.",
            $"{n * factor}/{d * factor}"
        );
    }

    // =====================================================
    // FRACTION COMPARISON
    // =====================================================

    private (string, string) GenerateComparisonRule(
        int[] ds,
        string difficulty,
        bool sameDenominator)
    {
        int d1 = Pick(ds);
        int d2 = sameDenominator
            ? d1
            : PickDifferent(ds, d1);

        int n1 = random.Next(1, d1);
        int n2 = random.Next(1, d2);

        int left = n1 * d2;
        int right = n2 * d1;

        string answer = left < right ? "<" :
                        left > right ? ">" : "=";

        return (
            $"Compare {n1}/{d1} and {n2}/{d2}. " +
            "Enter <, >, or =.",
            answer
        );
    }

    // =====================================================
    // IMPROPER FRACTION TO MIXED NUMBER
    // =====================================================

    private (string, string) GenerateImproperToMixed(
        int[] ds, string difficulty)
    {
        int d = Pick(ds);
        int whole = PickWhole(difficulty);
        int remainder = random.Next(1, d);
        int numerator = whole * d + remainder;

        return (
            $"Express {numerator}/{d} as a mixed number. " +
            "Use the format: whole numerator/denominator.",
            $"{whole} {remainder}/{d}"
        );
    }

    // =====================================================
    // MIXED NUMBER TO IMPROPER FRACTION
    // =====================================================

    private (string, string) GenerateMixedToImproper(
        int[] ds, string difficulty)
    {
        int d = Pick(ds);
        int whole = PickWhole(difficulty);
        int n = random.Next(1, d);

        return (
            $"Convert {whole} {n}/{d} into an improper fraction.",
            $"{whole * d + n}/{d}"
        );
    }

    // =====================================================
    // FRACTION TO DECIMAL
    // =====================================================

    private (string, string) GenerateFractionToDecimal(
        int[] ds, string difficulty)
    {
        int[] terminating = ds
            .Where(d => d > 1 && HasTerminatingDecimal(d))
            .ToArray();

        if (terminating.Length == 0)
        {
            throw new NotSupportedException(
                "No permitted denominators produce terminating decimals.");
        }

        int d = Pick(terminating);
        int n = random.Next(1, d);

        decimal value = (decimal)n / d;

        return (
            $"Write {n}/{d} as a decimal. Use a decimal point.",
            value.ToString(CultureInfo.InvariantCulture)
        );
    }

    // =====================================================
    // FRACTION OPERATIONS
    // =====================================================

    private (string, string) GenerateOperations(
        int[] ds,
        string difficulty,
        bool sameDenominator)
    {
        int d1 = Pick(ds);
        int d2 = sameDenominator ? d1 : Pick(ds);

        int n1 = random.Next(1, d1);
        int n2 = random.Next(1, d2);

        bool subtract = random.Next(2) == 0;

        int numerator = subtract
            ? n1 * d2 - n2 * d1
            : n1 * d2 + n2 * d1;

        int denominator = d1 * d2;

        if (numerator < 0)
        {
            subtract = false;
            numerator = n1 * d2 + n2 * d1;
        }

        string symbol = subtract ? "-" : "+";

        return (
            $"Calculate {n1}/{d1} {symbol} {n2}/{d2}. " +
            "Write the result as a fraction.",
            Simplify(numerator, denominator)
        );
    }

    // =====================================================
    // MIXED NUMBER OPERATIONS
    // =====================================================

    private (string, string) GenerateMixedOperations(
        int[] ds, string difficulty)
    {
        int d = Pick(ds);
        int w1 = PickWhole(difficulty);
        int w2 = PickWhole(difficulty);

        int n1 = random.Next(1, d);
        int n2 = random.Next(1, d);

        int totalNumerator =
            (w1 * d + n1) + (w2 * d + n2);

        int whole = totalNumerator / d;
        int remainder = totalNumerator % d;

        string answer = remainder == 0
            ? whole.ToString()
            : $"{whole} {remainder}/{d}";

        return (
            $"Calculate {w1} {n1}/{d} + {w2} {n2}/{d}. " +
            "Express the answer as a mixed number.",
            answer
        );
    }

    // =====================================================
    // GENERATION HELPERS
    // =====================================================

    private int[] ParseDenominators(string? text)
    {
        int[] values = (text ?? "")
            .Split(
                new[] { ',', ';', ' ' },
                StringSplitOptions.RemoveEmptyEntries)
            .Select(s => int.TryParse(s, out int n) ? n : 0)
            .Where(n => n >= 2 && n <= 100)
            .Distinct()
            .ToArray();

        if (values.Length == 0)
        {
            throw new NotSupportedException(
                "This learning detail has no valid allowed denominators.");
        }

        return values;
    }

    private int Pick(int[] values) =>
        values[random.Next(values.Length)];

    private int PickDifferent(int[] values, int excluded)
    {
        int[] alternatives = values
            .Where(x => x != excluded)
            .ToArray();

        if (alternatives.Length == 0)
        {
            throw new NotSupportedException(
                "At least two denominators are required.");
        }

        return Pick(alternatives);
    }

    private int PickWhole(string difficulty) =>
        difficulty switch
        {
            "Easy" => random.Next(1, 3),
            "Medium" => random.Next(2, 5),
            _ => random.Next(3, 9)
        };

    private static bool HasTerminatingDecimal(
        int denominator)
    {
        while (denominator % 2 == 0)
            denominator /= 2;

        while (denominator % 5 == 0)
            denominator /= 5;

        return denominator == 1;
    }

    private static string Simplify(int n, int d)
    {
        int gcd = Gcd(Math.Abs(n), d);
        return $"{n / gcd}/{d / gcd}";
    }

    private static int Gcd(int a, int b)
    {
        while (b != 0)
        {
            int remainder = a % b;
            a = b;
            b = remainder;
        }

        return a == 0 ? 1 : a;
    }

    // =====================================================
    // PRESERVED GRADE 3 OA11 GENERATION
    // =====================================================

    private Activity GenerateThirdGrade(
        GenerationRequest request)
    {
        string indicator =
            Normalize(request.AssessmentIndicator);

        if (indicator.Contains("cuerda") ||
            indicator.Contains("reparten equitativamente"))
            return GenerateSharing(request);

        if (indicator.Contains("magnitudes continuas") &&
            indicator.Contains("unitarias"))
            return GenerateUnitMeasurement(request);

        if (indicator.Contains("cortes") ||
            indicator.Contains("dobleces"))
            return GenerateEqualParts(request);

        if (indicator.Contains("numerador") &&
            indicator.Contains("denominador"))
            return GenerateExplanation(request);

        if (indicator.Contains("problemas"))
            return GenerateWordProblem(request);

        if (indicator.Contains("comparan fracciones"))
            return GenerateComparison(request);

        if (indicator.Contains("recta numerica") ||
            indicator.Contains("escalas graduadas"))
            return GenerateNumberLine(request);

        if (indicator.Contains("representaciones pictoricas") ||
            indicator.Contains("representaciones de fracciones") ||
            indicator.Contains("partes de un todo"))
            return GeneratePictorial(request);

        if (indicator.Contains("confeccionan") ||
            indicator.Contains("fracciones"))
            return GeneratePictorial(request);

        return CreateActivity(
            request,
            "No exercise template is mapped to this assessment indicator yet: " +
            request.AssessmentIndicator,
            "N/A");
    }

    private Activity GeneratePictorial(
        GenerationRequest request)
    {
        var (n, d) = PickFraction();

        string bar = new string('■', n) +
                     new string('□', d - n);

        return CreateActivity(
            request,
            $"Observe the fraction model:\n\n{bar}\n\n" +
            $"The whole has {d} equal parts.\n" +
            "Write the fraction represented by the filled parts.",
            $"{n}/{d}");
    }

    private Activity GenerateNumberLine(
        GenerationRequest request)
    {
        var (n, d) = PickFraction();

        string line = "0 ";

        for (int i = 1; i <= d; i++)
            line += i == n ? "──●" : "──|";

        line += " 1";

        return CreateActivity(
            request,
            $"Look at the number line:\n\n{line}\n\n" +
            $"The interval from 0 to 1 is divided into {d} equal parts.\n" +
            "What fraction is marked by ●?",
            $"{n}/{d}");
    }

    private Activity GenerateSharing(
        GenerationRequest request)
    {
        int people = PickDenominator();

        return CreateActivity(
            request,
            $"A rope is shared equally among {people} students.\n\n" +
            "What fraction of the rope does each student receive?",
            $"1/{people}");
    }

    private Activity GenerateUnitMeasurement(
        GenerationRequest request)
    {
        int d = PickDenominator();

        return CreateActivity(
            request,
            $"A one-meter strip is divided into {d} equal parts.\n\n" +
            "What fraction of a meter is the length of one part?",
            $"1/{d}");
    }

    private Activity GenerateEqualParts(
        GenerationRequest request)
    {
        int d = PickDenominator();

        return CreateActivity(
            request,
            $"You fold a paper strip into {d} equal parts.\n\n" +
            "If you color one part, what fraction of the strip is colored?",
            $"1/{d}");
    }

    private Activity GenerateComparison(
        GenerationRequest request)
    {
        string indicator =
            Normalize(request.AssessmentIndicator);

        int n1, d1, n2, d2;

        if (indicator.Contains("unitarias"))
        {
            d1 = PickDenominator();

            do
            {
                d2 = PickDenominator();
            } while (d2 == d1);

            n1 = 1;
            n2 = 1;
        }
        else
        {
            d1 = random.Next(3, 5);
            d2 = d1;
            n1 = random.Next(1, d1);

            do
            {
                n2 = random.Next(1, d1);
            } while (n2 == n1);
        }

        string answer = n1 * d2 < n2 * d1 ? "<" :
                        n1 * d2 > n2 * d1 ? ">" : "=";

        return CreateActivity(
            request,
            $"Compare {n1}/{d1} and {n2}/{d2}.\n\n" +
            "Which symbol makes the comparison correct: <, >, or = ?",
            answer);
    }

    private Activity GenerateExplanation(
        GenerationRequest request)
    {
        var (n, d) = PickFraction();

        return CreateActivity(
            request,
            $"Consider the fraction {n}/{d}.\n\n" +
            "Explain what the numerator and denominator mean " +
            "when the fraction represents part of a whole.",
            $"The denominator {d} tells us the whole is divided " +
            $"into {d} equal parts. The numerator {n} tells us " +
            $"that {n} of those parts are selected.");
    }

    private Activity GenerateWordProblem(
        GenerationRequest request)
    {
        var (n, d) = PickFraction();

        return CreateActivity(
            request,
            $"A chocolate bar is divided into {d} equal pieces.\n" +
            $"A student eats {n} pieces.\n\n" +
            "What fraction of the whole chocolate bar was eaten?",
            $"{n}/{d}");
    }

    private (int n, int d) PickFraction() =>
        commonFractions[random.Next(commonFractions.Length)];

    private int PickDenominator()
    {
        int[] denominators = { 2, 3, 4 };
        return denominators[random.Next(denominators.Length)];
    }

    private string Normalize(string? value)
    {
        string text = (value ?? "")
            .Trim()
            .ToLowerInvariant()
            .Normalize(NormalizationForm.FormD);

        var result = new StringBuilder();

        foreach (char c in text)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) !=
                UnicodeCategory.NonSpacingMark)
            {
                result.Append(c);
            }
        }

        return result.ToString()
            .Normalize(NormalizationForm.FormC);
    }

    private Activity CreateActivity(
        GenerationRequest request,
        string question,
        string answer)
    {
        return new Activity(
            request.Grade,
            request.OA,
            request.AssessmentIndicator,
            request.MathematicalSkill,
            request.Difficulty,
            question,
            answer);
    }
}
