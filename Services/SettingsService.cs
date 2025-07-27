using AutoMapper;
using CommunityToolkit.Mvvm.ComponentModel;
using CryptoWidget.Services.Dto;
using System;
using System.Collections.Generic;
using System.IO;
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
        private List<string> cryptoList = new List<string>();

        public async Task LoadAsync()
        {
            if (!File.Exists(_configPath)) return;

            try
            {
                var json = await File.ReadAllTextAsync(_configPath);
                var dto = JsonSerializer.Deserialize<SettingsDto>(json);

                if (dto is not null)
                    OpacityLevel = dto.OpacityLevel;
            }
            catch { /* 讀檔失敗時使用預設值 */ }
        }

        private async Task SaveAsync()
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(_configPath)!);
                var dto = new SettingsDto { OpacityLevel = OpacityLevel };
                var json = JsonSerializer.Serialize(dto, new JsonSerializerOptions { WriteIndented = true });
                await File.WriteAllTextAsync(_configPath, json);
            }
            catch { /* 寫檔失敗可略過或另行記錄 */ }
        }

        partial void OnOpacityLevelChanged(double oldValue, double newValue) => _ = SaveAsync();

        
    }
}
