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
            "Patterns and Algebra",
            "Geometry",
            "Measurement",
            "Data and Probability"
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

    // Returns the difficulty levels used by the engine.
    public static List<string> GetDifficulties()
    {
        return new List<string>
        {
            "Easy",
            "Medium",
            "Hard"
        };
    }

    // Returns curriculum items containing the relationship:
    //
    // Field → Content → Grade → OA
    // → Learning Detail → Assessment Indicator
    //
    // This list will be expanded using the REAM curriculum.
    public static List<CurriculumItem> GetCurriculum()
    {
        List<CurriculumItem> curriculum =
            new List<CurriculumItem>();


        // ==========================================
        // PATTERNS AND ALGEBRA
        // 6TH GRADE - OA09
        // ==========================================

        curriculum.Add(
            new CurriculumItem(
                "Patterns and Algebra",
                "Numerical Patterns",
                "6th Grade",
                "OA09",
                "Demonstrate understanding of relationships " +
                "in numerical patterns.",
                "Identify rules in numerical patterns",
                new List<string>
                {
                    "Identify the rule in a numerical pattern"
                }
            )
        );

        curriculum.Add(
            new CurriculumItem(
                "Patterns and Algebra",
                "Numerical Patterns",
                "6th Grade",
                "OA09",
                "Demonstrate understanding of relationships " +
                "in numerical patterns.",
                "Complete numerical sequences",
                new List<string>
                {
                    "Complete a numerical sequence using a given rule"
                }
            )
        );


        // ==========================================
        // NUMBERS
        // 4TH GRADE - OA08
        // ==========================================

        curriculum.Add(
            new CurriculumItem(
                "Numbers",
                "Fractions",
                "4th Grade",
                "OA08",
                "Demonstrate understanding of fractions " +
                "with denominators 2, 3, 4, 5, 6, 8, 10 and 12.",
                "Represent fractions",
                new List<string>
                {
                    "Represent fractions concretely, " +
                    "pictorially and symbolically"
                }
            )
        );

        curriculum.Add(
            new CurriculumItem(
                "Numbers",
                "Fractions",
                "4th Grade",
                "OA08",
                "Demonstrate understanding of fractions " +
                "with denominators 2, 3, 4, 5, 6, 8, 10 and 12.",
                "Compare fractions with the same denominator",
                new List<string>
                {
                    "Compare fractions with the same denominator"
                }
            )
        );


        // ==========================================
        // MULTIPLICATIVE FIELD
        // 4TH GRADE - OA05
        // ==========================================

        curriculum.Add(
            new CurriculumItem(
                "Multiplicative Field",
                "Division",
                "4th Grade",
                "OA05",
                "Demonstrate understanding of division.",
                "Represent division",
                new List<string>
                {
                    "Represent division using equal sharing " +
                    "or repeated subtraction"
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
                "Division in problem situations",
                new List<string>
                {
                    "Solve routine division problems " +
                    "in real contexts"
                }
            )
        );


        return curriculum;
    }
}