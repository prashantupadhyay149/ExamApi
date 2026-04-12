using ExamApi.Models;

public class Question
{
    public int Id { get; set; }
    public int TestId { get; set; }
    public string Text { get; set; }
    public string Type { get; set; }
    public string OptionsJson { get; set; }
    public string CorrectAnswer { get; set; }
    public Test Test { get; set; }
    public ICollection<Submission> Submissions { get; set; } // ✅ Add this
}