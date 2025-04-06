using System.Diagnostics;
using System.Windows.Controls;
using WpfKantarExample.ViewModels;

namespace WpfKantarExample.Views
{
    public partial class TaskListView : UserControl
    {
        public TaskListView()
        {
            InitializeComponent();
            Debug.WriteLine("TaskListView created");
            
            // Verify DataContext
            this.Loaded += (s, e) =>
            {
                Debug.WriteLine($"TaskListView loaded, DataContext: {DataContext?.GetType().Name ?? "null"}");
            };
        }
    }
} 