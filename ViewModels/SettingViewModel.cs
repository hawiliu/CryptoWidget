using AutoMapper;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CryptoWidget.Services;
using CryptoWidget.Services.Dto;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace CryptoWidget.ViewModels
{
    public sealed partial class SettingViewModel : ViewModelBase
    {
        private readonly IMapper _mapper;
        private readonly string _configPath;

        public SettingViewModel(IMapper mapper)
        {
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
        private bool closeOnExit = false;

        // 支援的交易所清單
        public static List<string> SupportedExchanges { get { return ExchangeUtil.GetExchanges(); } }

        [ObservableProperty]
        private string selectedExchange = "binance";

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

        [RelayCommand]
        private void AddCrypto()
        {
            var sym = NewCryptoSymbol.Trim().ToUpper();
            if (string.IsNullOrWhiteSpace(sym)) return;
            if (!sym.Contains('/') && !sym.Contains(':')) sym += "/USDT";       // 允許輸入 BTC 自動補 /USDT
            if (!CryptoList.Contains(sym))
                CryptoList.Add(sym);
            NewCryptoSymbol = string.Empty;               // 清空輸入框
        }

        [RelayCommand]
        private void RemoveCrypto(string symbol)
        {
            if (symbol is not null) CryptoList.Remove(symbol);
        }
    }
}
