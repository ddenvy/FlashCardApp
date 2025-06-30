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
#if DEBUG
                System.Diagnostics.Debug.WriteLine($"SelectedTopic changing from '{_selectedTopic}' to '{value}'");
#endif
                if (SetProperty(ref _selectedTopic, value))
                {
#if DEBUG
                    System.Diagnostics.Debug.WriteLine($"SelectedTopic changed to '{value}', calling FilterCards()");
#endif
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
#if DEBUG
            System.Diagnostics.Debug.WriteLine("LoadDataAsync started");
#endif            
            try
            {
                await _cardService.InitializeDatabaseAsync();
                
                var cards = await _cardService.GetAllCardsAsync();
#if DEBUG
                System.Diagnostics.Debug.WriteLine($"Loaded {cards.Count} cards from database");
#endif
                
                // Обновляем коллекции в UI потоке
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    AllCards.Clear();
                    foreach (var card in cards)
                    {
                        AllCards.Add(card);
                    }
#if DEBUG
                    System.Diagnostics.Debug.WriteLine($"AllCards.Count after loading: {AllCards.Count}");
#endif
                });

                var topics = await _cardService.GetAllTopicsAsync();
                
                // Обновляем темы в UI потоке
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    // Сохраняем текущую выбранную тему
                    var currentSelectedTopic = SelectedTopic;
#if DEBUG
                    System.Diagnostics.Debug.WriteLine($"Current selected topic before update: {currentSelectedTopic}");
#endif
                    
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
#if DEBUG
                        System.Diagnostics.Debug.WriteLine($"Restored selected topic: {currentSelectedTopic}");
#endif
                    }
                    else
                    {
                        SelectedTopic = "Все темы";
#if DEBUG
                        System.Diagnostics.Debug.WriteLine("Selected topic reset to 'Все темы'");
#endif
                    }
                });

                // Обновляем группированные коллекции
#if DEBUG
                System.Diagnostics.Debug.WriteLine("Calling UpdateGroupedCards from LoadDataAsync");
#endif
                UpdateGroupedCards();
            }
            catch (Exception ex)
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine($"Error in LoadDataAsync: {ex.Message}");
#endif
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
            
#if DEBUG
            // Отладочная информация
            System.Diagnostics.Debug.WriteLine($"UpdateGroupedCards: AllCards.Count = {AllCards.Count}, FilteredCards.Count = {filteredCards.Count}");
#endif

            // Обновляем коллекцию новых карточек
            _newCards.Clear();
            foreach (var card in filteredCards.Where(c => c.Status == CardStatus.New))
            {
                _newCards.Add(card);
            }
#if DEBUG
            System.Diagnostics.Debug.WriteLine($"NewCards after update: {_newCards.Count}");
#endif

            // Обновляем коллекцию изучаемых карточек
            _learningCards.Clear();
            foreach (var card in filteredCards.Where(c => c.Status == CardStatus.Learning))
            {
                _learningCards.Add(card);
            }
#if DEBUG
            System.Diagnostics.Debug.WriteLine($"LearningCards after update: {_learningCards.Count}");
#endif

            // Обновляем коллекцию изученных карточек
            _knownCards.Clear();
            foreach (var card in filteredCards.Where(c => c.Status == CardStatus.Known))
            {
                _knownCards.Add(card);
            }
#if DEBUG
            System.Diagnostics.Debug.WriteLine($"KnownCards after update: {_knownCards.Count}");
#endif
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

            #if DEBUG
                System.Diagnostics.Debug.WriteLine($"DeleteCard called for card: {card.Question}");
#endif

            var result = MessageBox.Show(
                $"Удалить карточку?\n\nВопрос: {card.Question}",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
#if DEBUG
                    System.Diagnostics.Debug.WriteLine($"Deleting card with ID: {card.Id}");
#endif
                    await _cardService.DeleteCardAsync(card.Id);
#if DEBUG
                    System.Diagnostics.Debug.WriteLine("Card deleted from database, calling LoadDataAsync...");
#endif
                    LoadDataAsync();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении карточки: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
#if DEBUG
                    System.Diagnostics.Debug.WriteLine($"Error deleting card: {ex.Message}");
#endif
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