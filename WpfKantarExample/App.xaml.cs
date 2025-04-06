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

        public ServiceProvider Services => _serviceProvider ?? throw new InvalidOperationException("ServiceProvider not initialized");

        private ServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            // Register services
            services.AddSingleton<IThemeService, ThemeService>();
            services.AddSingleton<IStateService, StateService>();

            // Register views and viewmodels
            services.AddSingleton<MainWindow>();
            services.AddTransient<TaskListView>();
            services.AddTransient<TaskListViewModel>();

            // Register navigation service
            services.AddSingleton<INavigationService>(sp =>
            {
                var mainWindow = sp.GetRequiredService<MainWindow>();
                return new NavigationService(sp, mainWindow.MainContent);
            });

            return services.BuildServiceProvider();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                // Initialize service provider
                _serviceProvider = ConfigureServices();

                // Create and show main window first
                var mainWindow = Services.GetRequiredService<MainWindow>();
                mainWindow.Show();

                // Initialize main window services
                mainWindow.InitializeServices(Services);

                // Set initial theme
                var themeService = Services.GetRequiredService<IThemeService>();
                themeService.SetTheme(ThemeType.Light);

                // Get navigation service and navigate to initial view
                var navigationService = Services.GetRequiredService<INavigationService>();
                navigationService.NavigateTo(nameof(TaskListView));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Application failed to start: {ex.Message}", "Startup Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                Current.Shutdown();
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            _serviceProvider?.Dispose();
        }
    }
}
