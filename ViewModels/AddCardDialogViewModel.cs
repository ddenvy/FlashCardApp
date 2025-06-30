using FlashCardApp.Models;
using FlashCardApp.Services;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace FlashCardApp.ViewModels
{
    public class AddCardDialogViewModel : BaseViewModel
    {
        private readonly CardService _cardService;
        private readonly FlashCard? _editingCard;
        private string _question = string.Empty;
        private string _answer = string.Empty;
        private string _selectedTopic = string.Empty;
        private string _newTopic = string.Empty;
        private bool _isEditMode;

        public AddCardDialogViewModel(CardService cardService, FlashCard? editingCard = null)
        {
            _cardService = cardService;
            _editingCard = editingCard;
            _isEditMode = editingCard != null;
            
            InitializeCommands();
            LoadExistingTopics();
            
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
                    CommandManager.InvalidateRequerySuggested();
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
                    CommandManager.InvalidateRequerySuggested();
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
                    CommandManager.InvalidateRequerySuggested();
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
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public bool IsEditMode
        {
            get => _isEditMode;
            set => SetProperty(ref _isEditMode, value);
        }

        public ObservableCollection<string> ExistingTopics { get; } = new ObservableCollection<string>();

        public RelayCommand SaveCommand { get; private set; }
        public RelayCommand CancelCommand { get; private set; }

        private bool? _dialogResult;
        public bool? DialogResult 
        { 
            get => _dialogResult;
            set => SetProperty(ref _dialogResult, value);
        }

        private void InitializeCommands()
        {
            SaveCommand = new RelayCommand(Save, CanSave);
            CancelCommand = new RelayCommand(Cancel);
        }

        private async void LoadExistingTopics()
        {
            var topics = await _cardService.GetAllTopicsAsync();
            ExistingTopics.Clear();
            
            foreach (var topic in topics)
            {
                ExistingTopics.Add(topic);
            }
        }

        private bool CanSave()
        {
            return !string.IsNullOrWhiteSpace(Question) &&
                   !string.IsNullOrWhiteSpace(Answer) &&
                   !string.IsNullOrWhiteSpace(SelectedTopic);
        }

        private async void Save()
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
                CloseDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении карточки: {ex.Message}", 
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancel()
        {
            DialogResult = false;
            CloseDialog();
        }

        private void CloseDialog()
        {
            // Закрытие происходит автоматически через DialogResult
        }
    }
} 