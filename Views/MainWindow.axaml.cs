using Avalonia.Controls;

namespace CryptoWidget
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.PointerPressed += (_, e) =>
            {
                if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
                    BeginMoveDrag(e);
            };
        }

        // Ãö³¬«ö¶s
        private void CloseButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
            => Close();
    }
}