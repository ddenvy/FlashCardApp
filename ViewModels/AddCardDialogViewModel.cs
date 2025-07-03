using QuickMind.Models;
using QuickMind.Services;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace QuickMind.ViewModels
{
    public class AddCardDialogViewModel : ViewModelBase
    {
        private readonly CardService _cardService;
        private readonly LocalizationService _localizationService;
        private readonly FlashCard? _editingCard;
        private string _question = string.Empty;
        private string _answer = string.Empty;
        private string _selectedTopic = string.Empty;
        private string _newTopic = string.Empty;
        private bool _isEditMode;

        public AddCardDialogViewModel(CardService cardService, FlashCard? editingCard = null)
        {
            _cardService = cardService;
            _localizationService = LocalizationService.Instance;
            _editingCard = editingCard;
            _isEditMode = editingCard != null;
            
            _localizationService.LanguageChanged += OnLanguageChanged;
            
            InitializeCommands();
            _ = LoadExistingTopicsAsync();
            
            if (_editingCard != null)
            {
                Question = _editingCard.Question;
                Answer = _editingCard.Answer;
                SelectedTopic = _editingCard.Topic;
            }
        }

        public string Question
        {
            get => _question;
            set 
            { 
                if (SetProperty(ref _question, value))
                {
                    (SaveCommand as RelayCommand)?.RaiseCanExecuteChanged();
                }
            }
        }

        public string Answer
        {
            get => _answer;
            set 
            { 
                if (SetProperty(ref _answer, value))
                {
                    (SaveCommand as RelayCommand)?.RaiseCanExecuteChanged();
                }
            }
        }

        public string SelectedTopic
        {
            get => _selectedTopic;
            set 
            { 
                if (SetProperty(ref _selectedTopic, value))
                {
                    (SaveCommand as RelayCommand)?.RaiseCanExecuteChanged();
                }
            }
        }

        public string NewTopic
        {
            get => _newTopic;
            set
            {
                if (SetProperty(ref _newTopic, value))
                {
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        SelectedTopic = value;
                    }
                    (SaveCommand as RelayCommand)?.RaiseCanExecuteChanged();
                }
            }
        }

        public bool IsEditMode
        {
            get => _isEditMode;
            set => SetProperty(ref _isEditMode, value);
        }

        public ObservableCollection<string> ExistingTopics { get; } = new ObservableCollection<string>();

        public string DialogTitle => IsEditMode ? _localizationService.GetString("EditCard") : _localizationService.GetString("AddCard");
        public string QuestionLabel => _localizationService.GetString("Question");
        public string AnswerLabel => _localizationService.GetString("Answer");
        public string TopicLabel => _localizationService.GetString("Topic");
        public string NewTopicLabel => _localizationService.GetString("NewTopic");
        public string PreviewLabel => _localizationService.GetString("Preview");
        public string SaveLabel => _localizationService.GetString("Save");
        public string CancelLabel => _localizationService.GetString("Cancel");
        public string OrCreateNewLabel => _localizationService.GetString("OrCreateNew");

        public ICommand SaveCommand { get; private set; } = null!;
        public ICommand CancelCommand { get; private set; } = null!;

        private bool? _dialogResult;
        public bool? DialogResult 
        { 
            get => _dialogResult;
            set => SetProperty(ref _dialogResult, value);
        }

        public event Action? RequestClose;

        private void InitializeCommands()
        {
            SaveCommand = new RelayCommand(async () => await SaveAsync(), CanSave);
            CancelCommand = new RelayCommand(Cancel);
        }

        private async Task LoadExistingTopicsAsync()
        {
            try
            {
                var topics = await _cardService.GetAllTopicsAsync();
                ExistingTopics.Clear();
                
                foreach (var topic in topics)
                {
                    ExistingTopics.Add(topic);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading topics: {ex.Message}");
            }
        }

        private bool CanSave()
        {
            return !string.IsNullOrWhiteSpace(Question) &&
                   !string.IsNullOrWhiteSpace(Answer) &&
                   !string.IsNullOrWhiteSpace(SelectedTopic);
        }

        private async Task SaveAsync()
        {
            try
            {
                if (IsEditMode && _editingCard != null)
                {
                    _editingCard.Question = Question.Trim();
                    _editingCard.Answer = Answer.Trim();
                    _editingCard.Topic = SelectedTopic.Trim();
                    
                    await _cardService.UpdateCardAsync(_editingCard);
                }
                else
                {
                    var newCard = new FlashCard
                    {
                        Question = Question.Trim(),
                        Answer = Answer.Trim(),
                        Topic = SelectedTopic.Trim(),
                        Status = CardStatus.New,
                        CreatedAt = DateTime.Now
                    };
                    
                    await _cardService.AddCardAsync(newCard);
                }

                DialogResult = true;
                RequestClose?.Invoke();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving card: {ex.Message}");
            }
        }

        private void Cancel()
        {
            DialogResult = false;
            RequestClose?.Invoke();
        }

        private void OnLanguageChanged()
        {
            OnPropertyChanged(nameof(DialogTitle));
            OnPropertyChanged(nameof(QuestionLabel));
            OnPropertyChanged(nameof(AnswerLabel));
            OnPropertyChanged(nameof(TopicLabel));
            OnPropertyChanged(nameof(NewTopicLabel));
            OnPropertyChanged(nameof(PreviewLabel));
            OnPropertyChanged(nameof(SaveLabel));
            OnPropertyChanged(nameof(CancelLabel));
            OnPropertyChanged(nameof(OrCreateNewLabel));
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