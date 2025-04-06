using NUnit.Framework;
using WpfKantarExample.Services;

namespace WpfKantarExample.Tests
{
    [TestFixture]
    public class ThemeServiceTests
    {
        private IThemeService _themeService = null!;

        [SetUp]
        public void Setup()
        {
            _themeService = new ThemeService();
        }

        [Test]
        public void SetTheme_WhenCalled_UpdatesCurrentTheme()
        {
            // Arrange
            var expectedTheme = ThemeType.Dark;

            // Act
            _themeService.SetTheme(expectedTheme);

            // Assert
            Assert.That(_themeService.CurrentTheme, Is.EqualTo(expectedTheme));
        }

        [Test]
        public void CurrentTheme_DefaultValue_IsLight()
        {
            // Assert
            Assert.That(_themeService.CurrentTheme, Is.EqualTo(ThemeType.Light));
        }
    }
}
