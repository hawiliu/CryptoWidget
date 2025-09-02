using AutoMapper;
using CryptoWidget.Services.Dto;
using CryptoWidget.ViewModels;

namespace CryptoWidget.Services.AutoMapper
{
    public class SettingsProfile : Profile
    {
        public SettingsProfile()
        {
            // 雙向對應：OpacityLevel 會自動匹配
            CreateMap<SettingViewModel, SettingsDto>().ReverseMap();
        }
    }
}
