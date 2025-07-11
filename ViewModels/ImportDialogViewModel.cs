using QuickMind.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;

namespace QuickMind.ViewModels
{
    public class ImportDialogViewModel : ViewModelBase
    {
        private readonly AnkiImportService _ankiImportService;
        private readonly CardService _cardService;
        private string _selectedFilePath = string.Empty;
        private string _selectedTopic = string.Empty;
        private string _statusMessage = string.Empty;
        private bool _isImporting = false;
        private ObservableCollection<string> _existingTopics;

        public ImportDialogViewModel(AnkiImportService ankiImportService, CardService cardService)
        {
            _ankiImportService = ankiImportService;
            _cardService = cardService;
            _existingTopics = new ObservableCollection<string>();
            
            InitializeCommands();
            LoadExistingTopics();
        }

        public string SelectedFilePath
        {
            get => _selectedFilePath;
            set => SetProperty(ref _selectedFilePath, value);
        }

        public string SelectedTopic
        {
            get => _selectedTopic;
            set => SetProperty(ref _selectedTopic, value);
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        public bool IsImporting
        {
            get => _isImporting;
            set => SetProperty(ref _isImporting, value);
        }

        public ObservableCollection<string> ExistingTopics
        {
            get => _existingTopics;
            set => SetProperty(ref _existingTopics, value);
        }

        public ICommand SelectFileCommand { get; private set; } = null!;
        public ICommand ImportCommand { get; private set; } = null!;
        public ICommand CancelCommand { get; private set; } = null!;

        public string ImportDialogTitle => LocalizationService.Instance.GetString("ImportDialogTitle");
        public string ImportDialogHeader => LocalizationService.Instance.GetString("ImportDialogHeader");
        public string ImportDialogFilePathWatermark => LocalizationService.Instance.GetString("ImportDialogFilePathWatermark");
        public string ImportDialogBrowse => LocalizationService.Instance.GetString("ImportDialogBrowse");
        public string ImportDialogTopic => LocalizationService.Instance.GetString("ImportDialogTopic");
        public string ImportDialogTopicWatermark => LocalizationService.Instance.GetString("ImportDialogTopicWatermark");
        public string ImportDialogFormatHeader => LocalizationService.Instance.GetString("ImportDialogFormatHeader");
        public string ImportDialogFormatTxt => LocalizationService.Instance.GetString("ImportDialogFormatTxt");
        public string ImportDialogFormatCsv => LocalizationService.Instance.GetString("ImportDialogFormatCsv");
        public string ImportDialogFormatEachLine => LocalizationService.Instance.GetString("ImportDialogFormatEachLine");
        public string ImportDialogCancel => LocalizationService.Instance.GetString("ImportDialogCancel");
        public string ImportDialogImport => LocalizationService.Instance.GetString("Import");

        private void InitializeCommands()
        {
            SelectFileCommand = new RelayCommand(SelectFile);
            ImportCommand = new RelayCommand(async () => await ImportAsync());
            CancelCommand = new RelayCommand(() => { });
        }

        private async void LoadExistingTopics()
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
                StatusMessage = $"Ошибка загрузки тем: {ex.Message}";
            }
        }

        private void SelectFile()
        {
            // Этот метод будет вызван из code-behind
            StatusMessage = "Выберите файл с карточками (поддерживаются .json, .csv, .txt, .apkg)";
        }

        public async Task ImportAsync()
        {
            if (string.IsNullOrEmpty(SelectedFilePath))
            {
                StatusMessage = "Выберите файл для импорта";
                return;
            }

            if (string.IsNullOrEmpty(SelectedTopic))
            {
                StatusMessage = "Введите название темы";
                return;
            }

            if (!File.Exists(SelectedFilePath))
            {
                StatusMessage = "Файл не найден";
                return;
            }

            IsImporting = true;
            StatusMessage = "Импорт в процессе...";

            try
            {
                var result = await _ankiImportService.ImportFromFileAsync(SelectedFilePath, SelectedTopic);
                StatusMessage = result.Success ? $"Импортировано {result.ImportedCards} карточек" : result.ErrorMessage ?? "Ошибка импорта";
                
                if (result.Success)
                {
                    // Обновляем список тем
                    LoadExistingTopics();
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Ошибка импорта: {ex.Message}";
            }
            finally
            {
                IsImporting = false;
            }
        }
    }
} 