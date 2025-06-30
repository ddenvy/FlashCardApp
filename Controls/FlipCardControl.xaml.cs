using FlashCardApp.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace FlashCardApp.Controls
{
    public partial class FlipCardControl : UserControl
    {
        private bool _isFlipped = false;
        private Storyboard? _flipToAnswerStoryboard;
        private Storyboard? _flipToQuestionStoryboard;

        public static readonly DependencyProperty FlashCardProperty =
            DependencyProperty.Register(nameof(FlashCard), typeof(FlashCard), 
                typeof(FlipCardControl), new PropertyMetadata(null, OnFlashCardChanged));

        public FlashCard FlashCard
        {
            get => (FlashCard)GetValue(FlashCardProperty);
            set => SetValue(FlashCardProperty, value);
        }

        public FlipCardControl()
        {
            InitializeComponent();
            Loaded += OnLoaded;
            MouseLeftButtonDown += OnMouseLeftButtonDown;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            _flipToAnswerStoryboard = (Storyboard)Resources["FlipToAnswer"];
            _flipToQuestionStoryboard = (Storyboard)Resources["FlipToQuestion"];
        }

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            FlipCard();
        }

        private static void OnFlashCardChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FlipCardControl control && e.NewValue is FlashCard card)
            {
                control.DataContext = new FlashCardViewModel(card);
                control._isFlipped = false;
            }
        }

        private void FlipCard()
        {
            if (_isFlipped)
            {
                _flipToQuestionStoryboard?.Begin();
            }
            else
            {
                _flipToAnswerStoryboard?.Begin();
            }
            _isFlipped = !_isFlipped;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Предотвращаем переворот карточки при нажатии на кнопки управления
            e.Handled = true;
        }
    }

    // ViewModel для отображения карточки
    public class FlashCardViewModel
    {
        private readonly FlashCard _card;

        public FlashCardViewModel(FlashCard card)
        {
            _card = card;
        }

        // Добавляем доступ к исходному объекту FlashCard
        public FlashCard Card => _card;

        public string Question => _card?.Question ?? "";
        public string Answer => _card?.Answer ?? "";
        public string Topic => _card?.Topic ?? "";
        public string Status => _card?.Status.ToString() ?? "";

        public Brush TopicColor
        {
            get
            {
                return Topic switch
                {
                    "C#" => new SolidColorBrush(Color.FromRgb(156, 39, 176)), // Purple
                    "SQL" => new SolidColorBrush(Color.FromRgb(33, 150, 243)), // Blue
                    "ASP.NET" => new SolidColorBrush(Color.FromRgb(76, 175, 80)), // Green
                    "JavaScript" => new SolidColorBrush(Color.FromRgb(255, 193, 7)), // Amber
                    "HTML" => new SolidColorBrush(Color.FromRgb(255, 87, 34)), // Deep Orange
                    "CSS" => new SolidColorBrush(Color.FromRgb(63, 81, 181)), // Indigo
                    _ => new SolidColorBrush(Color.FromRgb(96, 125, 139)) // Blue Grey
                };
            }
        }

        public Brush StatusColor
        {
            get
            {
                return _card?.Status switch
                {
                    CardStatus.New => new SolidColorBrush(Color.FromRgb(244, 67, 54)), // Red
                    CardStatus.Learning => new SolidColorBrush(Color.FromRgb(255, 152, 0)), // Orange
                    CardStatus.Known => new SolidColorBrush(Color.FromRgb(76, 175, 80)), // Green
                    _ => new SolidColorBrush(Color.FromRgb(158, 158, 158)) // Grey
                };
            }
        }
    }
} 