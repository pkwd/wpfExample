using System.Diagnostics;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using WpfKantarExample.Services;
using System.ComponentModel;

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
            // Ensure DataContext is set for potential bindings
            DataContext = this;
        }

        public void InitializeServices(IServiceProvider serviceProvider)
        {
            Debug.WriteLine("Initializing MainWindow services");
            _themeService = serviceProvider.GetRequiredService<IThemeService>();
            _navigationService = serviceProvider.GetRequiredService<INavigationService>();

            // Subscribe to navigation service property changes
            if (_navigationService is INotifyPropertyChanged notifyNavigation)
            {
                notifyNavigation.PropertyChanged += NavigationService_PropertyChanged;
            }

            // Update back button state initially
            UpdateBackButtonState();
            Debug.WriteLine("MainWindow services initialized");
        }

        private void NavigationService_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(INavigationService.CanGoBack))
            {
                // Ensure UI updates happen on the UI thread
                Dispatcher.Invoke(UpdateBackButtonState);
            }
        }

        private void UpdateBackButtonState()
        {
            BackButton.IsEnabled = _navigationService?.CanGoBack ?? false;
            Debug.WriteLine($"Back button IsEnabled set to: {BackButton.IsEnabled}");
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
            Debug.WriteLine("Back button clicked");
            _navigationService?.GoBack();
        }

        // Unsubscribe when the window closes
        protected override void OnClosed(EventArgs e)
        {
            if (_navigationService is INotifyPropertyChanged notifyNavigation)
            {
                notifyNavigation.PropertyChanged -= NavigationService_PropertyChanged;
            }
            base.OnClosed(e);
        }
    }
}