
using System;

// Generates randomized mathematics questions
// from a complete GenerationRequest.
public class QuestionGenerator
{
    private readonly Random random = new Random();

    // Receives the teacher's complete generation request
    // and selects the appropriate generator.
    public Activity GenerateQuestion(GenerationRequest request)
    {
        // OA09: Numerical Patterns.
        if (request.Field == "Patterns and Algebra" &&
            request.Content == "Numerical Patterns" &&
            request.OA == "OA09")
        {
            return GenerateNumericalPattern(request);
        }
        // FRACTIONS: Grades 3–6.
        // Route fraction requests to the dedicated generator.
        // Other mathematical content remains unchanged.
        if (request.Content.Equals(
                "Fractions",
                StringComparison.OrdinalIgnoreCase))
        {
            return new FractionQuestionGenerator()
                .GenerateQuestion(request);
        }
        // Fallback when a generator has not yet been
        // implemented for the selected content.
        return new Activity(
            request.Grade,
            request.OA,
            request.AssessmentIndicator,
            request.MathematicalSkill,
            request.Difficulty,
            "Generator not yet available for this content.",
            "N/A"
        );
    }

    // Selects the question format according to
    // the teacher's chosen learning detail.
    private Activity GenerateNumericalPattern(
        GenerationRequest request)
    {
        // Generate an input-output-table question
        // when that learning detail is selected.
        if (request.LearningDetail ==
            "Identify and describe relationships in numerical patterns using input-output tables.")
        {
            return GenerateInputOutputTable(request);
        }

        // Otherwise, use the existing numerical-
        // sequence generator.
        int start;
        int increase;

        // Difficulty controls numerical complexity.
        if (request.Difficulty == "Easy")
        {
            start = random.Next(1, 10);
            increase = random.Next(2, 6);
        }
        else if (request.Difficulty == "Medium")
        {
            start = random.Next(5, 21);
            increase = random.Next(4, 10);
        }
        else
        {
            start = random.Next(10, 41);
            increase = random.Next(6, 16);
        }

        int second = start + increase;
        int third = second + increase;
        int fourth = third + increase;
        int fifth = fourth + increase;
        int sixth = fifth + increase;

        string question;
        string answer;

        // ==========================================
        // REPRESENT: Complete a numerical sequence.
        // ==========================================
        if (request.MathematicalSkill == "Represent")
        {
            question =
                "Complete the numerical sequence:\n\n" +
                $"{start}, {second}, {third}, __, " +
                $"{fifth}, {sixth}";

            answer = fourth.ToString();
        }

        // ==========================================
        // MODEL: Apply a numerical-pattern rule.
        // ==========================================
        else if (request.MathematicalSkill == "Model")
        {
            question =
                $"A numerical pattern starts at {start} " +
                $"and increases by {increase} each time.\n\n" +
                "What is the fifth term of the pattern?";

            answer = fifth.ToString();
        }

        // ==========================================
        // SOLVE PROBLEMS: Use a pattern in context.
        // ==========================================
        else if (request.MathematicalSkill == "Solve Problems")
        {
            question =
                $"A theater has {start} seats in the first row.\n" +
                $"Each new row has {increase} more seats " +
                "than the previous row.\n\n" +
                "How many seats are in the fifth row?";

            answer = fifth.ToString();
        }

        // ==========================================
        // ARGUE & COMMUNICATE: Explain a pattern.
        // ==========================================
        else
        {
            question =
                "Consider the sequence:\n\n" +
                $"{start}, {second}, {third}, " +
                $"{fourth}, {fifth}\n\n" +
                "Explain the rule used to generate " +
                "the sequence.";

            answer =
                $"Add {increase} to each previous term.";
        }

        return new Activity(
            request.Grade,
            request.OA,
            request.AssessmentIndicator,
            request.MathematicalSkill,
            request.Difficulty,
            question,
            answer
        );
    }

    // Generates input-output-table questions according
    // to the selected mathematical skill and difficulty.
    private Activity GenerateInputOutputTable(
        GenerationRequest request)
    {
        int multiplier;

        // Difficulty controls numerical complexity.
        if (request.Difficulty == "Easy")
        {
            multiplier = random.Next(2, 5);
        }
        else if (request.Difficulty == "Medium")
        {
            multiplier = random.Next(5, 10);
        }
        else
        {
            multiplier = random.Next(10, 16);
        }

        int firstOutput = multiplier;
        int secondOutput = 2 * multiplier;
        int thirdOutput = 3 * multiplier;
        int fourthOutput = 4 * multiplier;
        int fifthOutput = 5 * multiplier;

        string table =
            "Input | Output\n" +
            "------+-------\n" +
            $"  1   |   {firstOutput}\n" +
            $"  2   |   {secondOutput}\n" +
            $"  3   |   {thirdOutput}\n" +
            $"  4   |   {fourthOutput}\n";

        string question;
        string answer;

        // ==========================================
        // REPRESENT: Complete a missing table value.
        // ==========================================
        if (request.MathematicalSkill == "Represent")
        {
            question =
                "Complete the input-output table:\n\n" +
                "Input | Output\n" +
                "------+-------\n" +
                $"  1   |   {firstOutput}\n" +
                $"  2   |   {secondOutput}\n" +
                $"  3   |   {thirdOutput}\n" +
                "  4   |   ?\n\n" +
                "What number belongs in the empty cell?";

            answer = fourthOutput.ToString();
        }

        // ==========================================
        // MODEL: Express the relationship as a rule.
        // ==========================================
        else if (request.MathematicalSkill == "Model")
        {
            question =
                "Study the input-output table:\n\n" +
                table +
                "\nWrite a rule using x for the input " +
                "and y for the output.";

            answer = $"y = {multiplier}x";
        }

        // ==========================================
        // SOLVE PROBLEMS: Apply the rule to a new input.
        // ==========================================
        else if (request.MathematicalSkill == "Solve Problems")
        {
            question =
                "A machine transforms numbers according " +
                "to this table:\n\n" +
                table +
                "\nWhat output will the machine produce " +
                "when the input is 5?";

            answer = fifthOutput.ToString();
        }

        // ==========================================
        // ARGUE & COMMUNICATE: Explain and justify.
        // ==========================================
        else
        {
            question =
                "Study the input-output table:\n\n" +
                table +
                "\nExplain the relationship between " +
                "the input and output values. " +
                "Use two rows from the table to " +
                "justify your answer.";

            answer =
                $"The output equals the input multiplied " +
                $"by {multiplier}. For example, " +
                $"1 × {multiplier} = {firstOutput} and " +
                $"3 × {multiplier} = {thirdOutput}.";
        }

        return new Activity(
            request.Grade,
            request.OA,
            request.AssessmentIndicator,
            request.MathematicalSkill,
            request.Difficulty,
            question,
            answer
        );
    }
}