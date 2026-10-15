using System.Collections.Generic;

// Manages and filters curriculum information
// according to the teacher's selections.
public class CurriculumManager
{
    private readonly List<CurriculumItem> curriculum;

    // Loads curriculum records from MySQL when the manager is created.
    public CurriculumManager()
    {
        curriculum = CurriculumRepository.GetCurriculum();
    }

    // ==========================================
    // EXISTING CURRICULUM METHODS
    // ==========================================

    // Returns all mathematical fields.
    public List<string> GetFields()
    {
        return CurriculumData.GetFields();
    }

    // Returns all grades supported by the engine.
    public List<string> GetGrades()
    {
        return CurriculumData.GetGrades();
    }

    // Returns contents belonging to a selected field.
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

    // Returns grades available for a selected
    // field and mathematical content.
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

    // Returns the OAs associated with the selected
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

    // Returns learning details associated
    // with the selected OA.
    public List<string> GetLearningDetails(
        string field,
        string content,
        string grade,
        string oa)
    {
        List<string> learningDetails =
            new List<string>();

        foreach (CurriculumItem item in curriculum)
        {
            if (item.Field == field &&
                item.Content == content &&
                item.Grade == grade &&
                item.OA == oa &&
                !learningDetails.Contains(
                    item.LearningDetail))
            {
                learningDetails.Add(
                    item.LearningDetail);
            }
        }

        return learningDetails;
    }

    // Returns assessment indicators associated
    // with the selected learning detail.
    public List<string> GetIndicators(
        string field,
        string content,
        string grade,
        string oa,
        string learningDetail)
    {
        List<string> indicators =
            new List<string>();

        foreach (CurriculumItem item in curriculum)
        {
            if (item.Field == field &&
                item.Content == content &&
                item.Grade == grade &&
                item.OA == oa &&
                item.LearningDetail == learningDetail)
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

    // Returns the four transversal
    // mathematical skills.
    public List<string> GetMathematicalSkills()
    {
        return CurriculumData.GetMathematicalSkills();
    }

    // Returns available difficulty levels.
    public List<string> GetDifficulties()
    {
        return CurriculumData.GetDifficulties();
    }


    // ==========================================
    // MODULE #3 - SIMPLIFIED CURRICULUM METHODS
    // ==========================================
    //
    // These methods support the future web-app
    // curriculum workflow:
    //
    // Content
    //    ↓
    // Grade
    //    ↓
    // Learning Objective
    //    ↓
    // Assessment Indicator
    //
    // Mathematical Field and Learning Detail remain
    // available internally in the curriculum database.


    // Returns every mathematical content available
    // in the curriculum database.
    public List<string> GetAllContents()
    {
        List<string> contents = new List<string>();

        foreach (CurriculumItem item in curriculum)
        {
            if (!contents.Contains(item.Content))
            {
                contents.Add(item.Content);
            }
        }

        return contents;
    }

    // Returns grades available for a selected content.
    //
    // This overload does not require the mathematical
    // field because the future web interface begins
    // directly with mathematical content.
    public List<string> GetGradesForContent(
        string content)
    {
        List<string> grades = new List<string>();

        foreach (CurriculumItem item in curriculum)
        {
            if (item.Content == content &&
                !grades.Contains(item.Grade))
            {
                grades.Add(item.Grade);
            }
        }

        return grades;
    }

    // Returns learning objectives associated with
    // a selected content and grade.
    //
    // In the Chilean curriculum, the learning
    // objective code corresponds to an OA.
    public List<string> GetLearningObjectives(
        string content,
        string grade)
    {
        List<string> objectives =
            new List<string>();

        foreach (CurriculumItem item in curriculum)
        {
            if (item.Content == content &&
                item.Grade == grade &&
                !objectives.Contains(item.OA))
            {
                objectives.Add(item.OA);
            }
        }

        return objectives;
    }

    // Returns assessment indicators associated
    // with a selected content, grade, and
    // learning objective.
    //
    // LearningDetail remains part of the internal
    // curriculum structure, but the teacher does
    // not need to select it separately in the
    // simplified web interface.
    public List<string> GetAssessmentIndicators(
        string content,
        string grade,
        string learningObjective)
    {
        List<string> indicators =
            new List<string>();

        foreach (CurriculumItem item in curriculum)
        {
            if (item.Content == content &&
                item.Grade == grade &&
                item.OA == learningObjective)
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
}