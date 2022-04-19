using System.Collections.Generic;

namespace QuizFinalVersion.Models
{
    public class Quiz
    {
        public string Name { get; set; }
        public List<Question> Questions { get; set; }
    }
}