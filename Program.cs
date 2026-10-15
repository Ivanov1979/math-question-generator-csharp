
using System;
using System.Collections.Generic;
using MySqlConnector;

class Program
{
    // Displays a numbered menu and returns
    // the option selected by the teacher.
    static string SelectFromMenu(
        List<string> options,
        string title)
    {
        Console.WriteLine();
        Console.WriteLine(title);

        if (options.Count == 0)
        {
            Console.WriteLine("No options available.");
            return "";
        }

        for (int i = 0; i < options.Count; i++)
        {
            Console.WriteLine(
                $"{i + 1}. {options[i]}");
        }

        Console.Write("Choice: ");

        string input =
            Console.ReadLine() ?? "";

        if (!int.TryParse(
            input,
            out int choice))
        {
            return "";
        }

        if (choice < 1 ||
            choice > options.Count)
        {
            return "";
        }

        return options[choice - 1];
    }

    // Connects to MySQL and retrieves curriculum information
    // by joining learning objectives, learning details,
    // and assessment indicators.
    static void TestDatabase()
    {
        try
        {
            using var connection =
                DatabaseConnection.CreateConnection();

            connection.Open();

            Console.WriteLine(
                "Successfully connected to MySQL!");

            Console.WriteLine(
                $"Database: {connection.Database}");

            const string sql = @"
                SELECT
                    lo.oa_code,
                    lo.oa_description,
                    ld.detail_description,
                    ai.indicator_description
                FROM LearningObjectives AS lo
                INNER JOIN LearningDetails AS ld
                    ON lo.oa_id = ld.oa_id
                INNER JOIN AssessmentIndicators AS ai
                    ON ai.detail_id = ld.detail_id
                    AND ai.oa_id = lo.oa_id;";

            using var command =
                new MySqlCommand(sql, connection);

            using var reader =
                command.ExecuteReader();

            int recordCount = 0;

            while (reader.Read())
            {
                recordCount++;

                Console.WriteLine();
                Console.WriteLine(
                    $"OA: {reader.GetString(0)}");

                Console.WriteLine(
                    $"Description: {reader.GetString(1)}");

                Console.WriteLine(
                    $"Learning Detail: {reader.GetString(2)}");

                Console.WriteLine(
                    $"Assessment Indicator: {reader.GetString(3)}");
            }

            Console.WriteLine();
            Console.WriteLine(
                $"Curriculum records retrieved: {recordCount}");
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                $"Database test failed: {ex.Message}");
        }
    }

    // Main entry point of the mathematical
    // question generation engine.
    static void Main(string[] args)
    {

        // Launch the interactive fractions website.
        // Preserve the existing console application
        // using the --console argument.
        if (args.Length == 0 ||
            args[0] == "--web")
        {
            FractionWebServer.Run(
                Array.Empty<string>());
            return;
        }

        // ==========================================
        // TEST MYSQL DATABASE CONNECTION AND QUERY
        // ==========================================

        // Runs the MySQL CRUD demonstration without
        // starting the normal question-generation workflow.
        if (args.Length > 0 && args[0] == "--test-crud")
        {
            try
            {
                DatabaseCrud.RunDemo();
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"CRUD demonstration failed: {ex.Message}");
            }

            return;
        }
        if (args.Length > 0 && args[0] == "--test-db")
        {
            TestDatabase();
            return;
        }

        Console.WriteLine(
            "==========================================");

        Console.WriteLine(
            "     Mathematical Question Engine");

        Console.WriteLine(
            "==========================================");

        Console.WriteLine();

        Console.WriteLine(
            "Teacher Question Generator");

        // ==========================================
        // CREATE CURRICULUM MANAGER
        // ==========================================

        CurriculumManager manager =
            new CurriculumManager();

        // ==========================================
        // 1. MATHEMATICAL FIELD
        // ==========================================

        string field =
            SelectFromMenu(
                manager.GetFields(),
                "Select Mathematical Field:");

        if (field == "")
        {
            Console.WriteLine(
                "Invalid mathematical field.");

            return;
        }

        // ==========================================
        // 2. MATHEMATICAL CONTENT
        // ==========================================

        List<string> contents =
            manager.GetContents(field);

        string content =
            SelectFromMenu(
                contents,
                "Select Mathematical Content:");

        if (content == "")
        {
            Console.WriteLine(
                "Invalid mathematical content.");

            return;
        }

        // ==========================================
        // 3. GRADE
        // ==========================================

        List<string> grades =
            manager.GetGradesForContent(
                field,
                content);

        string grade =
            SelectFromMenu(
                grades,
                "Select Grade:");

        if (grade == "")
        {
            Console.WriteLine(
                "Invalid grade.");

            return;
        }

        // ==========================================
        // 4. LEARNING OBJECTIVE
        // ==========================================

        List<string> oas =
            manager.GetOAs(
                field,
                content,
                grade);

        string oa =
            SelectFromMenu(
                oas,
                "Select Learning Objective (OA):");

        if (oa == "")
        {
            Console.WriteLine(
                "Invalid Learning Objective.");

            return;
        }

        // ==========================================
        // 5. LEARNING DETAIL
        // ==========================================

        List<string> learningDetails =
            manager.GetLearningDetails(
                field,
                content,
                grade,
                oa);

        string learningDetail =
            SelectFromMenu(
                learningDetails,
                "Select Learning Detail:");

        if (learningDetail == "")
        {
            Console.WriteLine(
                "Invalid learning detail.");

            return;
        }

        // ==========================================
        // 6. ASSESSMENT INDICATOR
        // ==========================================

        List<string> indicators =
            manager.GetIndicators(
                field,
                content,
                grade,
                oa,
                learningDetail);

        string indicator =
            SelectFromMenu(
                indicators,
                "Select Assessment Indicator:");

        if (indicator == "")
        {
            Console.WriteLine(
                "Invalid assessment indicator.");

            return;
        }

        // ==========================================
        // 7. MATHEMATICAL SKILL
        // ==========================================

        string skill =
            SelectFromMenu(
                manager.GetMathematicalSkills(),
                "Select Mathematical Skill:");

        if (skill == "")
        {
            Console.WriteLine(
                "Invalid mathematical skill.");

            return;
        }

        // ==========================================
        // 8. DIFFICULTY
        // ==========================================

        string difficulty =
            SelectFromMenu(
                manager.GetDifficulties(),
                "Select Difficulty:");

        if (difficulty == "")
        {
            Console.WriteLine(
                "Invalid difficulty.");

            return;
        }

        // ==========================================
        // 9. EXERCISE TYPE
        // ==========================================

        // Exercise types will be incorporated
        // when the exercise classification
        // is added to the engine.

        string exerciseType = "General";

        // ==========================================
        // CREATE GENERATION REQUEST
        // ==========================================

        GenerationRequest request =
            new GenerationRequest(
                field,
                content,
                grade,
                oa,
                learningDetail,
                indicator,
                skill,
                difficulty,
                exerciseType);

        // ==========================================
        // DISPLAY GENERATION REQUEST
        // ==========================================

        Console.WriteLine();

        Console.WriteLine(
            "==========================================");

        Console.WriteLine(
            "GENERATION REQUEST");

        Console.WriteLine(
            "==========================================");

        Console.WriteLine(
            $"Field: {request.Field}");

        Console.WriteLine(
            $"Content: {request.Content}");

        Console.WriteLine(
            $"Grade: {request.Grade}");

        Console.WriteLine(
            $"OA: {request.OA}");

        Console.WriteLine(
            $"Learning Detail: " +
            $"{request.LearningDetail}");

        Console.WriteLine(
            $"Assessment Indicator: " +
            $"{request.AssessmentIndicator}");

        Console.WriteLine(
            $"Mathematical Skill: " +
            $"{request.MathematicalSkill}");

        Console.WriteLine(
            $"Difficulty: " +
            $"{request.Difficulty}");

        Console.WriteLine(
            $"Exercise Type: " +
            $"{request.ExerciseType}");

        // ==========================================
        // CREATE QUESTION GENERATOR
        // ==========================================

        QuestionGenerator generator =
            new QuestionGenerator();

        // ==========================================
        // GENERATE QUESTION
        // ==========================================

        Activity generatedQuestion =
            generator.GenerateQuestion(request);

        // ==========================================
        // DISPLAY GENERATED QUESTION
        // ==========================================

        Console.WriteLine();

        Console.WriteLine(
            "==========================================");

        Console.WriteLine(
            "GENERATED QUESTION");

        Console.WriteLine(
            "==========================================");

        Console.WriteLine();

        Console.WriteLine(
            generatedQuestion.GetQuestion());

        Console.WriteLine();

        Console.WriteLine(
            "Suggested Answer:");

        Console.WriteLine(
            generatedQuestion.GetAnswer());

        // ==========================================
        // END
        // ==========================================

        Console.WriteLine();

        Console.WriteLine(
            "==========================================");

        Console.WriteLine(
            "Question generated successfully.");

        Console.WriteLine(
            "==========================================");
    }
}