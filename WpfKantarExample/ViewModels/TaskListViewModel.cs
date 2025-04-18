using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WpfKantarExample.Models;
using WpfKantarExample.Services;
using WpfKantarExample.Views;

namespace WpfKantarExample.ViewModels
{
    public partial class TaskListViewModel : ViewModelBase
    {
        private readonly IStateService _stateService;
        private readonly INavigationService _navigationService;
        private const string STATE_KEY = "tasks";

        [ObservableProperty]
        private ObservableCollection<TaskItem> _tasks = new();

        [ObservableProperty]
        private string _newTaskTitle = string.Empty;

        public TaskListViewModel(IStateService stateService, INavigationService navigationService)
        {
            _stateService = stateService;
            _navigationService = navigationService;
            Title = "Task List";
            LoadTasks();
        }

        private async void LoadTasks()
        {
            await Application.Current.Dispatcher.InvokeAsync(async () =>
            {
                await LoadTasksAsync();
            });
        }

        [RelayCommand]
        private async Task AddTask()
        {
            if (string.IsNullOrWhiteSpace(NewTaskTitle))
                return;

            var task = new TaskItem
            {
                Title = NewTaskTitle,
                CreatedAt = DateTime.Now
            };

            Tasks.Add(task);
            NewTaskTitle = string.Empty;
            await SaveTasksAsync();
        }

        [RelayCommand]
        private async Task ToggleTaskComplete(TaskItem task)
        {
            task.IsCompleted = !task.IsCompleted;
            task.CompletedAt = task.IsCompleted ? DateTime.Now : null;
            await SaveTasksAsync();
        }

        [RelayCommand]
        private void DeleteTask(TaskItem task)
        {
            if (task != null)
            {
                _stateService.DeleteTask(task);
                Tasks.Remove(task);
            }
        }

        [RelayCommand]
        private void SwitchToGridView()
        {
            _navigationService.NavigateTo<TaskGridView>();
        }

        private async Task LoadTasksAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                var tasks = await _stateService.LoadStateAsync<List<TaskItem>>(STATE_KEY);
                
                Tasks.Clear();
                if (tasks != null)
                {
                    foreach (var task in tasks)
                    {
                        Tasks.Add(task);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading tasks: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task SaveTasksAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                await _stateService.SaveStateAsync(STATE_KEY, Tasks.ToList());
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
} 