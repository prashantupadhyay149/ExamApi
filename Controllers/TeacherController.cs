using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExamApi.Data;
using ExamApi.DTOs;
using ExamApi.Models;

namespace ExamApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TeacherController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TeacherController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("create-test")]
        public async Task<IActionResult> CreateTest([FromQuery] int teacherId, [FromBody] CreateTestRequest request)
        {
            var teacher = await _context.Users.FindAsync(teacherId);
            if (teacher == null || teacher.Role != "teacher")
                return Unauthorized();

            var test = new Test
            {
                Title = request.Title,
                TeacherId = teacherId,
                CreatedAt = DateTime.UtcNow
            };
            _context.Tests.Add(test);
            await _context.SaveChangesAsync();

            foreach (var q in request.Questions)
            {
                var question = new Question
                {
                    TestId = test.Id,
                    Text = q.Text,
                    Type = q.Type,
                    OptionsJson = q.OptionsJson,
                    CorrectAnswer = q.CorrectAnswer
                };
                _context.Questions.Add(question);
            }
            await _context.SaveChangesAsync();

            return Ok(new { testId = test.Id, message = "Test created successfully" });
        }

        [HttpGet("students")]
        public async Task<IActionResult> GetAllStudents()
        {
            var students = await _context.Users
                .Where(u => u.Role == "student")
                .Select(u => new { u.Id, u.FullName, u.Email })
                .ToListAsync();
            return Ok(students);
        }

        [HttpGet("student-results/{studentId}")]
        public async Task<IActionResult> GetStudentResults(int studentId)
        {
            var results = await _context.Results
                .Include(r => r.Test)
                .Where(r => r.StudentId == studentId)
                .Select(r => new { r.Test.Title, r.Score, r.TotalQuestions, r.SubmittedAt })
                .ToListAsync();
            return Ok(results);
        }

        [HttpGet("test-results/{testId}")]
        public async Task<IActionResult> GetTestResults(int testId)
        {
            var results = await _context.Results
                .Include(r => r.Student)
                .Where(r => r.TestId == testId)
                .Select(r => new { r.Student.FullName, r.Score, r.TotalQuestions, r.SubmittedAt })
                .ToListAsync();
            return Ok(results);
        }
    }
}