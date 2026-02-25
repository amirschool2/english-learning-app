using System;
using System.Linq;
using System.Windows.Input;
using EnglishLearningApp.Data;
using EnglishLearningApp.Services;

namespace EnglishLearningApp.ViewModels
{
    public class DashboardViewModel : BaseViewModel
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

        private double _averageQuizScore;
        public double AverageQuizScore
        {
            get => _averageQuizScore;
            set => SetProperty(ref _averageQuizScore, value);
        }

        public DashboardViewModel(AppDbContext context, ProgressService progressService)
        {
            _context = context;
            _progressService = progressService;
            LoadStatistics();
        }

        private void LoadStatistics()
        {
            var (total, mastered, due) = _progressService.GetStatistics();
            TotalWords = total;
            MasterWords = mastered;
            ReviewDue = due;

            var progress = _context.UserProgress.FirstOrDefault();
            if (progress != null)
            {
                DailyStreak = progress.DailyStreak;
                AverageQuizScore = Math.Round(progress.AverageQuizScore, 1);
            }
        }
    }
}
