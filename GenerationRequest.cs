
using System.Text.Json.Serialization;

// Stores all teacher selections required by the
// mathematical question generation engine.
public struct GenerationRequest
{
    public string Field { get; }
    public string Content { get; }
    public string Grade { get; }

    public string OA { get; }
    public string LearningDetail { get; }
    public string AssessmentIndicator { get; }

    public string MathematicalSkill { get; }
    public string Difficulty { get; }
    public string ExerciseType { get; }

    // Allows ASP.NET Core to deserialize JSON
    // using the complete constructor.
    [JsonConstructor]
    public GenerationRequest(
        string field,
        string content,
        string grade,
        string oa,
        string learningDetail,
        string assessmentIndicator,
        string mathematicalSkill,
        string difficulty,
        string exerciseType)
    {
        Field = field;
        Content = content;
        Grade = grade;

        OA = oa;
        LearningDetail = learningDetail;
        AssessmentIndicator = assessmentIndicator;

        MathematicalSkill = mathematicalSkill;
        Difficulty = difficulty;
        ExerciseType = exerciseType;
    }
}
