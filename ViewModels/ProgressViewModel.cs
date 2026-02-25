using System;
using System.Collections.ObjectModel;
using System.Linq;
using EnglishLearningApp.Data;
using EnglishLearningApp.Services;

namespace EnglishLearningApp.ViewModels
{
    public class ProgressViewModel : BaseViewModel
    {
        private readonly AppDbContext _context;
        private readonly ProgressService _progressService;

        private int _totalWords;
        public int TotalWords
        {
            get => _totalWords;
            set => SetProperty(ref _totalWords, value);
        }

        private int _masterWords;
        public int MasterWords
        {
            get => _masterWords;
            set => SetProperty(ref _masterWords, value);
        }

        private int _reviewDue;
        public int ReviewDue
        {
            get => _reviewDue;
            set => SetProperty(ref _reviewDue, value);
        }

        private int _dailyStreak;
        public int DailyStreak
        {
            get => _dailyStreak;
            set => SetProperty(ref _dailyStreak, value);
        }

        private int _totalQuizzes;
        public int TotalQuizzes
        {
            get => _totalQuizzes;
            set => SetProperty(ref _totalQuizzes, value);
        }

        private double _averageScore;
        public double AverageScore
        {
            get => _averageScore;
            set => SetProperty(ref _averageScore, value);
        }

        private ObservableCollection<RecentQuizResult> _recentQuizzes = new();
        public ObservableCollection<RecentQuizResult> RecentQuizzes
        {
            get => _recentQuizzes;
            set => SetProperty(ref _recentQuizzes, value);
        }

        public ProgressViewModel(AppDbContext context, ProgressService progressService)
        {
            _context = context;
            _progressService = progressService;
            LoadProgress();
        }

        private void LoadProgress()
        {
            var (total, mastered, due) = _progressService.GetStatistics();
            TotalWords = total;
            MasterWords = mastered;
            ReviewDue = due;

            var progress = _context.UserProgress.FirstOrDefault();
            if (progress != null)
            {
                DailyStreak = progress.DailyStreak;
                TotalQuizzes = progress.TotalQuizzesTaken;
                AverageScore = Math.Round(progress.AverageQuizScore, 1);
            }

            LoadRecentQuizzes();
        }

        private void LoadRecentQuizzes()
        {
            var quizzes = _context.QuizResults
                .OrderByDescending(q => q.QuizDate)
                .Take(10)
                .ToList();

            RecentQuizzes.Clear();
            foreach (var quiz in quizzes)
            {
                var percentage = (quiz.Score * 100) / quiz.TotalQuestions;
                RecentQuizzes.Add(new RecentQuizResult
                {
                    Date = quiz.QuizDate.ToString("g"),
                    Score = $"{quiz.Score}/{quiz.TotalQuestions}",
                    Percentage = percentage,
                    Type = quiz.QuizType ?? "Quiz"
                });
            }
        }
    }

    public class RecentQuizResult
    {
        public string Date { get; set; } = "";
        public string Score { get; set; } = "";
        public int Percentage { get; set; }
        public string Type { get; set; } = "";
    }
}
