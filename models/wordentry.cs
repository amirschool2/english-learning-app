using System;
using System.ComponentModel.DataAnnotations;

namespace EnglishLearningApp.Models
{
    public class WordEntry
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Text { get; set; } = string.Empty; // Word or phrase

        [Required]
        public string Definition { get; set; } = string.Empty;

        public string? Example { get; set; }

        public string? PronunciationUrl { get; set; }

        public string? Category { get; set; }

        public DateTime DateAdded { get; set; } = DateTime.Now;

        // Learning stats
        public int TimesReviewed { get; set; } = 0;

        public int TimesCorrect { get; set; } = 0;

        public DateTime? LastReviewed { get; set; }

        public DateTime? NextReviewDate { get; set; }

        public int MasteryLevel { get; set; } = 0; // 0-100
    }
}
