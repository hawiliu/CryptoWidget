using Avalonia;
using CryptoWidget.Services.AutoMapper;
using CryptoWidget.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;

namespace CryptoWidget
{
    internal sealed class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        [STAThread]
        public static async Task Main(string[] args)
        {
            // 建立 Generic Host
            using var host = Host.CreateDefaultBuilder(args)
                .ConfigureServices(services =>
                {
                    // AutoMapper：掃描所有 Profile
                    services.AddAutoMapper(cfg =>
                    {
                        cfg.AddProfile<SettingsProfile>();
                    });

                    services.AddSingleton<MainViewModel>();
                    services.AddSingleton<SettingViewModel>();

                    // Windows
                    services.AddSingleton<MainWindow>();      // 讓 DI 負責 MainWindow
                })
                .Build();

            // 啟動背景服務
            await host.StartAsync();

            host.Services.GetRequiredService<SettingViewModel>()
                     .LoadAsync().GetAwaiter().GetResult();

            App.Services = host.Services;

            BuildAvaloniaApp()
                .StartWithClassicDesktopLifetime(args);

            // UI 全關→回來；停止並釋放
            await host.StopAsync();
        }

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .WithInterFont()
                .LogToTrace();
    }
}
