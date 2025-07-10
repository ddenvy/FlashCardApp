using QuickMind.Models;
using QuickMind.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace QuickMind.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly CardService _cardService;
    private readonly LocalizationService _localizationService;
    private ObservableCollection<FlashCard> _allCards;
    private ObservableCollection<string> _topics;
    private string _selectedTopic = "All";
    private CardStatus? _selectedStatus = null;
    private LanguageItem _currentLanguage;
    
    private ObservableCollection<FlashCard> _newCards;
    private ObservableCollection<FlashCard> _learningCards;
    private ObservableCollection<FlashCard> _knownCards;

    public MainWindowViewModel()
    {
        _cardService = new CardService();
        _localizationService = LocalizationService.Instance;
        _allCards = new ObservableCollection<FlashCard>();
        _topics = new ObservableCollection<string>();
        _newCards = new ObservableCollection<FlashCard>();
        _learningCards = new ObservableCollection<FlashCard>();
        _knownCards = new ObservableCollection<FlashCard>();
        
        _currentLanguage = _localizationService.AvailableLanguages.FirstOrDefault(l => l.Code == _localizationService.CurrentCulture.Name) 
                          ?? _localizationService.AvailableLanguages.First();
        _localizationService.LanguageChanged += OnLanguageChanged;
        
        _cardService.CardsChanged += OnCardsChanged;
        
        InitializeCommands();
        _ = LoadDataAsync();
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

    public LanguageItem CurrentLanguage
    {
        get => _currentLanguage;
        set
        {
            if (SetProperty(ref _currentLanguage, value))
            {
                _localizationService.SetLanguage(value.Code);
            }
        }
    }

    public List<LanguageItem> AvailableLanguages => _localizationService.AvailableLanguages;

    public string NewLabel => _localizationService.GetString("New");
    public string LearningLabel => _localizationService.GetString("Learning");
    public string KnownLabel => _localizationService.GetString("Known");
    public string AddCardLabel => _localizationService.GetString("AddCard");
    public string StudyModeLabel => _localizationService.GetString("StudyMode");
    public string EditLabel => _localizationService.GetString("Edit");
    public string DeleteLabel => _localizationService.GetString("Delete");
    public string MoveToLearningTooltip => _localizationService.GetString("MoveToLearning");
    public string MoveToKnownTooltip => _localizationService.GetString("MoveToKnown");
    public string MoveToNewTooltip => _localizationService.GetString("MoveToNew");
    public string ImportLabel => _localizationService.GetString("Import");
    public string DeleteTopicLabel => _localizationService.GetString("DeleteTopic");

    public ICommand AddCardCommand { get; private set; } = null!;
    public ICommand RefreshCommand { get; private set; } = null!;
    public ICommand EditCardCommand { get; private set; } = null!;
    public ICommand DeleteCardCommand { get; private set; } = null!;
    public ICommand StartStudyCommand { get; private set; } = null!;
    public ICommand ShowLanguageSelectionCommand { get; private set; } = null!;
    public ICommand ImportCardsCommand { get; private set; } = null!;
    public ICommand DeleteTopicCommand { get; private set; } = null!;
    public ICommand MoveToNewCommand { get; private set; } = null!;
    public ICommand MoveToLearningCommand { get; private set; } = null!;
    public ICommand MoveToKnownCommand { get; private set; } = null!;

    public ObservableCollection<FlashCard> NewCards => _newCards;
    public ObservableCollection<FlashCard> LearningCards => _learningCards;
    public ObservableCollection<FlashCard> KnownCards => _knownCards;

    public async void MoveCardToStatus(FlashCard card, CardStatus newStatus)
    {
        if (card.Status == newStatus)
            return;
        try
        {
            card.Status = newStatus;
            await _cardService.UpdateCardAsync(card);
            await LoadDataAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error moving card: {ex.Message}");
        }
    }

    private async Task MoveCardToStatusAsync(FlashCard card, CardStatus newStatus)
    {
        if (card.Status == newStatus)
            return;
        try
        {
            card.Status = newStatus;
            await _cardService.UpdateCardAsync(card);
            await LoadDataAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error moving card: {ex.Message}");
        }
    }

    private void InitializeCommands()
    {
        AddCardCommand = new RelayCommand(() => AddCardAsync());
        RefreshCommand = new RelayCommand(async () => await LoadDataAsync());
        EditCardCommand = new RelayCommand<FlashCard>((card) => EditCardAsync(card));
        DeleteCardCommand = new RelayCommand<FlashCard>(async (card) => await DeleteCardAsync(card));
        StartStudyCommand = new RelayCommand(StartStudy);
        ShowLanguageSelectionCommand = new RelayCommand(() => ShowLanguageSelectionAsync());
        ImportCardsCommand = new RelayCommand(() => ImportCardsAsync());
        DeleteTopicCommand = new RelayCommand(async () => await DeleteTopicAsync());
        MoveToNewCommand = new RelayCommand<FlashCard>(async (card) => await MoveCardToStatusAsync(card, CardStatus.New));
        MoveToLearningCommand = new RelayCommand<FlashCard>(async (card) => await MoveCardToStatusAsync(card, CardStatus.Learning));
        MoveToKnownCommand = new RelayCommand<FlashCard>(async (card) => await MoveCardToStatusAsync(card, CardStatus.Known));
    }

    private async Task LoadDataAsync()
    {
        try
        {
            await _cardService.InitializeDatabaseAsync();
            
            var cards = await _cardService.GetAllCardsAsync();
            
            AllCards.Clear();
            foreach (var card in cards)
            {
                AllCards.Add(card);
            }

            var topics = await _cardService.GetAllTopicsAsync();
            
            var currentSelectedTopic = SelectedTopic;
            
            Topics.Clear();
            var allTopicsString = _localizationService.GetString("All");
            Topics.Add(allTopicsString);
            foreach (var topic in topics)
            {
                Topics.Add(topic);
            }
            
            if (!string.IsNullOrEmpty(currentSelectedTopic) && Topics.Contains(currentSelectedTopic))
            {
                SelectedTopic = currentSelectedTopic;
            }
            else
            {
                SelectedTopic = allTopicsString;
            }

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
        var filteredCards = AllCards.Where(FilterCard).ToList();
        
        _newCards.Clear();
        foreach (var card in filteredCards.Where(c => c.Status == CardStatus.New))
        {
            _newCards.Add(card);
        }

        _learningCards.Clear();
        foreach (var card in filteredCards.Where(c => c.Status == CardStatus.Learning))
        {
            _learningCards.Add(card);
        }

        _knownCards.Clear();
        foreach (var card in filteredCards.Where(c => c.Status == CardStatus.Known))
        {
            _knownCards.Add(card);
        }
    }

    private bool FilterCard(FlashCard card)
    {
        var allTopicsString = _localizationService.GetString("All");
        bool topicMatch = SelectedTopic == allTopicsString || card.Topic == SelectedTopic;
        bool statusMatch = SelectedStatus == null || card.Status == SelectedStatus;
        
        return topicMatch && statusMatch;
    }

    private void AddCardAsync()
    {
        try
        {
            var viewModel = new AddCardDialogViewModel(_cardService);
            var dialog = new Views.AddCardDialog(viewModel);
            
            dialog.Show();
            
            dialog.Closed += async (s, e) =>
            {
                if (viewModel.DialogResult == true)
                {
                    await LoadDataAsync();
                }
            };
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error opening add card dialog: {ex.Message}");
        }
    }

    private void EditCardAsync(FlashCard? card)
    {
        if (card == null) return;

        try
        {
            var viewModel = new AddCardDialogViewModel(_cardService, card);
            var dialog = new Views.AddCardDialog(viewModel);
            
            dialog.Show();
            
            dialog.Closed += async (s, e) =>
            {
                if (viewModel.DialogResult == true)
                {
                    await LoadDataAsync();
                }
            };
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error opening edit card dialog: {ex.Message}");
        }
    }

    private async Task DeleteCardAsync(FlashCard? card)
    {
        if (card == null) return;

        try
        {
            await _cardService.DeleteCardAsync(card.Id);
            await LoadDataAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error deleting card: {ex.Message}");
        }
    }

    private void StartStudy()
    {
        try
        {
            string? studyTopic = null;
            var allTopicsString = _localizationService.GetString("All");
            if (SelectedTopic != allTopicsString)
            {
                studyTopic = SelectedTopic;
            }

            var viewModel = new StudyModeViewModel(_cardService, studyTopic);
            var studyWindow = new Views.StudyModeWindow(viewModel);
            
            studyWindow.Show();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error opening study mode: {ex.Message}");
        }
    }

    private void ShowLanguageSelectionAsync()
    {
        try
        {
            var languageWindow = new Views.LanguageSelectionWindow();
            languageWindow.Show();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error opening language selection: {ex.Message}");
        }
    }

    private void ImportCardsAsync()
    {
        try
        {
            var importService = new ImportService(_cardService);
            var viewModel = new ImportDialogViewModel(importService);
            var dialog = new Views.ImportDialog(viewModel);
            
            dialog.Show();
            
            dialog.Closed += async (s, e) =>
            {
                await LoadDataAsync();
            };
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error opening import dialog: {ex.Message}");
        }
    }

    private async Task DeleteTopicAsync()
    {
        var allTopicsString = _localizationService.GetString("All");
        if (SelectedTopic == allTopicsString)
            return;
        try
        {
            using var context = new FlashCardContext();
            var cards = context.FlashCards.Where(c => c.Topic == SelectedTopic).ToList();
            if (cards.Count == 0)
                return;
            context.FlashCards.RemoveRange(cards);
            await context.SaveChangesAsync();
            await LoadDataAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error deleting topic: {ex.Message}");
        }
    }

    private void OnLanguageChanged()
    {
        var newLanguage = _localizationService.AvailableLanguages.FirstOrDefault(l => l.Code == _localizationService.CurrentCulture.Name) 
                         ?? _localizationService.AvailableLanguages.First();
        SetProperty(ref _currentLanguage, newLanguage, nameof(CurrentLanguage));
        OnPropertyChanged(nameof(NewLabel));
        OnPropertyChanged(nameof(LearningLabel));
        OnPropertyChanged(nameof(KnownLabel));
        OnPropertyChanged(nameof(AddCardLabel));
        OnPropertyChanged(nameof(StudyModeLabel));
        OnPropertyChanged(nameof(EditLabel));
        OnPropertyChanged(nameof(DeleteLabel));
        OnPropertyChanged(nameof(MoveToLearningTooltip));
        OnPropertyChanged(nameof(MoveToKnownTooltip));
        OnPropertyChanged(nameof(MoveToNewTooltip));
        OnPropertyChanged(nameof(ImportLabel));
        OnPropertyChanged(nameof(DeleteTopicLabel));
        
        var allTopicsString = _localizationService.GetString("All");
        
        var currentSelectedTopic = SelectedTopic;
        var isAllTopicsSelected = (currentSelectedTopic == "Все темы" || 
                                 currentSelectedTopic == "All" || 
                                 currentSelectedTopic == "全部");
        
        var topics = Topics.Where(t => t != "Все темы" && t != "All" && t != "全部").ToList();
        
        Topics.Clear();
        Topics.Add(allTopicsString);
        foreach (var topic in topics)
        {
            Topics.Add(topic);
        }
        
        if (isAllTopicsSelected)
        {
            SelectedTopic = allTopicsString;
        }
        else if (Topics.Contains(currentSelectedTopic))
        {
            SelectedTopic = currentSelectedTopic;
        }
        else
        {
            SelectedTopic = allTopicsString;
        }
    }

    private async void OnCardsChanged()
    {
        try
        {
            await LoadDataAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error refreshing cards: {ex.Message}");
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _localizationService.LanguageChanged -= OnLanguageChanged;
            _cardService.CardsChanged -= OnCardsChanged;
        }
        base.Dispose(disposing);
    }
}
