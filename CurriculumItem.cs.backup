using System.Collections.Generic;

// Represents one specific piece of mathematical
// learning inside the REAM curriculum structure.
public class CurriculumItem
{
    public string Field { get; }
    public string Content { get; }
    public string Grade { get; }

    public string OA { get; }
    public string OADescription { get; }

    public string LearningDetail { get; }

    public List<string> Indicators { get; }

    // Creates one curriculum item used by
    // the mathematical generation engine.
    public CurriculumItem(
        string field,
        string content,
        string grade,
        string oa,
        string oaDescription,
        string learningDetail,
        List<string> indicators)
    {
        Field = field;
        Content = content;
        Grade = grade;

        OA = oa;
        OADescription = oaDescription;

        LearningDetail = learningDetail;

        Indicators = indicators;
    }
}