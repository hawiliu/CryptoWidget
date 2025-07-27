using Avalonia.Controls;
using CryptoWidget.ViewModels;

namespace CryptoWidget
{
    public partial class MainWindow : Window
    {
        public MainWindow(MainViewModel mainViewModel)
        {
            InitializeComponent();
            DataContext = mainViewModel;

            this.PointerPressed += (_, e) =>
            {
                if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
                    BeginMoveDrag(e);
            };
        }

        // �������s
        private void CloseButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
            => Close();
    }
}