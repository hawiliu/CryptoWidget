using Avalonia.Controls;
using CryptoWidget.ViewModels;
using System.ComponentModel;

namespace CryptoWidget
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

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

        private void CloseButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (DataContext is MainViewModel vm)
            {
                if (vm._settingWindow != null)
                    vm._settingWindow.Close();

                if (vm._aboutWindow != null)
                    vm._aboutWindow.Close();

                if (vm.Settings.CloseOnExit)
                {
                    Close(); // 完全關閉應用程式
                }
                else
                {
                    Hide(); // 隱藏到系統匣
                }
            }
        }
    }
}