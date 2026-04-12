namespace ExamApi.DTOs
{
    public class SubmitAnswerRequest
    {
        public int StudentId { get; set; }
        public int QuestionId { get; set; }
        public string AnswerGiven { get; set; }
    }
}