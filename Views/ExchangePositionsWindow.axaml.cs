using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using CryptoWidget.ViewModels;

namespace CryptoWidget
{
    public partial class ExchangePositionsWindow : Window
    {
        public ExchangePositionsWindow()
        {
            InitializeComponent();
        }

        public ExchangePositionsWindow(SettingViewModel settings)
        {
            InitializeComponent();

            DataContext = new ExchangePositionsViewModel(settings);

            this.PointerPressed += (_, e) =>
            {
                if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
                    BeginMoveDrag(e);
            };
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void CloseButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            Close();
        }
    }
}