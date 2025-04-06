using System.Diagnostics;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using WpfKantarExample.Services;

namespace WpfKantarExample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IThemeService? _themeService;
        private INavigationService? _navigationService;

        public MainWindow()
        {
            InitializeComponent();
            Debug.WriteLine("MainWindow created");
            Debug.WriteLine($"MainContent control name: {MainContent.Name}");
        }

        public void InitializeServices(IServiceProvider serviceProvider)
        {
            Debug.WriteLine("Initializing MainWindow services");
            _themeService = serviceProvider.GetRequiredService<IThemeService>();
            _navigationService = serviceProvider.GetRequiredService<INavigationService>();

            // Update back button visibility
            UpdateBackButtonState();
            Debug.WriteLine("MainWindow services initialized");
        }

        private void UpdateBackButtonState()
        {
            if (_navigationService != null)
            {
                BackButton.IsEnabled = _navigationService.CanGoBack;
            }
        }

        private void ThemeToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if (_themeService == null)
            {
                Debug.WriteLine("Theme service not initialized");
                return;
            }

            Debug.WriteLine("Theme toggle clicked");
            var newTheme = _themeService.CurrentTheme == ThemeType.Light 
                ? ThemeType.Dark 
                : ThemeType.Light;
            
            _themeService.SetTheme(newTheme);
            Debug.WriteLine($"Theme changed to: {newTheme}");
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (_navigationService?.CanGoBack == true)
            {
                _navigationService.GoBack();
                UpdateBackButtonState();
                Debug.WriteLine("Navigated back");
            }
        }
    }
}