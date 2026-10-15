using System;

public class Activity
{
    private string course;
    private string oa;
    private string indicator;
    private string competency;
    private string difficulty;
    private string question;
    private string answer;


    // Public properties used by the web API for JSON serialization.
    public string Course => course;
    public string OA => oa;
    public string Indicator => indicator;
    public string Competency => competency;
    public string Difficulty => difficulty;
    public string Question => question;
    public string Answer => answer;


    // Constructor: initializes all activity information.
    public Activity(
        string course,
        string oa,
        string indicator,
        string competency,
        string difficulty,
        string question,
        string answer)
    {
        this.course = course;
        this.oa = oa;
        this.indicator = indicator;
        this.competency = competency;
        this.difficulty = difficulty;
        this.question = question;
        this.answer = answer;
    }

    // Returns the course.
    public string GetCourse()
    {
        return course;
    }

    // Returns the Learning Objective (OA).
    public string GetOA()
    {
        return oa;
    }

    // Returns the assessment indicator.
    public string GetIndicator()
    {
        return indicator;
    }

    // Returns the mathematical competency.
    public string GetCompetency()
    {
        return competency;
    }

    // Returns the difficulty level.
    public string GetDifficulty()
    {
        return difficulty;
    }

    // Returns the activity question.
    public string GetQuestion()
    {
        return question;
    }

    // Returns the correct answer.
    public string GetAnswer()
    {
        return answer;
    }

    // Displays the activity information.
    public void Display()
    {
        Console.WriteLine();
        Console.WriteLine("==========================================");
        Console.WriteLine("Activity");
        Console.WriteLine("==========================================");

        Console.WriteLine("Course: " + course);
        Console.WriteLine("OA: " + oa);
        Console.WriteLine("Assessment Indicator: " + indicator);
        Console.WriteLine("Competency: " + competency);
        Console.WriteLine("Difficulty: " + difficulty);
        Console.WriteLine("Question: " + question);
    }

    // Checks whether the student's answer matches the correct answer.
    public bool CheckAnswer(string studentAnswer)
    {
        return studentAnswer == answer;
    }
}