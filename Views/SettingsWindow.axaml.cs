using Avalonia.Controls;
using CryptoWidget.Services;
using System.Collections.ObjectModel;

namespace CryptoWidget
{
    public partial class SettingsWindow : Window
    {
        public SettingsWindow() { 
            InitializeComponent();
        }
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

        private void CloseButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
            => Close();

        private void RemoveCrypto_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is string symbol && DataContext is SettingsService settings)
            {
                settings.RemoveCryptoCommand.Execute(symbol);
            }
        }

        private async void SaveButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (DataContext is SettingsService settings)
            {
                // 保存設定
                await settings.SaveAsync();
                Close();
            }
        }

        private async void CancelButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (DataContext is SettingsService settings)
            {
                // 還原設定
                await settings.LoadAsync();

                Close();
            }
        }
    }
}