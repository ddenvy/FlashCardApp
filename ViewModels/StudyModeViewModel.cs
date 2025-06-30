using FlashCardApp.Models;
using FlashCardApp.Services;
using System.Collections.Generic;
using System.Linq;

namespace FlashCardApp.ViewModels
{
    public class StudyModeViewModel : BaseViewModel
    {
        private readonly CardService _cardService;
        private List<FlashCard> _studyCards;
        private int _currentIndex;
        private FlashCard? _currentCard;
        private bool _showAnswer;
        private string _studyTopic;

        public StudyModeViewModel(CardService cardService, string? topic = null)
        {
            _cardService = cardService;
            _studyTopic = topic ?? "Все темы";
            _studyCards = new List<FlashCard>();
            _currentIndex = 0;
            _showAnswer = false;

            InitializeCommands();
            LoadStudyCards();
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

        // Команды
        public RelayCommand ShowAnswerCommand { get; private set; }
        public RelayCommand KnowCommand { get; private set; }
        public RelayCommand DontKnowCommand { get; private set; }
        public RelayCommand NextCardCommand { get; private set; }
        public RelayCommand PreviousCardCommand { get; private set; }
        public RelayCommand RestartCommand { get; private set; }

        private void InitializeCommands()
        {
            ShowAnswerCommand = new RelayCommand(ShowAnswerAction);
            KnowCommand = new RelayCommand(MarkAsKnown);
            DontKnowCommand = new RelayCommand(MarkAsLearning);
            NextCardCommand = new RelayCommand(NextCard, () => CanGoNext);
            PreviousCardCommand = new RelayCommand(PreviousCard, () => CanGoPrevious);
            RestartCommand = new RelayCommand(Restart);
        }

        private async void LoadStudyCards()
        {
            try
            {
                List<FlashCard> allCards;
                
                if (_studyTopic == "Все темы" || string.IsNullOrEmpty(_studyTopic))
                {
                    allCards = await _cardService.GetAllCardsAsync();
                }
                else
                {
                    allCards = await _cardService.GetCardsByTopicAsync(_studyTopic);
                }

                // Приоритизируем новые и изучаемые карточки
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
                }
            }
            catch (Exception ex)
            {
                // Обработка ошибок
                System.Windows.MessageBox.Show($"Ошибка при загрузке карточек: {ex.Message}");
            }
        }

        private void ShowAnswerAction()
        {
            ShowAnswer = true;
        }

        private async void MarkAsKnown()
        {
            if (CurrentCard != null)
            {
                await _cardService.UpdateCardStatusAsync(CurrentCard.Id, CardStatus.Known);
                CurrentCard.Status = CardStatus.Known;
                NextCard();
            }
        }

        private async void MarkAsLearning()
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
            }
            else if (CurrentIndex == TotalCards - 1)
            {
                // Достигнут конец
                System.Windows.MessageBox.Show("Вы завершили изучение всех карточек!", "Поздравляем!");
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
            }
        }
    }
} 