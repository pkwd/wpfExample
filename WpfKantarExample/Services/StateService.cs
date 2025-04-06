using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using WpfKantarExample.Models;

namespace WpfKantarExample.Services
{
    public interface IStateService
    {
        Task SaveStateAsync<T>(string key, T state);
        Task<T?> LoadStateAsync<T>(string key);
        void ClearState(string key);
        void DeleteTask(TaskItem task);
    }

    public class StateService : IStateService
    {
        private readonly string _stateDirectory;
        private const string TASKS_KEY = "tasks";

        public StateService()
        {
            _stateDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "WpfKantarExample",
                "State");
            
            Directory.CreateDirectory(_stateDirectory);
        }

        private string GetStateFilePath(string key) => 
            Path.Combine(_stateDirectory, $"{key}.json");

        public async Task SaveStateAsync<T>(string key, T state)
        {
            var filePath = GetStateFilePath(key);
            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(state, options);
            await File.WriteAllTextAsync(filePath, json);
        }

        public async Task<T?> LoadStateAsync<T>(string key)
        {
            var filePath = GetStateFilePath(key);
            if (!File.Exists(filePath))
                return default;

            try
            {
                var json = await File.ReadAllTextAsync(filePath);
                if (string.IsNullOrWhiteSpace(json))
                    return default;

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                return JsonSerializer.Deserialize<T>(json, options);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading state: {ex.Message}");
                return default;
            }
        }

        public void ClearState(string key)
        {
            var filePath = GetStateFilePath(key);
            if (File.Exists(filePath))
                File.Delete(filePath);
        }

        public async void DeleteTask(TaskItem task)
        {
            try
            {
                var tasks = await LoadStateAsync<List<TaskItem>>(TASKS_KEY);
                if (tasks != null)
                {
                    tasks.RemoveAll(t => t.Title == task.Title && t.CreatedAt == task.CreatedAt);
                    await SaveStateAsync(TASKS_KEY, tasks);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error deleting task: {ex.Message}");
            }
        }
    }
} 