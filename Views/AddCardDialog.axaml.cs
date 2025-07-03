using Avalonia.Controls;
using QuickMind.ViewModels;

namespace QuickMind.Views
{
    public partial class AddCardDialog : Window
    {
        public AddCardDialog()
        {
            InitializeComponent();
        }

        public AddCardDialog(AddCardDialogViewModel viewModel) : this()
        {
            DataContext = viewModel;
            viewModel.RequestClose += () => Close();
        }
    }
} 