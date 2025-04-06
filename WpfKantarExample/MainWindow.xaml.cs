using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using WpfKantarExample.ViewModels;

namespace WpfKantarExample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindowViewModel? _viewModel;

        public MainWindow()
        {
            InitializeComponent();
        }

        public void InitializeServices(IServiceProvider serviceProvider)
        {
            _viewModel = serviceProvider.GetRequiredService<MainWindowViewModel>();
            DataContext = _viewModel;
        }

        protected override void OnClosed(EventArgs e)
        {
            _viewModel?.Cleanup();
            base.OnClosed(e);
        }
    }
}