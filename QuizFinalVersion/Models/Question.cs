using System;
using System.Collections.Generic;

namespace QuizFinalVersion.Models
{
    public class Question
    {
        public string QuestionName { get; set; }
        public string AnswerA { get; set; }
        public string AnswerB { get; set; }
        public string AnswerC { get; set; }
        public string AnswerD { get; set; }
        public List<string> CorrectAnswers { get; set; }

        public override string ToString()
        {
            return
                $"name: {QuestionName}, A: {AnswerA}, B: {AnswerB}, C: {AnswerC}, D: {AnswerD}, correct: {string.Join(", ", CorrectAnswers)}";
        }
    }
}