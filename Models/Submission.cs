using System;

namespace ExamApi.Models
{
    public class Submission
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int QuestionId { get; set; }
        public string AnswerGiven { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
        public DateTime SubmittedAt { get; set; }
        public User Student { get; set; } = null!;
        public Question Question { get; set; } = null!;
    }
}