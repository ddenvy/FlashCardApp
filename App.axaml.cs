using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using Avalonia.Markup.Xaml;
using QuickMind.ViewModels;
using QuickMind.Views;
using QuickMind.Services;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace QuickMind;

public partial class App : Application
{
    public static IServiceProvider ServiceProvider { get; private set; } = null!;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
        ConfigureServices();
    }

    private void ConfigureServices()
    {
        var services = new ServiceCollection();
        
        // Регистрируем сервисы
        services.AddSingleton<CardService>();
        services.AddSingleton<SpacedRepetitionService>();
        services.AddSingleton<AnkiImportService>();
        services.AddSingleton<LocalizationService>(provider => LocalizationService.Instance);
        
        ServiceProvider = services.BuildServiceProvider();
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            DisableAvaloniaDataAnnotationValidation();
            
            var localizationService = LocalizationService.Instance;
            
            var languageSelectionWindow = new LanguageSelectionWindow();
            desktop.MainWindow = languageSelectionWindow;
            languageSelectionWindow.Closed += (s, e) =>
            {
                var cardService = ServiceProvider.GetRequiredService<CardService>();
                var spacedRepetitionService = ServiceProvider.GetRequiredService<SpacedRepetitionService>();
                var ankiImportService = ServiceProvider.GetRequiredService<AnkiImportService>();
                
                var mainWindow = new MainWindow
                {
                    DataContext = new MainWindowViewModel(cardService, spacedRepetitionService, ankiImportService),
                };
                desktop.MainWindow = mainWindow;
                mainWindow.Show();
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }
}