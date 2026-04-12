using System;

namespace ExamApi.Models
{
    public class Result
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int TestId { get; set; }
        public int Score { get; set; }
        public int TotalQuestions { get; set; }
        public DateTime SubmittedAt { get; set; }
        public User Student { get; set; }
        public Test Test { get; set; }
    }
}