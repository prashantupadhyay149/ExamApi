namespace ExamApi.DTOs
{
    public class AddQuestionRequest
    {
        public int TestId { get; set; }
        public string Text { get; set; }
        public string Type { get; set; }
        public string OptionsJson { get; set; }
        public string CorrectAnswer { get; set; }
    }
}