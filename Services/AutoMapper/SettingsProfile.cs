using AutoMapper;
using CryptoWidget.Services.Dto;

namespace CryptoWidget.Services.AutoMapper
{
    public class SettingsProfile : Profile
    {
        public SettingsProfile()
        {
            // 雙向對應：OpacityLevel 會自動匹配
            CreateMap<SettingsService, SettingsDto>().ReverseMap();
        }
    }
}
