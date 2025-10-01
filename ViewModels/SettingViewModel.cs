using AutoMapper;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CryptoWidget.Services;
using CryptoWidget.Services.Dto;
using Lang.Avalonia;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
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

        // -- 一般設定 --
        // 語系選擇
        [ObservableProperty]
        private string selectedLanguage = "en";

        // 當前選中的語系項目
        [ObservableProperty]
        private LanguageOption? selectedLanguageItem;

        public void SelectedLanguageItemChanged(LanguageOption? value)
        {
            if (value != null && value.Code != SelectedLanguage)
            {
                SelectedLanguage = value.Code;
                I18nManager.Instance.Culture = new CultureInfo(SelectedLanguage);
            }
        }

        // 支援的語系清單
        public static List<LanguageOption> SupportedLanguages { get; } = new List<LanguageOption>
        {
            new LanguageOption { Code = "en", DisplayName = "English" },
            new LanguageOption { Code = "zh-tw", DisplayName = "繁體中文" },
            new LanguageOption { Code = "zh-cn", DisplayName = "简体中文" }
        };

        [ObservableProperty]
        private bool keepOnTop = false;

        [ObservableProperty]
        private bool closeOnExit = false;

        // -- 交易所設定 --
        // 支援的交易所清單
        public static List<string> SupportedExchanges { get { return ExchangeUtil.GetExchanges(); } }

        [ObservableProperty]
        private string selectedExchange = "binance";

        [ObservableProperty]
        private string exchangeApiKey = string.Empty;

        [ObservableProperty]
        private string exchangeApiSecret = string.Empty;

        [ObservableProperty]
        private ObservableCollection<string> cryptoList = new ObservableCollection<string>() { "BTC/USDT" };

        [ObservableProperty]
        private string newCryptoSymbol = string.Empty;

        // -- 外觀設定 --
        [ObservableProperty]
        private double opacityLevel = 0.8;

        [ObservableProperty]
        private bool showChart = true;

        [ObservableProperty]
        private bool showTemporaryInput = true;

        [ObservableProperty]
        private double symbolFontSize = 16;

        [ObservableProperty]
        private double priceFontSize = 14;

        public async Task LoadAsync()
        {
            if (!File.Exists(_configPath))
                return;

            var dto = JsonSerializer.Deserialize<SettingsDto>(
                await File.ReadAllTextAsync(_configPath));

            if (dto != null)
            {
                _mapper.Map(dto, this);

                // 設定選中的語系項目
                SelectedLanguageItem = SupportedLanguages.FirstOrDefault(l => l.Code == SelectedLanguage);

                // 載入語系設定後，立即套用到應用程式
                if (!string.IsNullOrEmpty(SelectedLanguage))
                {
                    I18nManager.Instance.Culture = new CultureInfo(SelectedLanguage);
                }
            }
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

        [RelayCommand]
        private void MoveUp(string symbol)
        {
            if (symbol is null) return;
            var index = CryptoList.IndexOf(symbol);
            if (index > 0)
            {
                CryptoList.Move(index, index - 1);
            }
        }

        [RelayCommand]
        private void MoveDown(string symbol)
        { 
            if (symbol is null) return;
            var index = CryptoList.IndexOf(symbol);
            if (index < CryptoList.Count - 1 && index != -1)
            {
                CryptoList.Move(index, index + 1);
            }
        }
    }
}
