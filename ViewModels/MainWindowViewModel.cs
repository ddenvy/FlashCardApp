using FlashCardApp.Models;
using FlashCardApp.Services;
using FlashCardApp.Views;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace FlashCardApp.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        private readonly CardService _cardService;
        private ObservableCollection<FlashCard> _allCards;
        private ObservableCollection<string> _topics;
        private string _selectedTopic = "Все темы";
        private CardStatus? _selectedStatus = null;

        public MainWindowViewModel()
        {
            _cardService = new CardService();
            _allCards = new ObservableCollection<FlashCard>();
            _topics = new ObservableCollection<string>();
            
            InitializeCommands();
            LoadDataAsync();
        }

        public ObservableCollection<FlashCard> AllCards
        {
            get => _allCards;
            set => SetProperty(ref _allCards, value);
        }

        public ObservableCollection<string> Topics
        {
            get => _topics;
            set => SetProperty(ref _topics, value);
        }

        public string SelectedTopic
        {
            get => _selectedTopic;
            set
            {
                if (SetProperty(ref _selectedTopic, value))
                {
                    FilterCards();
                }
            }
        }

        public CardStatus? SelectedStatus
        {
            get => _selectedStatus;
            set
            {
                if (SetProperty(ref _selectedStatus, value))
                {
                    FilterCards();
                    OnPropertyChanged(nameof(IsAllStatusSelected));
                }
            }
        }

        public bool IsAllStatusSelected
        {
            get => _selectedStatus == null;
            set
            {
                if (value)
                {
                    SelectedStatus = null;
                }
            }
        }

        // Команды
        public RelayCommand AddCardCommand { get; private set; }
        public RelayCommand<FlashCard> EditCardCommand { get; private set; }
        public RelayCommand<FlashCard> DeleteCardCommand { get; private set; }
        public RelayCommand<string> StartStudyModeCommand { get; private set; }
        public RelayCommand RefreshCommand { get; private set; }

        // Группировка карточек по темам для канбан-доски
        public ObservableCollection<FlashCard> NewCards => 
            new ObservableCollection<FlashCard>(FilteredCards.Where(c => c.Status == CardStatus.New));

        public ObservableCollection<FlashCard> LearningCards => 
            new ObservableCollection<FlashCard>(FilteredCards.Where(c => c.Status == CardStatus.Learning));

        public ObservableCollection<FlashCard> KnownCards => 
            new ObservableCollection<FlashCard>(FilteredCards.Where(c => c.Status == CardStatus.Known));

        private ObservableCollection<FlashCard> FilteredCards => 
            new ObservableCollection<FlashCard>(AllCards.Where(FilterCard));

        private void InitializeCommands()
        {
            AddCardCommand = new RelayCommand(AddCard);
            EditCardCommand = new RelayCommand<FlashCard>(EditCard);
            DeleteCardCommand = new RelayCommand<FlashCard>(DeleteCard);
            StartStudyModeCommand = new RelayCommand<string>(StartStudyMode);
            RefreshCommand = new RelayCommand(() => LoadDataAsync());
        }

        private async void LoadDataAsync()
        {
            await _cardService.InitializeDatabaseAsync();
            
            var cards = await _cardService.GetAllCardsAsync();
            AllCards.Clear();
            foreach (var card in cards)
            {
                AllCards.Add(card);
            }

            var topics = await _cardService.GetAllTopicsAsync();
            Topics.Clear();
            Topics.Add("Все темы");
            foreach (var topic in topics)
            {
                Topics.Add(topic);
            }

            OnPropertyChanged(nameof(NewCards));
            OnPropertyChanged(nameof(LearningCards));
            OnPropertyChanged(nameof(KnownCards));
        }

        private void FilterCards()
        {
            OnPropertyChanged(nameof(NewCards));
            OnPropertyChanged(nameof(LearningCards));
            OnPropertyChanged(nameof(KnownCards));
        }

        private bool FilterCard(FlashCard card)
        {
            bool topicMatch = SelectedTopic == "Все темы" || card.Topic == SelectedTopic;
            bool statusMatch = SelectedStatus == null || card.Status == SelectedStatus;
            
            return topicMatch && statusMatch;
        }

        private void AddCard()
        {
            var dialog = new AddCardDialog();
            var viewModel = new AddCardDialogViewModel(_cardService);
            dialog.DataContext = viewModel;
            
            if (dialog.ShowDialog() == true)
            {
                LoadDataAsync();
            }
        }

        private void EditCard(FlashCard? card)
        {
            if (card == null) return;

            var dialog = new AddCardDialog();
            var viewModel = new AddCardDialogViewModel(_cardService, card);
            dialog.DataContext = viewModel;
            dialog.Title = "Редактировать карточку";
            
            if (dialog.ShowDialog() == true)
            {
                LoadDataAsync();
            }
        }

        private async void DeleteCard(FlashCard? card)
        {
            if (card == null) return;

            var result = MessageBox.Show(
                $"Удалить карточку?\n\nВопрос: {card.Question}",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                await _cardService.DeleteCardAsync(card.Id);
                LoadDataAsync();
            }
        }

        private void StartStudyMode(string? topic)
        {
            var studyWindow = new StudyModeWindow();
            var viewModel = new StudyModeViewModel(_cardService, topic);
            studyWindow.DataContext = viewModel;
            studyWindow.Show();
        }


    }
} 