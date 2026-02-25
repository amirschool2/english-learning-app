using System;
using System.ComponentModel.DataAnnotations;

namespace EnglishLearningApp.Models
{
    public class QuizResult
    {
        [Key]
        public int Id { get; set; }

        public DateTime QuizDate { get; set; } = DateTime.Now;

        public int Score { get; set; }

        public int TotalQuestions { get; set; }

        public string? AnswersJson { get; set; } // Stores quiz answers as JSON

        public int DurationSeconds { get; set; }

        public string? QuizType { get; set; } // "Multiple Choice", "FillInBlank", etc.
    }
}
