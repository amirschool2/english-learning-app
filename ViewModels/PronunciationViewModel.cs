using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using EnglishLearningApp.Data;
using EnglishLearningApp.Models;
using EnglishLearningApp.Services;

namespace EnglishLearningApp.ViewModels
{
    public class PronunciationViewModel : BaseViewModel
    {
        private readonly AppDbContext _context;
        private readonly ProgressService _progressService;

        private ObservableCollection<WordEntry> _words = new();
        public ObservableCollection<WordEntry> Words
        {
            get => _words;
            set => SetProperty(ref _words, value);
        }

        private WordEntry? _selectedWord;
        public WordEntry? SelectedWord
        {
            get => _selectedWord;
            set => SetProperty(ref _selectedWord, value);
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        private string _statusMessage = "Select a word to practice pronunciation";
        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        public ICommand PlayPronunciationCommand { get; }
        public ICommand EnrichWordCommand { get; }

        public PronunciationViewModel(AppDbContext context, ProgressService progressService)
        {
            _context = context;
            _progressService = progressService;

            PlayPronunciationCommand = new RelayCommand(_ => PlayPronunciation());
            EnrichWordCommand = new RelayCommand(_ => EnrichWithDictionary());

            LoadWords();
        }

        private void LoadWords()
        {
            var words = _context.Words.ToList();
            Words = new ObservableCollection<WordEntry>(words);
        }

        private async void PlayPronunciation()
        {
            if (SelectedWord == null)
            {
                StatusMessage = "Please select a word first";
                return;
            }

            IsLoading = true;
            StatusMessage = "Loading pronunciation...";

            try
            {
                string? audioUrl = SelectedWord.PronunciationUrl;

                if (string.IsNullOrEmpty(audioUrl))
                {
                    audioUrl = await DictionaryApiService.GetPronunciationUrlAsync(SelectedWord.Text);
                    if (!string.IsNullOrEmpty(audioUrl))
                    {
                        SelectedWord.PronunciationUrl = audioUrl;
                        _context.SaveChanges();
                    }
                }

                if (!string.IsNullOrEmpty(audioUrl))
                {
                    await PronunciationService.PlayAudioAsync(audioUrl);
                    StatusMessage = $"Playing: {SelectedWord.Text}";
                }
                else
                {
                    StatusMessage = $"No pronunciation available for: {SelectedWord.Text}";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async void EnrichWithDictionary()
        {
            if (SelectedWord == null)
            {
                StatusMessage = "Please select a word first";
                return;
            }

            IsLoading = true;
            StatusMessage = "Fetching data from dictionary...";

            try
            {
                var definition = await DictionaryApiService.GetDefinitionAsync(SelectedWord.Text);
                var audioUrl = await DictionaryApiService.GetPronunciationUrlAsync(SelectedWord.Text);

                if (!string.IsNullOrEmpty(definition))
                    SelectedWord.Definition = definition;

                if (!string.IsNullOrEmpty(audioUrl))
                    SelectedWord.PronunciationUrl = audioUrl;

                _context.SaveChanges();
                StatusMessage = $"Data enriched for: {SelectedWord.Text}";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
