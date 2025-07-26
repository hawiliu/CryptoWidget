using CommunityToolkit.Mvvm.ComponentModel;

namespace CryptoWidget.Services
{
    public sealed partial class SettingsService : ObservableObject
    {
        private static SettingsService? _instance;
        public static SettingsService Instance => _instance ??= new SettingsService();

        [ObservableProperty]
        private double opacityLevel = 0.8;
    }
}
