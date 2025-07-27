using AutoMapper;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CryptoWidget.Services.Dto;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace CryptoWidget.Services
{
    public sealed partial class SettingsService : ObservableObject
    {
        private readonly IMapper _mapper;
        private readonly string _configPath;

        public SettingsService(IMapper mapper) {
            _mapper = mapper;
            _configPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "CryptoWidget", "settings.json");
        }

        [ObservableProperty]
        private double opacityLevel = 0.8;

        [ObservableProperty]
        private bool keepOnTop = false;

        [ObservableProperty]
        private ObservableCollection<string> cryptoList = new ObservableCollection<string>() { "BTC/USDT" };

        [ObservableProperty] 
        private string newCryptoSymbol = string.Empty;

        public async Task LoadAsync()
        {
            if (!File.Exists(_configPath))
                return;

            var dto = JsonSerializer.Deserialize<SettingsDto>(
                await File.ReadAllTextAsync(_configPath));

            if (dto != null)
                _mapper.Map(dto, this);
        }

        public async Task SaveAsync()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_configPath)!);

            var dto = _mapper.Map<SettingsDto>(this);

            await File.WriteAllTextAsync(
                _configPath,
                JsonSerializer.Serialize(dto, new JsonSerializerOptions { WriteIndented = true }));
        }

        [RelayCommand]                // AddCryptoCommand
        private void AddCrypto()
        {
            var sym = NewCryptoSymbol.Trim().ToUpper();
            if (string.IsNullOrWhiteSpace(sym)) return;
            if (!sym.Contains('/')) sym += "/USDT";       // 允許輸入 BTC 自動補 /USDT
            if (!CryptoList.Contains(sym))
                CryptoList.Add(sym);
            NewCryptoSymbol = string.Empty;               // 清空輸入框
        }

        [RelayCommand]                // RemoveCryptoCommand (CommandParameter 傳入要刪的字串)
        private void RemoveCrypto(string symbol)
        {
            if (symbol is not null) CryptoList.Remove(symbol);
        }

        // 移除透明度改變時的自動保存，改為手動保存
        // partial void OnOpacityLevelChanged(double oldValue, double newValue) => _ = SaveAsync();
    }
}
