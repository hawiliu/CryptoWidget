using Avalonia.Controls;
using CryptoWidget.Services;

namespace CryptoWidget
{
    public partial class SettingsWindow : Window
    {
        public SettingsWindow(SettingsService settings)
        {
            InitializeComponent();
            DataContext = settings;

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