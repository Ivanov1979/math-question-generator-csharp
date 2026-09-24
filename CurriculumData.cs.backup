using System.Collections.Generic;

// Provides the curriculum structure used by the
// mathematical question generation engine.
public static class CurriculumData
{
    // Returns all mathematical fields in the REAM structure.
    public static List<string> GetFields()
    {
        return new List<string>
        {
            "Numbers",
            "Additive Field",
            "Multiplicative Field",
            "Patterns",
            "Geometry",
            "Measurement",
            "Data"
        };
    }

    // Returns all grade levels supported by the engine.
    public static List<string> GetGrades()
    {
        return new List<string>
        {
            "1st Grade",
            "2nd Grade",
            "3rd Grade",
            "4th Grade",
            "5th Grade",
            "6th Grade"
        };
    }

    // Returns the four transversal mathematical skills.
    public static List<string> GetMathematicalSkills()
    {
        return new List<string>
        {
            "Represent",
            "Model",
            "Solve Problems",
            "Argue & Communicate"
        };
    }

    // Returns the difficulty levels used by
    // the question generation engine.
    public static List<string> GetDifficulties()
    {
        return new List<string>
        {
            "Easy",
            "Medium",
            "Hard"
        };
    }

    // Returns curriculum items containing the
    // relationships between field, content,
    // grade, OA, and assessment indicator.
    public static List<CurriculumItem> GetCurriculum()
    {
        List<CurriculumItem> curriculum =
            new List<CurriculumItem>();

        // Initial prototype:
        // Additional REAM curriculum items will be
        // incorporated without changing the engine.

        curriculum.Add(
            new CurriculumItem(
                "Patterns",
                "Numerical Patterns",
                "6th Grade",
                "OA09",
                "Numerical patterns and relationships.",
                new List<string>
                {
                    "Identify the rule in a numerical pattern",
                    "Complete a numerical sequence using a given rule"
                }
            )
        );

        curriculum.Add(
            new CurriculumItem(
                "Numbers",
                "Fractions",
                "4th Grade",
                "OA08",
                "Understand fractions with denominators " +
                "2, 3, 4, 5, 6, 8, 10 and 12.",
                new List<string>
                {
                    "Represent fractions concretely, pictorially and symbolically",
                    "Compare fractions with the same denominator"
                }
            )
        );

        curriculum.Add(
            new CurriculumItem(
                "Multiplicative Field",
                "Division",
                "4th Grade",
                "OA05",
                "Demonstrate understanding of division.",
                new List<string>
                {
                    "Represent division using equal sharing or repeated subtraction",
                    "Solve routine division problems in real contexts"
                }
            )
        );

        return curriculum;
    }
}