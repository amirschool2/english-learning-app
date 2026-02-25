using System;
using System.Linq;
using EnglishLearningApp.Data;
using EnglishLearningApp.Models;

namespace EnglishLearningApp.Services
{
    public class ProgressService
    {
        private readonly AppDbContext _context;

        public ProgressService(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Update user progress after a learning session
        /// </summary>
        public void UpdateProgressAfterReview(int wordId, bool isCorrect)
        {
            var word = _context.Words.FirstOrDefault(w => w.Id == wordId);
            if (word == null)
                return;

            word.TimesReviewed++;
            if (isCorrect)
                word.TimesCorrect++;

            // Calculate mastery level (0-100)
            if (word.TimesReviewed > 0)
            {
                word.MasteryLevel = (int)((double)word.TimesCorrect / word.TimesReviewed * 100);
            }

            word.LastReviewed = DateTime.Now;

            // Spaced repetition: calculate next review date
            int daysUntilNextReview = word.MasteryLevel switch
            {
                >= 80 => 14, // Mastered - review in 2 weeks
                >= 60 => 7,  // Good - review in 1 week
                >= 40 => 3,  // Fair - review in 3 days
                _ => 1       // Poor - review tomorrow
            };

            word.NextReviewDate = DateTime.Now.AddDays(daysUntilNextReview);

            _context.SaveChanges();
            UpdateUserProgress();
        }

        /// <summary>
        /// Update overall user progress statistics
        /// </summary>
        public void UpdateUserProgress()
        {
            var progress = _context.UserProgress.FirstOrDefault() ?? new UserProgress();

            var words = _context.Words.ToList();
            progress.TotalWordsLearned = words.Count;
            progress.WordsMastered = words.Count(w => w.MasteryLevel >= 80);

            // Update daily streak
            var lastActivity = progress.LastActivityDate;
            var today = DateTime.Now.Date;

            if (lastActivity.Date == today)
            {
                // Already active today
            }
            else if (lastActivity.Date == today.AddDays(-1))
            {
                // Streak continues
                progress.DailyStreak++;
            }
            else
            {
                // Streak broken
                progress.DailyStreak = 1;
            }

            progress.LastActivityDate = DateTime.Now;

            _context.UserProgress.Update(progress);
            _context.SaveChanges();
        }

        /// <summary>
        /// Record quiz result
        /// </summary>
        public void RecordQuizResult(int score, int totalQuestions, int durationSeconds, string quizType)
        {
            var result = new QuizResult
            {
                Score = score,
                TotalQuestions = totalQuestions,
                DurationSeconds = durationSeconds,
                QuizType = quizType,
                QuizDate = DateTime.Now
            };

            _context.QuizResults.Add(result);

            // Update user progress
            var progress = _context.UserProgress.FirstOrDefault();
            if (progress != null)
            {
                progress.TotalQuizzesTaken++;
                var allResults = _context.QuizResults.ToList();
                progress.AverageQuizScore = allResults.Average(r => (double)r.Score / r.TotalQuestions * 100);
            }

            _context.SaveChanges();
            UpdateUserProgress();
        }

        /// <summary>
        /// Get learning statistics
        /// </summary>
        public (int totalWords, int mastered, int dueForReview) GetStatistics()
        {
            var words = _context.Words.ToList();
            var totalWords = words.Count;
            var mastered = words.Count(w => w.MasteryLevel >= 80);
            var dueForReview = words.Count(w => w.NextReviewDate == null || w.NextReviewDate <= DateTime.Now);

            return (totalWords, mastered, dueForReview);
        }
    }
}
