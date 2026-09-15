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
        // Currently OA09 numerical patterns is implemented.
        // More generators will be added for other contents.
        if (request.Field == "Patterns" &&
            request.Content == "Numerical Patterns" &&
            request.OA == "OA09")
        {
            return GenerateNumericalPattern(request);
        }

        // Fallback question if a generator has not
        // yet been implemented for the selected content.
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


    // Generates randomized numerical-pattern questions.
    private Activity GenerateNumericalPattern(
        GenerationRequest request)
    {
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
        // REPRESENT
        // ==========================================

        if (request.MathematicalSkill == "Represent")
        {
            question =
                $"Complete the numerical sequence:\n\n" +
                $"{start}, {second}, {third}, __, " +
                $"{fifth}, {sixth}";

            answer = fourth.ToString();
        }


        // ==========================================
        // MODEL
        // ==========================================

        else if (request.MathematicalSkill == "Model")
        {
            question =
                $"A numerical pattern starts at {start} " +
                $"and increases by {increase} each time.\n\n" +
                $"What is the fifth term of the pattern?";

            answer = fifth.ToString();
        }


        // ==========================================
        // SOLVE PROBLEMS
        // ==========================================

        else if (
            request.MathematicalSkill == "Solve Problems")
        {
            question =
                $"A theater has {start} seats in the first row.\n" +
                $"Each new row has {increase} more seats " +
                $"than the previous row.\n\n" +
                $"How many seats are in the fifth row?";

            answer = fifth.ToString();
        }


        // ==========================================
        // ARGUE & COMMUNICATE
        // ==========================================

        else
        {
            question =
                $"Consider the sequence:\n\n" +
                $"{start}, {second}, {third}, " +
                $"{fourth}, {fifth}\n\n" +
                $"Explain the rule used to generate " +
                $"the sequence.";

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
}