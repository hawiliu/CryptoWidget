using Avalonia.Controls;
using CryptoWidget.Services;

namespace CryptoWidget
{
    public partial class AboutWindow : Window
    {
        public AboutWindow(SettingsService settings)
        {
            InitializeComponent();
            DataContext = settings;

            this.PointerPressed += (_, e) =>
            {
                if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
                    BeginMoveDrag(e);
            };
        }

        private void CloseButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            Close();
        }
    }
} 