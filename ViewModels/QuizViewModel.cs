using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using EnglishLearningApp.Data;
using EnglishLearningApp.Models;
using EnglishLearningApp.Services;

namespace EnglishLearningApp.ViewModels
{
    public class QuizViewModel : BaseViewModel
    {
        private readonly AppDbContext _context;
        private readonly ProgressService _progressService;
        private List<WordEntry> _quizWords = new();
        private int _currentQuestionIndex = 0;
        private int _correctAnswers = 0;
        private DateTime _quizStartTime;

        private string _currentQuestion = "Select a quiz type to start";
        public string CurrentQuestion
        {
            get => _currentQuestion;
            set => SetProperty(ref _currentQuestion, value);
        }

        private ObservableCollection<string> _options = new();
        public ObservableCollection<string> Options
        {
            get => _options;
            set => SetProperty(ref _options, value);
        }

        private bool _quizActive;
        public bool QuizActive
        {
            get => _quizActive;
            set => SetProperty(ref _quizActive, value);
        }

        private string _progressText = "No quiz active";
        public string ProgressText
        {
            get => _progressText;
            set => SetProperty(ref _progressText, value);
        }

        private int _score;
        public int Score
        {
            get => _score;
            set => SetProperty(ref _score, value);
        }

        public ICommand StartMultipleChoiceCommand { get; }
        public ICommand SubmitAnswerCommand { get; }
        public ICommand CancelCommand { get; }

        public QuizViewModel(AppDbContext context, ProgressService progressService)
        {
            _context = context;
            _progressService = progressService;

            StartMultipleChoiceCommand = new RelayCommand(_ => StartMultipleChoiceQuiz());
            SubmitAnswerCommand = new RelayCommand(o => SubmitAnswer(o));
            CancelCommand = new RelayCommand(_ => CancelQuiz());
        }

        private void StartMultipleChoiceQuiz()
        {
            _quizWords = _context.Words.OrderBy(w => w.MasteryLevel).Take(10).ToList();

            if (_quizWords.Count == 0)
            {
                CurrentQuestion = "No words available for quiz. Please import some words first.";
                return;
            }

            _currentQuestionIndex = 0;
            _correctAnswers = 0;
            _quizStartTime = DateTime.Now;
            QuizActive = true;
            Score = 0;

            LoadQuestion();
        }

        private void LoadQuestion()
        {
            if (_currentQuestionIndex >= _quizWords.Count)
            {
                EndQuiz();
                return;
            }

            var word = _quizWords[_currentQuestionIndex];
            CurrentQuestion = $"Question {_currentQuestionIndex + 1}/{_quizWords.Count}\n\nWhat is the definition of: {word.Text}?";
            ProgressText = $"Progress: {_currentQuestionIndex + 1}/{_quizWords.Count}";

            // Generate options
            Options.Clear();
            var allDefinitions = _context.Words.Select(w => w.Definition).ToList();
            var correctDefinition = word.Definition;

            var options = new List<string> { correctDefinition };

            // Add 3 random wrong options
            var wrongOptions = allDefinitions
                .Where(d => d != correctDefinition)
                .OrderBy(x => Guid.NewGuid())
                .Take(3)
                .ToList();

            options.AddRange(wrongOptions);
            options = options.OrderBy(x => Guid.NewGuid()).ToList();

            foreach (var option in options)
                Options.Add(option);
        }

        private void SubmitAnswer(object? selectedOption)
        {
            if (selectedOption == null || _currentQuestionIndex >= _quizWords.Count)
                return;

            var word = _quizWords[_currentQuestionIndex];
            var isCorrect = selectedOption.ToString() == word.Definition;

            if (isCorrect)
            {
                _correctAnswers++;
                Score = (_correctAnswers * 100) / _quizWords.Count;
            }

            _currentQuestionIndex++;
            LoadQuestion();
        }

        private void EndQuiz()
        {
            QuizActive = false;
            var duration = (int)(DateTime.Now - _quizStartTime).TotalSeconds;
            _progressService.RecordQuizResult(_correctAnswers, _quizWords.Count, duration, "Multiple Choice");

            CurrentQuestion = $"Quiz Complete!\n\nYour Score: {_correctAnswers}/{_quizWords.Count}\n\nPercentage: {Score}%";
            ProgressText = "Quiz finished!";
        }

        private void CancelQuiz()
        {
            QuizActive = false;
            CurrentQuestion = "Quiz cancelled";
            ProgressText = "No quiz active";
        }
    }
}
