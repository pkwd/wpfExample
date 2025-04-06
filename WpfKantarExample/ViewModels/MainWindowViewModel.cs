using System.ComponentModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WpfKantarExample.Services;

namespace WpfKantarExample.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        private readonly IThemeService _themeService;
        private readonly INavigationService _navigationService;

        [ObservableProperty]
        private bool _canGoBack;

        public MainWindowViewModel(IThemeService themeService, INavigationService navigationService)
        {
            _themeService = themeService;
            _navigationService = navigationService;

            if (_navigationService is INotifyPropertyChanged notifyNavigation)
            {
                notifyNavigation.PropertyChanged += NavigationService_PropertyChanged;
            }

            UpdateBackButtonState();
        }

        [RelayCommand]
        private void ToggleTheme()
        {
            var newTheme = _themeService.CurrentTheme == ThemeType.Light 
                ? ThemeType.Dark 
                : ThemeType.Light;
            
            _themeService.SetTheme(newTheme);
        }

        [RelayCommand]
        private void GoBack()
        {
            _navigationService?.GoBack();
        }

        private void NavigationService_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(INavigationService.CanGoBack))
            {
                UpdateBackButtonState();
            }
        }

        private void UpdateBackButtonState()
        {
            CanGoBack = _navigationService?.CanGoBack ?? false;
        }

        public void Cleanup()
        {
            if (_navigationService is INotifyPropertyChanged notifyNavigation)
            {
                notifyNavigation.PropertyChanged -= NavigationService_PropertyChanged;
            }
        }
    }
} 