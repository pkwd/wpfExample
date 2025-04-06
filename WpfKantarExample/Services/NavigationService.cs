using System;
using System.Collections.Generic;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using WpfKantarExample.Views;
using WpfKantarExample.ViewModels;
using System.Diagnostics;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WpfKantarExample.Services
{
    public interface INavigationService : INotifyPropertyChanged
    {
        void NavigateTo<T>() where T : UserControl;
        void NavigateTo(string viewName);
        void GoBack();
        bool CanGoBack { get; }
    }

    public class NavigationService : INavigationService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Stack<UserControl> _navigationStack = new();
        private readonly ContentControl _contentControl;

        public event PropertyChangedEventHandler? PropertyChanged;

        public NavigationService(IServiceProvider serviceProvider, ContentControl contentControl)
        {
            Debug.WriteLine("NavigationService created");
            _serviceProvider = serviceProvider;
            _contentControl = contentControl ?? throw new ArgumentNullException(nameof(contentControl));
            Debug.WriteLine($"ContentControl reference: {_contentControl.Name}");
        }

        public bool CanGoBack => _navigationStack.Count > 1;

        public void NavigateTo<T>() where T : UserControl
        {
            try
            {
                var view = _serviceProvider.GetRequiredService<T>();
                var viewModelType = GetViewModelType<T>();
                var viewModel = _serviceProvider.GetRequiredService(viewModelType) as ViewModelBase;

                if (viewModel != null)
                {
                    view.DataContext = viewModel;
                    viewModel.OnNavigatedTo();
                }

                _navigationStack.Push(view);
                _contentControl.Content = view;
                OnPropertyChanged(nameof(CanGoBack)); // Notify CanGoBack changed
                Debug.WriteLine($"Successfully navigated to {typeof(T).Name}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Navigation error: {ex}");
                throw;
            }
        }

        private Type GetViewModelType<T>() where T : UserControl
        {
            var viewName = typeof(T).Name;
            var viewModelName = viewName.Replace("View", "ViewModel");
            var viewModelType = Type.GetType($"WpfKantarExample.ViewModels.{viewModelName}, WpfKantarExample");
            
            if (viewModelType == null)
                throw new InvalidOperationException($"Could not find ViewModel for {viewName}");
            
            return viewModelType;
        }

        public void NavigateTo(string viewName)
        {
            Debug.WriteLine($"Attempting to navigate to: {viewName}");

            try
            {
                UserControl? view = null;
                ViewModelBase? viewModel = null;

                if (viewName == nameof(TaskListView))
                {
                    view = _serviceProvider.GetRequiredService<TaskListView>();
                    viewModel = _serviceProvider.GetRequiredService<TaskListViewModel>();
                }
                else if (viewName == nameof(TaskGridView))
                {
                    view = _serviceProvider.GetRequiredService<TaskGridView>();
                    viewModel = _serviceProvider.GetRequiredService<TaskGridViewModel>();
                }
                else
                {
                    throw new ArgumentException($"Unknown view: {viewName}");
                }

                if (view == null)
                {
                    throw new InvalidOperationException($"Could not create view: {viewName}");
                }

                if (viewModel != null)
                {
                    view.DataContext = viewModel;
                    viewModel.OnNavigatedTo();
                }

                _navigationStack.Push(view);
                _contentControl.Content = view;
                OnPropertyChanged(nameof(CanGoBack)); // Notify CanGoBack changed
                Debug.WriteLine($"Successfully navigated to {viewName}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Navigation error: {ex}");
                throw;
            }
        }

        public void GoBack()
        {
            if (!CanGoBack) return;
            
            _navigationStack.Pop(); // Remove current view
            var previousView = _navigationStack.Peek();
            _contentControl.Content = previousView;

            if (previousView.DataContext is ViewModelBase viewModel)
            {
                viewModel.OnNavigatedTo();
            }

            OnPropertyChanged(nameof(CanGoBack)); // Notify CanGoBack changed
            Debug.WriteLine("Navigated back");
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
} 