using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using EnglishLearningApp.Data;
using EnglishLearningApp.Models;
using EnglishLearningApp.Services;

namespace EnglishLearningApp.ViewModels
{
    public class FlashcardViewModel : BaseViewModel
    {
        private readonly AppDbContext _context;
        private readonly ProgressService _progressService;

        private ObservableCollection<WordEntry> _cards = new();
        public ObservableCollection<WordEntry> Cards
        {
            get => _cards;
            set => SetProperty(ref _cards, value);
        }

        private WordEntry? _currentCard;
        public WordEntry? CurrentCard
        {
            get => _currentCard;
            set => SetProperty(ref _currentCard, value);
        }

        private bool _showDefinition;
        public bool ShowDefinition
        {
            get => _showDefinition;
            set => SetProperty(ref _showDefinition, value);
        }

        private int _currentIndex;
        public int CurrentIndex
        {
            get => _currentIndex;
            set => SetProperty(ref _currentIndex, value);
        }

        public ICommand NextCommand { get; }
        public ICommand PreviousCommand { get; }
        public ICommand ToggleDefinitionCommand { get; }
        public ICommand MarkCorrectCommand { get; }
        public ICommand MarkIncorrectCommand { get; }

        public FlashcardViewModel(AppDbContext context, ProgressService progressService)
        {
            _context = context;
            _progressService = progressService;

            NextCommand = new RelayCommand(_ => Next());
            PreviousCommand = new RelayCommand(_ => Previous());
            ToggleDefinitionCommand = new RelayCommand(_ => ToggleDefinition());
            MarkCorrectCommand = new RelayCommand(_ => MarkCorrect());
            MarkIncorrectCommand = new RelayCommand(_ => MarkIncorrect());

            LoadCards();
        }

        private void LoadCards()
        {
            var words = _context.Words.ToList();
            Cards = new ObservableCollection<WordEntry>(words);

            if (Cards.Count > 0)
                CurrentCard = Cards[0];

            CurrentIndex = 0;
        }

        private void ToggleDefinition()
        {
            ShowDefinition = !ShowDefinition;
        }

        private void Next()
        {
            ShowDefinition = false;
            if (CurrentIndex < Cards.Count - 1)
            {
                CurrentIndex++;
                CurrentCard = Cards[CurrentIndex];
            }
        }

        private void Previous()
        {
            ShowDefinition = false;
            if (CurrentIndex > 0)
            {
                CurrentIndex--;
                CurrentCard = Cards[CurrentIndex];
            }
        }

        private void MarkCorrect()
        {
            if (CurrentCard != null)
            {
                _progressService.UpdateProgressAfterReview(CurrentCard.Id, true);
                Next();
            }
        }

        private void MarkIncorrect()
        {
            if (CurrentCard != null)
            {
                _progressService.UpdateProgressAfterReview(CurrentCard.Id, false);
                Next();
            }
        }
    }
}
