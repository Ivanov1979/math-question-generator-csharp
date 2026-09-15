using System.Collections.Generic;

// Filters curriculum information according to
// the selections made by the teacher.
public class CurriculumManager
{
    private readonly List<CurriculumItem> curriculum;

    // Loads the curriculum when the manager is created.
    public CurriculumManager()
    {
        curriculum = CurriculumData.GetCurriculum();
    }

    // Returns the mathematical fields.
    public List<string> GetFields()
    {
        return CurriculumData.GetFields();
    }

    // Returns all grades supported by the engine.
    public List<string> GetGrades()
    {
        return CurriculumData.GetGrades();
    }

    // Returns the contents available for a field.
    public List<string> GetContents(string field)
    {
        List<string> contents = new List<string>();

        foreach (CurriculumItem item in curriculum)
        {
            if (item.Field == field &&
                !contents.Contains(item.Content))
            {
                contents.Add(item.Content);
            }
        }

        return contents;
    }

    // Returns grades available for a specific
    // mathematical field and content.
    public List<string> GetGradesForContent(
        string field,
        string content)
    {
        List<string> grades = new List<string>();

        foreach (CurriculumItem item in curriculum)
        {
            if (item.Field == field &&
                item.Content == content &&
                !grades.Contains(item.Grade))
            {
                grades.Add(item.Grade);
            }
        }

        return grades;
    }

    // Returns the OAs available for the selected
    // field, content, and grade.
    public List<string> GetOAs(
        string field,
        string content,
        string grade)
    {
        List<string> oas = new List<string>();

        foreach (CurriculumItem item in curriculum)
        {
            if (item.Field == field &&
                item.Content == content &&
                item.Grade == grade &&
                !oas.Contains(item.OA))
            {
                oas.Add(item.OA);
            }
        }

        return oas;
    }

    // Returns assessment indicators for the
    // selected curriculum combination.
    public List<string> GetIndicators(
        string field,
        string content,
        string grade,
        string oa)
    {
        List<string> indicators = new List<string>();

        foreach (CurriculumItem item in curriculum)
        {
            if (item.Field == field &&
                item.Content == content &&
                item.Grade == grade &&
                item.OA == oa)
            {
                foreach (string indicator in item.Indicators)
                {
                    if (!indicators.Contains(indicator))
                    {
                        indicators.Add(indicator);
                    }
                }
            }
        }

        return indicators;
    }

    // Returns the mathematical skills used
    // by the generation engine.
    public List<string> GetMathematicalSkills()
    {
        return CurriculumData.GetMathematicalSkills();
    }

    // Returns available difficulty levels.
    public List<string> GetDifficulties()
    {
        return CurriculumData.GetDifficulties();
    }
}