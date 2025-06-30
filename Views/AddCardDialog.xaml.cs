using FlashCardApp.ViewModels;
using System.Windows;

namespace FlashCardApp.Views
{
    public partial class AddCardDialog : Window
    {
        public AddCardDialog()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is AddCardDialogViewModel viewModel)
            {
                viewModel.PropertyChanged += (s, args) =>
                {
                    if (args.PropertyName == nameof(AddCardDialogViewModel.DialogResult))
                    {
                        DialogResult = viewModel.DialogResult;
                    }
                };
            }
        }
    }
} 