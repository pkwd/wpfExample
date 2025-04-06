using System;
using System.Linq;
using System.Windows;
using System.Diagnostics;

namespace WpfKantarExample.Services
{
    public enum ThemeType
    {
        Light,
        Dark
    }

    public interface IThemeService
    {
        void SetTheme(ThemeType theme);
        ThemeType CurrentTheme { get; }
    }

    public class ThemeService : IThemeService
    {
        private ThemeType _currentTheme;

        public ThemeType CurrentTheme => _currentTheme;

        public ThemeService()
        {
            Debug.WriteLine("ThemeService: Initializing");
            // Default theme is set in App.xaml, read it if possible
            var initialTheme = Application.Current.Resources.MergedDictionaries
                .FirstOrDefault(d => d.Source?.OriginalString.Contains("Theme") ?? false);
            
            if (initialTheme?.Source?.OriginalString.Contains("Dark") ?? false)
            {
                _currentTheme = ThemeType.Dark;
            }
            else
            {
                _currentTheme = ThemeType.Light;
            }
            Debug.WriteLine($"ThemeService: Initialized with {_currentTheme} theme based on App.xaml");
        }

        public void SetTheme(ThemeType theme)
        {
            if (theme == _currentTheme) return;

            Debug.WriteLine($"ThemeService: Setting theme to {theme}");
            _currentTheme = theme;
            string themeName = theme == ThemeType.Dark ? "DarkTheme" : "LightTheme";
            
            try
            {
                var mergedDicts = Application.Current.Resources.MergedDictionaries;
                Debug.WriteLine($"ThemeService: Current merged dictionaries count: {mergedDicts.Count}");
                
                // Find and remove the current theme dictionary
                var currentThemeDict = mergedDicts
                    .FirstOrDefault(d => d.Source?.OriginalString.Contains("Theme") ?? false);
                
                if (currentThemeDict != null)
                {
                    mergedDicts.Remove(currentThemeDict);
                    Debug.WriteLine("ThemeService: Removed existing theme dictionary.");
                }
                else
                {
                    Debug.WriteLine("ThemeService: No existing theme dictionary found to remove.");
                }

                // Add the new theme dictionary
                var themeUri = new Uri($"/Themes/{themeName}.xaml", UriKind.Relative);
                Debug.WriteLine($"ThemeService: Loading theme from {themeUri}");
                
                var themeDictionary = new ResourceDictionary { Source = themeUri };
                mergedDicts.Add(themeDictionary);
                Debug.WriteLine($"ThemeService: Added new theme dictionary ({themeName}). Current count: {mergedDicts.Count}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ThemeService: Error setting theme - {ex.Message}");
                Debug.WriteLine($"ThemeService: Stack trace - {ex.StackTrace}");
            }
        }
    }
} 