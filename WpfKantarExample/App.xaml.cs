using System;
using System.Configuration;
using System.Data;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using WpfKantarExample.Services;
using WpfKantarExample.ViewModels;
using WpfKantarExample.Views;

namespace WpfKantarExample
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private ServiceProvider? _serviceProvider;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var services = new ServiceCollection();

            // Create main window first
            var mainWindow = new MainWindow();

            // Configure services including navigation service
            ConfigureServices(services, mainWindow);

            _serviceProvider = services.BuildServiceProvider();

            // Initialize main window services
            mainWindow.InitializeServices(_serviceProvider);
            
            // Show window and navigate to initial view
            mainWindow.Show();
            var navigationService = _serviceProvider.GetRequiredService<INavigationService>();
            navigationService.NavigateTo<TaskListView>();
        }

        private void ConfigureServices(IServiceCollection services, MainWindow mainWindow)
        {
            // Register services
            services.AddSingleton<IStateService, StateService>();
            services.AddSingleton<IThemeService, ThemeService>();
            services.AddSingleton<INavigationService>(sp => 
                new NavigationService(sp, mainWindow.MainContent));

            // Register views
            services.AddTransient<TaskListView>();
            services.AddTransient<TaskGridView>();

            // Register view models
            services.AddTransient<TaskListViewModel>();
            services.AddTransient<TaskGridViewModel>();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _serviceProvider?.Dispose();
            base.OnExit(e);
        }
    }
}
