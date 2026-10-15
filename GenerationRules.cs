
using System;
using System.Collections.Generic;
using System.Linq;

// Mathematical rules for curriculum-aligned
// interactive activity generation.
public static class GenerationRules
{
    public static int GetMinimumValue(
        string grade,
        string difficulty)
    {
        int gradeNumber = GetGradeNumber(grade);

        if (difficulty == "Easy")
            return 1;

        if (difficulty == "Medium")
            return Math.Max(2, gradeNumber * 2);

        return Math.Max(5, gradeNumber * 5);
    }

    public static int GetMaximumValue(
        string grade,
        string difficulty)
    {
        int gradeNumber = GetGradeNumber(grade);

        if (difficulty == "Easy")
            return gradeNumber * 10;

        if (difficulty == "Medium")
            return gradeNumber * 25;

        return gradeNumber * 100;
    }

    public static int GetReasoningSteps(
        string difficulty)
    {
        if (difficulty == "Easy")
            return 1;

        if (difficulty == "Medium")
            return 2;

        return 3;
    }

    // Restrict denominators using the values
    // defined by the selected curriculum rule.
    public static int[] GetAllowedDenominators(
        string allowedDenominators,
        string difficulty)
    {
        if (string.IsNullOrWhiteSpace(allowedDenominators))
        {
            throw new ArgumentException(
                "The curriculum rule must specify " +
                "allowed denominators.");
        }

        int[] denominators = allowedDenominators
            .Trim('[', ']', ' ', '"')
            .Split(
                new char[] { ',', ';', ' ' },
                StringSplitOptions.RemoveEmptyEntries)
            .Select(value =>
            {
                if (!int.TryParse(value, out int number))
                {
                    throw new ArgumentException(
                        "Invalid denominator: " + value);
                }

                return number;
            })
            .Where(number => number >= 2)
            .Distinct()
            .OrderBy(number => number)
            .ToArray();

        if (denominators.Length == 0)
        {
            throw new ArgumentException(
                "No valid denominators were found.");
        }

        // Difficulty changes the selection range,
        // without introducing forbidden denominators.
        int count = difficulty switch
        {
            "Easy" => Math.Max(
                1, (denominators.Length + 2) / 3),

            "Medium" => Math.Max(
                1, (denominators.Length * 2 + 2) / 3),

            _ => denominators.Length
        };

        return denominators.Take(count).ToArray();
    }

    // Determine which interactive representation
    // can be used for a generation strategy.
    public static string GetInteractionType(
        string activityType)
    {
        string type = (activityType ?? "")
            .Trim()
            .ToLowerInvariant();

        return type switch
        {
            "equalsharing" => "EqualSharing",
            "partofwhole" => "FractionShading",
            "fractionshading" => "FractionShading",
            "numberline" => "NumberLine",
            "fractionaddition" => "FractionInput",
            "fractionsubtraction" => "FractionInput",
            "fractioncomparison" => "Comparison",
            "equivalentfractions" => "FractionInput",

            // Unknown types must be explicitly mapped.
            _ => "Unsupported"
        };
    }

    public static bool IsSupportedInteraction(
        string activityType)
    {
        return GetInteractionType(activityType)
            != "Unsupported";
    }

    private static int GetGradeNumber(string grade)
    {
        return grade switch
        {
            "1st Grade" => 1,
            "2nd Grade" => 2,
            "3rd Grade" => 3,
            "4th Grade" => 4,
            "5th Grade" => 5,
            "6th Grade" => 6,
            _ => 1
        };
    }
}
