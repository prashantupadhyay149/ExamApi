using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExamApi.Data;
using ExamApi.DTOs;

namespace ExamApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentController : ControllerBase
    {
        private readonly AppDbContext _context;

        public StudentController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("tests")]
        public async Task<IActionResult> GetAllTests()
        {
            var tests = await _context.Tests
                .Include(t => t.Teacher)
                .Select(t => new { t.Id, t.Title, TeacherName = t.Teacher.FullName })
                .ToListAsync();
            return Ok(tests);
        }

        [HttpGet("test-questions/{testId}")]
        public async Task<IActionResult> GetTestQuestions(int testId)
        {
            var questions = await _context.Questions
                .Where(q => q.TestId == testId)
                .Select(q => new { q.Id, q.Text, q.Type, q.OptionsJson })
                .ToListAsync();
            return Ok(questions);
        }

        [HttpPost("submit-answer")]
        public async Task<IActionResult> SubmitAnswer(SubmitAnswerRequest request)
        {
            var question = await _context.Questions.FindAsync(request.QuestionId);
            if (question == null) return NotFound();

            bool isCorrect = (question.Type == "text")
                ? question.CorrectAnswer.Equals(request.AnswerGiven, StringComparison.OrdinalIgnoreCase)
                : question.CorrectAnswer == request.AnswerGiven;

            var submission = new Models.Submission
            {
                StudentId = request.StudentId,
                QuestionId = request.QuestionId,
                AnswerGiven = request.AnswerGiven,
                IsCorrect = isCorrect,
                SubmittedAt = DateTime.UtcNow
            };
            _context.Submissions.Add(submission);
            await _context.SaveChangesAsync();

            return Ok(new { isCorrect });
        }

        [HttpPost("complete-test")]
        public async Task<IActionResult> CompleteTest(int studentId, int testId)
        {
            var submissions = await _context.Submissions
                .Where(s => s.StudentId == studentId && s.Question.TestId == testId)
                .ToListAsync();

            if (!submissions.Any()) return BadRequest("No answers submitted");

            int total = await _context.Questions.CountAsync(q => q.TestId == testId);
            int score = submissions.Count(s => s.IsCorrect);

            var result = new Models.Result
            {
                StudentId = studentId,
                TestId = testId,
                Score = score,
                TotalQuestions = total,
                SubmittedAt = DateTime.UtcNow
            };
            _context.Results.Add(result);
            await _context.SaveChangesAsync();

            return Ok(new { score, total });
        }
    }
}