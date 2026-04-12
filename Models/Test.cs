using System;
using System.Collections.Generic;

namespace ExamApi.Models
{
    public class Test
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int TeacherId { get; set; }
        public DateTime CreatedAt { get; set; }
        public User Teacher { get; set; }
        public ICollection<Question> Questions { get; set; }
    }
}