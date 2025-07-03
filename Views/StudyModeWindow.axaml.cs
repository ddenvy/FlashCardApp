using Avalonia.Controls;
using QuickMind.ViewModels;

namespace QuickMind.Views
{
    public partial class StudyModeWindow : Window
    {
        public StudyModeWindow()
        {
            InitializeComponent();
        }

        public StudyModeWindow(StudyModeViewModel viewModel) : this()
        {
            DataContext = viewModel;
            viewModel.RequestClose += () => Close();
        }
    }
}