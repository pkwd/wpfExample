using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WpfKantarExample.Models;
using WpfKantarExample.Services;
using WpfKantarExample.Views;

namespace WpfKantarExample.ViewModels
{
    public partial class TaskGridViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IStateService _stateService;
        private const string STATE_KEY = "tasks";

        [ObservableProperty]
        private ObservableCollection<TaskItem> _tasks = new();

        [ObservableProperty]
        private bool _isBusy;

        [ObservableProperty]
        private string _title = "Task Grid";

        public TaskGridViewModel(INavigationService navigationService, IStateService stateService)
        {
            _navigationService = navigationService;
            _stateService = stateService;
            LoadTasksAsync().ConfigureAwait(false);
        }

        [RelayCommand]
        private async Task LoadTasks()
        {
            await LoadTasksAsync();
        }

        [RelayCommand]
        private async Task AddTask()
        {
            var task = new TaskItem
            {
                Title = "New Task",
                CreatedAt = DateTime.Now
            };

            Tasks.Add(task);
            await SaveTasksAsync();
        }

        [RelayCommand]
        private async Task DeleteTask(TaskItem task)
        {
            if (task != null)
            {
                Tasks.Remove(task);
                await SaveTasksAsync();
            }
        }

        [RelayCommand]
        private async Task ToggleTaskComplete(TaskItem task)
        {
            if (task != null)
            {
                task.IsCompleted = !task.IsCompleted;
                task.CompletedAt = task.IsCompleted ? DateTime.Now : null;
                await SaveTasksAsync();
            }
        }

        [RelayCommand]
        private void SwitchToListView()
        {
            _navigationService.NavigateTo<TaskListView>();
        }

        private async Task LoadTasksAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                var tasks = await _stateService.LoadStateAsync<List<TaskItem>>(STATE_KEY);
                
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    Tasks.Clear();
                    if (tasks != null)
                    {
                        foreach (var task in tasks)
                        {
                            Tasks.Add(task);
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    MessageBox.Show($"Error loading tasks: {ex.Message}", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                });
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task SaveTasksAsync()
        {
            try
            {
                IsBusy = true;
                await _stateService.SaveStateAsync(STATE_KEY, Tasks.ToList());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving tasks: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public override async void OnNavigatedTo()
        {
            await LoadTasks();
        }
    }
} 