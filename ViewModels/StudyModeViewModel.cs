using QuickMind.Models;
using QuickMind.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace QuickMind.ViewModels
{
    public class StudyModeViewModel : ViewModelBase
    {
        private readonly CardService _cardService;
        private readonly LocalizationService _localizationService;
        private List<FlashCard> _studyCards;
        private int _currentIndex;
        private FlashCard? _currentCard;
        private bool _showAnswer;
        private string _studyTopic;

        public StudyModeViewModel(CardService cardService, string? topic = null)
        {
            _cardService = cardService;
            _localizationService = LocalizationService.Instance;
            _studyTopic = topic ?? _localizationService.GetString("All");
            _studyCards = new List<FlashCard>();
            _currentIndex = 0;
            _showAnswer = false;

            _localizationService.LanguageChanged += OnLanguageChanged;

            InitializeCommands();
            _ = LoadStudyCardsAsync();
        }

        public FlashCard? CurrentCard
        {
            get => _currentCard;
            set => SetProperty(ref _currentCard, value);
        }

        public bool ShowAnswer
        {
            get => _showAnswer;
            set => SetProperty(ref _showAnswer, value);
        }

        public string StudyTopic
        {
            get => _studyTopic;
            set => SetProperty(ref _studyTopic, value);
        }

        public int CurrentIndex
        {
            get => _currentIndex;
            set => SetProperty(ref _currentIndex, value);
        }

        public int TotalCards => _studyCards.Count;

        public string Progress => $"{CurrentIndex + 1} / {TotalCards}";

        public bool HasCards => _studyCards.Any();

        public bool CanGoNext => CurrentIndex < TotalCards - 1;

        public bool CanGoPrevious => CurrentIndex > 0;

        public string StudyModeLabel => _localizationService.GetString("StudyMode");
        public string QuestionLabel => _localizationService.GetString("Question");
        public string AnswerLabel => _localizationService.GetString("Answer");
        public string ShowAnswerLabel => _localizationService.GetString("ShowAnswer");
        public string KnowLabel => _localizationService.GetString("Know");
        public string DontKnowLabel => _localizationService.GetString("DontKnow");
        public string NextLabel => _localizationService.GetString("Next");
        public string PreviousLabel => _localizationService.GetString("Previous");
        public string RestartLabel => _localizationService.GetString("Restart");
        public string NoCardsLabel => _localizationService.GetString("NoCardsToStudy");
        public string AddCardsLabel => _localizationService.GetString("AddCardsOrSelectTopic");
        public string ClickToShowAnswerLabel => _localizationService.GetString("ClickToShowAnswer");
        public string RateKnowledgeLabel => _localizationService.GetString("RateKnowledge");

        public ICommand ShowAnswerCommand { get; private set; } = null!;
        public ICommand KnowCommand { get; private set; } = null!;
        public ICommand DontKnowCommand { get; private set; } = null!;
        public ICommand NextCardCommand { get; private set; } = null!;
        public ICommand PreviousCardCommand { get; private set; } = null!;
        public ICommand RestartCommand { get; private set; } = null!;

        public event Action? RequestClose;

        private void InitializeCommands()
        {
            ShowAnswerCommand = new RelayCommand(ShowAnswerAction);
            KnowCommand = new RelayCommand(async () => await MarkAsKnownAsync());
            DontKnowCommand = new RelayCommand(async () => await MarkAsLearningAsync());
            NextCardCommand = new RelayCommand(NextCard, () => CanGoNext);
            PreviousCardCommand = new RelayCommand(PreviousCard, () => CanGoPrevious);
            RestartCommand = new RelayCommand(Restart);
        }

        private async Task LoadStudyCardsAsync()
        {
            try
            {
                List<FlashCard> allCards;
                
                var allTopicsString = _localizationService.GetString("All");
                if (_studyTopic == allTopicsString || string.IsNullOrEmpty(_studyTopic))
                {
                    allCards = await _cardService.GetAllCardsAsync();
                }
                else
                {
                    allCards = await _cardService.GetCardsByTopicAsync(_studyTopic);
                }

                _studyCards = allCards
                    .OrderBy(c => c.Status == CardStatus.Known ? 1 : 0)
                    .ThenBy(c => c.LastViewedAt ?? DateTime.MinValue)
                    .ToList();

                if (_studyCards.Any())
                {
                    CurrentCard = _studyCards[0];
                    OnPropertyChanged(nameof(Progress));
                    OnPropertyChanged(nameof(HasCards));
                    OnPropertyChanged(nameof(CanGoNext));
                    OnPropertyChanged(nameof(CanGoPrevious));
                    OnPropertyChanged(nameof(TotalCards));
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading study cards: {ex.Message}");
            }
        }

        private void ShowAnswerAction()
        {
            ShowAnswer = true;
        }

        private async Task MarkAsKnownAsync()
        {
            if (CurrentCard != null)
            {
                await _cardService.UpdateCardStatusAsync(CurrentCard.Id, CardStatus.Known);
                CurrentCard.Status = CardStatus.Known;
                NextCard();
            }
        }

        private async Task MarkAsLearningAsync()
        {
            if (CurrentCard != null)
            {
                await _cardService.UpdateCardStatusAsync(CurrentCard.Id, CardStatus.Learning);
                CurrentCard.Status = CardStatus.Learning;
                NextCard();
            }
        }

        private void NextCard()
        {
            if (CanGoNext)
            {
                CurrentIndex++;
                CurrentCard = _studyCards[CurrentIndex];
                ShowAnswer = false;
                OnPropertyChanged(nameof(Progress));
                OnPropertyChanged(nameof(CanGoNext));
                OnPropertyChanged(nameof(CanGoPrevious));
                
                ((RelayCommand)NextCardCommand).RaiseCanExecuteChanged();
                ((RelayCommand)PreviousCardCommand).RaiseCanExecuteChanged();
            }
            else if (CurrentIndex == TotalCards - 1)
            {
                System.Diagnostics.Debug.WriteLine("Study session completed!");
            }
        }

        private void PreviousCard()
        {
            if (CanGoPrevious)
            {
                CurrentIndex--;
                CurrentCard = _studyCards[CurrentIndex];
                ShowAnswer = false;
                OnPropertyChanged(nameof(Progress));
                OnPropertyChanged(nameof(CanGoNext));
                OnPropertyChanged(nameof(CanGoPrevious));
                
                ((RelayCommand)NextCardCommand).RaiseCanExecuteChanged();
                ((RelayCommand)PreviousCardCommand).RaiseCanExecuteChanged();
            }
        }

        private void Restart()
        {
            CurrentIndex = 0;
            if (_studyCards.Any())
            {
                CurrentCard = _studyCards[0];
                ShowAnswer = false;
                OnPropertyChanged(nameof(Progress));
                OnPropertyChanged(nameof(CanGoNext));
                OnPropertyChanged(nameof(CanGoPrevious));
                
                ((RelayCommand)NextCardCommand).RaiseCanExecuteChanged();
                ((RelayCommand)PreviousCardCommand).RaiseCanExecuteChanged();
            }
        }

        private void OnLanguageChanged()
        {
            OnPropertyChanged(nameof(StudyModeLabel));
            OnPropertyChanged(nameof(QuestionLabel));
            OnPropertyChanged(nameof(AnswerLabel));
            OnPropertyChanged(nameof(ShowAnswerLabel));
            OnPropertyChanged(nameof(KnowLabel));
            OnPropertyChanged(nameof(DontKnowLabel));
            OnPropertyChanged(nameof(NextLabel));
            OnPropertyChanged(nameof(PreviousLabel));
            OnPropertyChanged(nameof(RestartLabel));
            OnPropertyChanged(nameof(NoCardsLabel));
            OnPropertyChanged(nameof(AddCardsLabel));
            OnPropertyChanged(nameof(ClickToShowAnswerLabel));
            OnPropertyChanged(nameof(RateKnowledgeLabel));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _localizationService.LanguageChanged -= OnLanguageChanged;
            }
            base.Dispose(disposing);
        }
    }
} 