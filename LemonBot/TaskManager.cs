using System;
using LemonBot.Utilities;

namespace LemonBot;

public class TaskManager
{
    private static List<Task> _tasks = new List<Task>();

    public static void AddTask(Task task)
    {
        lock (_tasks)
            _tasks.Add(task);
    }

    public static void RunTask(Action action)
    {
        AddTask(Task.Run(action));
    }

    public static async Task Run() 
    {
        while (true) 
        {
            lock (_tasks) 
            {
                for (int i = 0; i < _tasks.Count; i++)
                {
                    Task task = _tasks[i];
                    if (task.IsCompleted)
                    {
                        _tasks.RemoveAt(i);
                        i--;

                        if (task.IsFaulted)
                        {
                            Logger.Error("Task Error: "+task.Exception.ToString());
                        }
                    }
                }
            }
            
            await Task.Delay(2500); // 2.5 seconds
        }
    }
}
