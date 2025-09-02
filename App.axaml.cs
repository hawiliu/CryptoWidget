using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using CryptoWidget.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace CryptoWidget
{
    public partial class App : Application
    {
        public static IServiceProvider? Services { get; set; }

        private TrayIcon? _trayIcon;      // 保留參考以便 Dispose

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
                // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
                DisableAvaloniaDataAnnotationValidation();

                desktop.ShutdownMode = ShutdownMode.OnMainWindowClose;
                desktop.Exit += OnExit;

                // 取出 XAML 產生的第一顆 TrayIcon
                var icons = TrayIcon.GetIcons(this);
                if (icons?.Count > 0)
                    _trayIcon = icons[0];

                desktop.MainWindow = Services!.GetRequiredService<MainWindow>();
            }

            base.OnFrameworkInitializationCompleted();
        }

        private void DisableAvaloniaDataAnnotationValidation()
        {
            // Get an array of plugins to remove
            var dataValidationPluginsToRemove =
                BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

            // remove each entry found
            foreach (var plugin in dataValidationPluginsToRemove)
            {
                BindingPlugins.DataValidators.Remove(plugin);
            }
        }

        // 雙擊檢測變數
        private DateTime _lastClickTime = DateTime.MinValue;
        private const int DOUBLE_CLICK_TIME_MS = 500; // 雙擊時間間隔（毫秒）

        // TrayIcon 雙擊事件處理
        private void TrayIcon_Clicked(object? sender, EventArgs e)
        {
            var currentTime = DateTime.Now;
            var timeDiff = (currentTime - _lastClickTime).TotalMilliseconds;

            if (timeDiff <= DOUBLE_CLICK_TIME_MS)
            {
                // 雙擊 - 顯示主視窗
                ShowMainWindow_Click(sender, e);
                _lastClickTime = DateTime.MinValue; // 重置，避免連續觸發
            }
            else
            {
                // 單擊 - 記錄時間
                _lastClickTime = currentTime;
            }
        }

        // TrayIcon 事件處理
        private void ShowMainWindow_Click(object? sender, EventArgs e)
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow?.Show();
                desktop.MainWindow?.Activate();
            }
        }

        private void OpenSettings_Click(object? sender, EventArgs e)
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var mainViewModel = desktop.MainWindow?.DataContext as MainViewModel;
                mainViewModel?.OpenSettingsCommand.Execute(null);
            }
        }

        private void OpenAbout_Click(object? sender, EventArgs e)
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var mainViewModel = desktop.MainWindow?.DataContext as MainViewModel;
                mainViewModel?.OpenAboutCommand.Execute(null);
            }
        }

        private void Exit_Click(object? sender, EventArgs e)
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.Shutdown();
            }
        }

        private void OnExit(object? sender, ControlledApplicationLifetimeExitEventArgs e)
        {
            _trayIcon?.Dispose();
        }
    }
}