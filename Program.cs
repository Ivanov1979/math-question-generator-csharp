using System;
using System.Collections.Generic;

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

        // Prevents the program from displaying
        // an empty menu.
        if (options.Count == 0)
        {
            Console.WriteLine("No options available.");
            return "";
        }

        // Displays each menu option with a number.
        for (int i = 0; i < options.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {options[i]}");
        }

        Console.Write("Choice: ");

        string input = Console.ReadLine() ?? "";

        // Validates that the user entered a number.
        if (!int.TryParse(input, out int choice))
        {
            return "";
        }

        // Validates that the number corresponds
        // to an available menu option.
        if (choice < 1 || choice > options.Count)
        {
            return "";
        }

        return options[choice - 1];
    }


    // Main entry point of the mathematical
    // question generation engine.
    static void Main(string[] args)
    {
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

        // The curriculum manager controls the
        // relationships between field, content,
        // grade, OA, and assessment indicator.
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

        // Only contents belonging to the selected
        // mathematical field are displayed.
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

        // Only grades where the selected content
        // exists are displayed.
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
        // 4. LEARNING OBJECTIVE (OA)
        // ==========================================

        // Only OAs associated with the selected
        // field, content, and grade are displayed.
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
        // 5. ASSESSMENT INDICATOR
        // ==========================================

        // Retrieves assessment indicators associated
        // with the selected curriculum combination.
        List<string> indicators =
            manager.GetIndicators(
                field,
                content,
                grade,
                oa);

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
        // 6. MATHEMATICAL SKILL
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
        // 7. DIFFICULTY
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
        // 8. EXERCISE TYPE
        // ==========================================

        // Exercise types will be incorporated later.
        // For the current engine prototype we use
        // "General" as the default exercise type.
        string exerciseType = "General";


        // ==========================================
        // CREATE GENERATION REQUEST
        // ==========================================

        // The GenerationRequest struct stores all
        // parameters selected by the teacher.
        GenerationRequest request =
            new GenerationRequest(
                field,
                content,
                grade,
                oa,
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
            $"Assessment Indicator: " +
            $"{request.AssessmentIndicator}");

        Console.WriteLine(
            $"Mathematical Skill: " +
            $"{request.MathematicalSkill}");

        Console.WriteLine(
            $"Difficulty: {request.Difficulty}");

        Console.WriteLine(
            $"Exercise Type: {request.ExerciseType}");


        // ==========================================
        // CREATE QUESTION GENERATOR
        // ==========================================

        QuestionGenerator generator =
            new QuestionGenerator();


        // ==========================================
        // GENERATE RANDOM QUESTION
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
        // END PROGRAM
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