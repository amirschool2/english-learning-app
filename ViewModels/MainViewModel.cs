using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using EnglishLearningApp.Data;
using EnglishLearningApp.Models;
using EnglishLearningApp.Services;

namespace EnglishLearningApp.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly AppDbContext _context;
        private readonly ProgressService _progressService;

        private object? _currentView;
        public object? CurrentView
        {
            get => _currentView;
            set => SetProperty(ref _currentView, value);
        }

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

        public ICommand NavigateDashboardCommand { get; }
        public ICommand NavigateFlashcardsCommand { get; }
        public ICommand NavigateQuizCommand { get; }
        public ICommand NavigatePronunciationCommand { get; }
        public ICommand NavigateProgressCommand { get; }
        public ICommand ImportWordsCommand { get; }

        public MainViewModel()
        {
            _context = new AppDbContext();
            _progressService = new ProgressService(_context);

            NavigateDashboardCommand = new RelayCommand(_ => NavigateToDashboard());
            NavigateFlashcardsCommand = new RelayCommand(_ => NavigateToFlashcards());
            NavigateQuizCommand = new RelayCommand(_ => NavigateToQuiz());
            NavigatePronunciationCommand = new RelayCommand(_ => NavigateToPronunciation());
            NavigateProgressCommand = new RelayCommand(_ => NavigateToProgress());
            ImportWordsCommand = new RelayCommand(_ => ImportWords());

            _context.Database.EnsureCreated();
            UpdateStatistics();
            NavigateToDashboard();
        }

        private void NavigateToDashboard()
        {
            CurrentView = new DashboardViewModel(_context, _progressService);
        }

        private void NavigateToFlashcards()
        {
            CurrentView = new FlashcardViewModel(_context, _progressService);
        }

        private void NavigateToQuiz()
        {
            CurrentView = new QuizViewModel(_context, _progressService);
        }

        private void NavigateToPronunciation()
        {
            CurrentView = new PronunciationViewModel(_context, _progressService);
        }

        private void NavigateToProgress()
        {
            CurrentView = new ProgressViewModel(_context, _progressService);
        }

        private void ImportWords()
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Text files (*.txt)|*.txt|CSV files (*.csv)|*.csv"
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    var entries = ImportService.ImportFromText(dialog.FileName);
                    foreach (var entry in entries)
                    {
                        _context.Words.Add(entry);
                    }
                    _context.SaveChanges();
                    UpdateStatistics();
                    MessageBox.Show($"Successfully imported {entries.Count} words!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error importing: {ex.Message}");
                }
            }
        }

        public void UpdateStatistics()
        {
            var (total, mastered, due) = _progressService.GetStatistics();
            TotalWords = total;
            MasterWords = mastered;
            ReviewDue = due;
        }

        public void OnWindowClosing()
        {
            _context?.Dispose();
            PronunciationService.Dispose();
        }
    }
}
