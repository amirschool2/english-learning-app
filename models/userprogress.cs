using System;
using System.ComponentModel.DataAnnotations;

namespace EnglishLearningApp.Models
{
    public class UserProgress
    {
        [Key]
        public int Id { get; set; }

        public int TotalWordsLearned { get; set; } = 0;

        public int WordsMastered { get; set; } = 0;

        public int DailyStreak { get; set; } = 0;

        public DateTime LastActivityDate { get; set; } = DateTime.Now;

        public int TotalHoursPracticed { get; set; } = 0;

        public int TotalQuizzesTaken { get; set; } = 0;

        public double AverageQuizScore { get; set; } = 0.0;

        public DateTime? LastResetDate { get; set; }
    }
}
