using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using WpfKantarExample.Data;
using WpfKantarExample.Models;

namespace WpfKantarExample.Services
{
    public class DbStateService : IStateService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

        public DbStateService(IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task SaveStateAsync<T>(string key, T state)
        {
            if (typeof(T) != typeof(List<TaskItem>))
                throw new ArgumentException("Only TaskItem lists are supported");

            var tasks = state as List<TaskItem>;
            if (tasks == null) return;

            using var context = await _contextFactory.CreateDbContextAsync();
            // Clear existing tasks
            context.Tasks.RemoveRange(await context.Tasks.ToListAsync());
            // Add new tasks
            await context.Tasks.AddRangeAsync(tasks);
            await context.SaveChangesAsync();
        }

        public async Task<T?> LoadStateAsync<T>(string key)
        {
            if (typeof(T) != typeof(List<TaskItem>))
                throw new ArgumentException("Only TaskItem lists are supported");

            using var context = await _contextFactory.CreateDbContextAsync();
            var tasks = await context.Tasks.ToListAsync();
            return (T)(object)tasks;
        }

        public async void DeleteTask(TaskItem task)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var dbTask = await context.Tasks.FirstOrDefaultAsync(t => t.Id == task.Id);
            if (dbTask != null)
            {
                context.Tasks.Remove(dbTask);
                await context.SaveChangesAsync();
            }
        }

        public void ClearState(string key)
        {
            using var context = _contextFactory.CreateDbContext();
            context.Tasks.RemoveRange(context.Tasks);
            context.SaveChanges();
        }
    }
} 