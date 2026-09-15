using System.Collections.Generic;

// Represents one curriculum learning objective and
// the assessment indicators associated with it.
public class CurriculumItem
{
    public string Field { get; }
    public string Content { get; }
    public string Grade { get; }
    public string OA { get; }
    public string Description { get; }
    public List<string> Indicators { get; }

    // Creates a curriculum item.
    public CurriculumItem(
        string field,
        string content,
        string grade,
        string oa,
        string description,
        List<string> indicators)
    {
        Field = field;
        Content = content;
        Grade = grade;
        OA = oa;
        Description = description;
        Indicators = indicators;
    }
}