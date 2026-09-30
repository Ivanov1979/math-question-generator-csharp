using System;

// Contains mathematical generation rules used
// to control the complexity of generated exercises.
public static class GenerationRules
{
    // Returns the minimum value that can be used
    // when generating numerical parameters.
    public static int GetMinimumValue(
        string grade,
        string difficulty)
    {
        int gradeNumber = GetGradeNumber(grade);

        if (difficulty == "Easy")
        {
            return 1;
        }

        if (difficulty == "Medium")
        {
            return Math.Max(2, gradeNumber * 2);
        }

        return Math.Max(5, gradeNumber * 5);
    }


    // Returns a general maximum numerical value.
    // Individual mathematical contents can later
    // impose more specific curriculum restrictions.
    public static int GetMaximumValue(
        string grade,
        string difficulty)
    {
        int gradeNumber = GetGradeNumber(grade);

        if (difficulty == "Easy")
        {
            return gradeNumber * 10;
        }

        if (difficulty == "Medium")
        {
            return gradeNumber * 25;
        }

        return gradeNumber * 100;
    }


    // Returns the approximate number of reasoning
    // steps appropriate for the selected difficulty.
    public static int GetReasoningSteps(
        string difficulty)
    {
        if (difficulty == "Easy")
        {
            return 1;
        }

        if (difficulty == "Medium")
        {
            return 2;
        }

        return 3;
    }


    // Converts the grade label used by the interface
    // into a numerical grade value.
    private static int GetGradeNumber(string grade)
    {
        if (grade == "1st Grade") return 1;
        if (grade == "2nd Grade") return 2;
        if (grade == "3rd Grade") return 3;
        if (grade == "4th Grade") return 4;
        if (grade == "5th Grade") return 5;
        if (grade == "6th Grade") return 6;

        return 1;
    }
}