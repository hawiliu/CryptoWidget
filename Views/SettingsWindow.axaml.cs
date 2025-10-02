using Avalonia.Controls;
using CryptoWidget.Services.Dto;
using CryptoWidget.ViewModels;

namespace CryptoWidget
{
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
        }

        public SettingsWindow(SettingViewModel settings)
        {
            InitializeComponent();
            DataContext = settings;

            this.PointerPressed += (_, e) =>
            {
                var point = e.GetCurrentPoint(this);
                if (point.Properties.IsLeftButtonPressed)
                {
                    // 檢查點擊的來源是否為控制項（如 ComboBox、Button 等）
                    var source = e.Source;
                    if (source is Avalonia.Controls.Control control)
                    {
                        // 如果是 ComboBox 或其子元素，不處理拖拽
                        if (IsComboBoxOrChild(control))
                            return;
                    }

                    BeginMoveDrag(e);
                }
            };
        }


        private async void SaveButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (DataContext is SettingViewModel settings)
            {
                // 保存設定
                await settings.SaveAsync();
                Close();
            }
        }

        private async void CancelButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (DataContext is SettingViewModel settings)
            {
                // 還原設定
                await settings.LoadAsync();

                Close();
            }
        }

        private void OnSelectedLanguageChanged(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (DataContext is SettingViewModel settings && sender is ComboBox comboBox)
            {
                settings.SelectedLanguageItemChanged((LanguageOption)comboBox.SelectedValue);
            }
        }

        // 檢查控制項是否為 ComboBox 或其子元素
        private bool IsComboBoxOrChild(Avalonia.Controls.Control control)
        {
            var current = control;
            while (current != null)
            {
                if (current is ComboBox || current is Button || current is CheckBox || current is RadioButton)
                    return true;

                current = current.Parent as Avalonia.Controls.Control;
            }
            return false;
        }
    }
}