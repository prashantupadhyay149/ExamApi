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

        [HttpPost("create-test-only")]
        public async Task<IActionResult> CreateTestOnly([FromQuery] int teacherId, [FromBody] CreateTestOnlyRequest request)
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

            return Ok(new { testId = test.Id, message = "Test created successfully" });
        }

        [HttpPost("add-question")]
        public async Task<IActionResult> AddQuestion([FromBody] AddQuestionRequest request)
        {
            try
            {
                var test = await _context.Tests.FindAsync(request.TestId);
                if (test == null)
                    return NotFound(new { message = "Test not found" });

                var question = new Question
                {
                    TestId = request.TestId,
                    Text = request.Text,
                    Type = request.Type,
                    OptionsJson = request.OptionsJson,
                    CorrectAnswer = request.CorrectAnswer
                };
                _context.Questions.Add(question);
                await _context.SaveChangesAsync();

                // IMPORTANT: Use the saved Id only, not the whole entity
                return Ok(new { questionId = question.Id, message = "Question added successfully" });
            }
            catch (Exception ex)
            {
                // Log the error (you can inject ILogger)
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, new { error = ex.Message });
            }
        }
        [HttpGet("my-tests")]
        public async Task<IActionResult> GetMyTests([FromQuery] int teacherId)
        {
            var tests = await _context.Tests
                .Where(t => t.TeacherId == teacherId)
                .Select(t => new { t.Id, t.Title, t.CreatedAt, QuestionCount = t.Questions.Count })
                .ToListAsync();
            return Ok(tests);
        }

        // Other APIs (same as before)
    }
}