using System;
using System.Configuration;
using System.Data;
using System.IO;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WpfKantarExample.Data;
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
            // Register database context
            var dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tasks.db");
            services.AddDbContextFactory<ApplicationDbContext>(options =>
                options.UseSqlite($"Data Source={dbPath}"));

            // Register services
            services.AddSingleton<IStateService, DbStateService>();
            services.AddSingleton<IThemeService, ThemeService>();
            services.AddSingleton<INavigationService>(sp => 
                new NavigationService(sp, mainWindow.MainContent));

            // Register views
            services.AddTransient<TaskListView>();
            services.AddTransient<TaskGridView>();

            // Register view models
            services.AddTransient<TaskListViewModel>();
            services.AddTransient<TaskGridViewModel>();
            services.AddSingleton<MainWindowViewModel>();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _serviceProvider?.Dispose();
            base.OnExit(e);
        }
    }
}
