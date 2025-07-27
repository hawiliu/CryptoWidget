using Avalonia.Controls;
using CryptoWidget.ViewModels;
using System.ComponentModel;

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

            // 監聽 KeepOnTop 屬性變化
            if (mainViewModel.Settings != null)
            {
                mainViewModel.Settings.PropertyChanged += OnSettingsPropertyChanged;
                // 初始化 Topmost 狀態
                this.Topmost = mainViewModel.Settings.KeepOnTop;
            }
        }

        private void OnSettingsPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Services.SettingsService.KeepOnTop) && sender is Services.SettingsService settings)
            {
                this.Topmost = settings.KeepOnTop;
            }
        }

        private void CloseButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (DataContext is MainViewModel vm && vm._settingWindow != null)
            { 
                vm._settingWindow.Close();
            }

            Close();
        }
    }
}