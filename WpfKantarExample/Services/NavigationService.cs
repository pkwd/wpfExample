using System;
using System.Collections.Generic;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using WpfKantarExample.Views;
using WpfKantarExample.ViewModels;
using System.Diagnostics;

namespace WpfKantarExample.Services
{
    public interface INavigationService
    {
        void NavigateTo(string viewName);
        void GoBack();
        bool CanGoBack { get; }
    }

    public class NavigationService : INavigationService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Stack<UserControl> _navigationStack = new();
        private readonly ContentControl _contentControl;

        public NavigationService(IServiceProvider serviceProvider, ContentControl contentControl)
        {
            Debug.WriteLine("NavigationService created");
            _serviceProvider = serviceProvider;
            _contentControl = contentControl ?? throw new ArgumentNullException(nameof(contentControl));
            Debug.WriteLine($"ContentControl reference: {_contentControl.Name}");
        }

        public bool CanGoBack => _navigationStack.Count > 1;

        public void NavigateTo(string viewName)
        {
            Debug.WriteLine($"Attempting to navigate to: {viewName}");

            try
            {
                // Try to get the view directly from DI first
                UserControl? view = null;
                if (viewName == nameof(TaskListView))
                {
                    view = _serviceProvider.GetRequiredService<TaskListView>();
                    var viewModel = _serviceProvider.GetRequiredService<TaskListViewModel>();
                    view.DataContext = viewModel;
                    Debug.WriteLine($"Created view {viewName} with ViewModel {viewModel.GetType().Name}");
                }
                else
                {
                    throw new ArgumentException($"Unknown view: {viewName}");
                }

                if (view == null)
                {
                    throw new InvalidOperationException($"Could not create view: {viewName}");
                }

                _navigationStack.Push(view);
                _contentControl.Content = view;
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
            Debug.WriteLine("Navigated back");
        }
    }
} 