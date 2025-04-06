using System;
using System.Windows;

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

        public void SetTheme(ThemeType theme)
        {
            _currentTheme = theme;
            string themeName = theme == ThemeType.Dark ? "DarkTheme" : "LightTheme";
            
            Application.Current.Resources.MergedDictionaries.Clear();
            Application.Current.Resources.MergedDictionaries.Add(
                new ResourceDictionary
                {
                    Source = new Uri($"/Themes/{themeName}.xaml", UriKind.Relative)
                });
        }
    }
} 