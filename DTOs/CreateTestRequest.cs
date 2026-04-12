using System.Collections.Generic;

namespace ExamApi.DTOs
{
    public class CreateTestRequest
    {
        public string Title { get; set; }
        public int TeacherId { get; set; }   // ✅ moved from query to body
        public List<QuestionDto> Questions { get; set; }
    }

    public class QuestionDto
    {
        public string Text { get; set; }
        public string Type { get; set; }
        public string OptionsJson { get; set; }
        public string CorrectAnswer { get; set; }
    }
}