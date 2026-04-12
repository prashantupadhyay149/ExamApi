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
        public async Task<IActionResult> CreateTest([FromBody] CreateTestRequest request)
        {
            // ✅ Validation
            if (request == null)
                return BadRequest("Request is null");

            if (string.IsNullOrEmpty(request.Title))
                return BadRequest("Title is required");

            if (request.Questions == null || !request.Questions.Any())
                return BadRequest("At least one question is required");

            // ✅ Check teacher
            var teacher = await _context.Users.FindAsync(request.TeacherId);
            if (teacher == null || teacher.Role != "teacher")
                return Unauthorized("Invalid teacher");

            // ✅ Create Test
            var test = new Test
            {
                Title = request.Title,
                TeacherId = request.TeacherId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Tests.Add(test);
            await _context.SaveChangesAsync(); // 🔥 required to get test.Id

            // ✅ Add Questions
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

            await _context.SaveChangesAsync(); // 🔥 saves questions

            return Ok(new
            {
                message = "Test and Questions created successfully",
                testId = test.Id
            });
        }

        // Other APIs (same as before)
    }
}