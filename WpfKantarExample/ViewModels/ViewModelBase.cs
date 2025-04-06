using CommunityToolkit.Mvvm.ComponentModel;

namespace WpfKantarExample.ViewModels
{
    public abstract partial class ViewModelBase : ObservableObject
    {
        [ObservableProperty]
        private bool _isBusy;

        [ObservableProperty]
        private string _title = string.Empty;

        public virtual void OnNavigatedTo()
        {
        }
    }
} 