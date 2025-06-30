using FlashCardApp.Models;
using FlashCardApp.Services;
using FlashCardApp.Views;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Threading;

namespace FlashCardApp.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        private readonly CardService _cardService;
        private ObservableCollection<FlashCard> _allCards;
        private ObservableCollection<string> _topics;
        private string _selectedTopic = "Все темы";
        private CardStatus? _selectedStatus = null;
        
        // Отдельные коллекции для каждого статуса карточек
        private ObservableCollection<FlashCard> _newCards;
        private ObservableCollection<FlashCard> _learningCards;
        private ObservableCollection<FlashCard> _knownCards;

        public MainWindowViewModel()
        {
            _cardService = new CardService();
            _allCards = new ObservableCollection<FlashCard>();
            _topics = new ObservableCollection<string>();
            _newCards = new ObservableCollection<FlashCard>();
            _learningCards = new ObservableCollection<FlashCard>();
            _knownCards = new ObservableCollection<FlashCard>();
            
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
                System.Diagnostics.Debug.WriteLine($"SelectedTopic changing from '{_selectedTopic}' to '{value}'");
                if (SetProperty(ref _selectedTopic, value))
                {
                    System.Diagnostics.Debug.WriteLine($"SelectedTopic changed to '{value}', calling FilterCards()");
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
        public ObservableCollection<FlashCard> NewCards => _newCards;
        public ObservableCollection<FlashCard> LearningCards => _learningCards;
        public ObservableCollection<FlashCard> KnownCards => _knownCards;

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
            System.Diagnostics.Debug.WriteLine("LoadDataAsync started");
            
            try
            {
                await _cardService.InitializeDatabaseAsync();
                
                var cards = await _cardService.GetAllCardsAsync();
                System.Diagnostics.Debug.WriteLine($"Loaded {cards.Count} cards from database");
                
                // Обновляем коллекции в UI потоке
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    AllCards.Clear();
                    foreach (var card in cards)
                    {
                        AllCards.Add(card);
                    }
                    System.Diagnostics.Debug.WriteLine($"AllCards.Count after loading: {AllCards.Count}");
                });

                var topics = await _cardService.GetAllTopicsAsync();
                
                // Обновляем темы в UI потоке
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    // Сохраняем текущую выбранную тему
                    var currentSelectedTopic = SelectedTopic;
                    System.Diagnostics.Debug.WriteLine($"Current selected topic before update: {currentSelectedTopic}");
                    
                    Topics.Clear();
                    Topics.Add("Все темы");
                    foreach (var topic in topics)
                    {
                        Topics.Add(topic);
                    }
                    
                    // Восстанавливаем выбранную тему, если она все еще существует
                    if (!string.IsNullOrEmpty(currentSelectedTopic) && Topics.Contains(currentSelectedTopic))
                    {
                        SelectedTopic = currentSelectedTopic;
                        System.Diagnostics.Debug.WriteLine($"Restored selected topic: {currentSelectedTopic}");
                    }
                    else
                    {
                        SelectedTopic = "Все темы";
                        System.Diagnostics.Debug.WriteLine("Selected topic reset to 'Все темы'");
                    }
                });

                // Обновляем группированные коллекции
                System.Diagnostics.Debug.WriteLine("Calling UpdateGroupedCards from LoadDataAsync");
                UpdateGroupedCards();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in LoadDataAsync: {ex.Message}");
            }
        }

        private void FilterCards()
        {
            UpdateGroupedCards();
        }

        private void UpdateGroupedCards()
        {
            // Убеждаемся, что обновления происходят в UI потоке
            if (Application.Current.Dispatcher.CheckAccess())
            {
                UpdateGroupedCardsInternal();
            }
            else
            {
                Application.Current.Dispatcher.Invoke(UpdateGroupedCardsInternal);
            }
        }

        private void UpdateGroupedCardsInternal()
        {
            var filteredCards = AllCards.Where(FilterCard).ToList();
            
            // Отладочная информация
            System.Diagnostics.Debug.WriteLine($"UpdateGroupedCards: AllCards.Count = {AllCards.Count}, FilteredCards.Count = {filteredCards.Count}");

            // Обновляем коллекцию новых карточек
            _newCards.Clear();
            foreach (var card in filteredCards.Where(c => c.Status == CardStatus.New))
            {
                _newCards.Add(card);
            }
            System.Diagnostics.Debug.WriteLine($"NewCards after update: {_newCards.Count}");

            // Обновляем коллекцию изучаемых карточек
            _learningCards.Clear();
            foreach (var card in filteredCards.Where(c => c.Status == CardStatus.Learning))
            {
                _learningCards.Add(card);
            }
            System.Diagnostics.Debug.WriteLine($"LearningCards after update: {_learningCards.Count}");

            // Обновляем коллекцию изученных карточек
            _knownCards.Clear();
            foreach (var card in filteredCards.Where(c => c.Status == CardStatus.Known))
            {
                _knownCards.Add(card);
            }
            System.Diagnostics.Debug.WriteLine($"KnownCards after update: {_knownCards.Count}");
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

            System.Diagnostics.Debug.WriteLine($"DeleteCard called for card: {card.Question}");

            var result = MessageBox.Show(
                $"Удалить карточку?\n\nВопрос: {card.Question}",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    System.Diagnostics.Debug.WriteLine($"Deleting card with ID: {card.Id}");
                    await _cardService.DeleteCardAsync(card.Id);
                    System.Diagnostics.Debug.WriteLine("Card deleted from database, calling LoadDataAsync...");
                    LoadDataAsync();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении карточки: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    System.Diagnostics.Debug.WriteLine($"Error deleting card: {ex.Message}");
                }
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