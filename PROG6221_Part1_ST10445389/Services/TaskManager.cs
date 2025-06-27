// Kallan Jones
// ST10445389
// GROUP 1

using System;
using System.Collections.Generic;
using System.Linq;
using CybersecurityAwarenessBot.Models;

namespace CybersecurityAwarenessBot.Services
{
    public class TaskManager
    {
        private List<CyberTask> tasks;
        private int nextId;

        public TaskManager()
        {
            tasks = new List<CyberTask>();
            nextId = 1;
        }

        public CyberTask AddTask(string title, string description, DateTime reminderDateTime, string category = "General")
        {
            var task = new CyberTask
            {
                Id = nextId++,
                Title = title,
                Description = description,
                ReminderDateTime = reminderDateTime,
                Category = category
            };

            tasks.Add(task);
            return task;
        }

        public List<CyberTask> GetAllTasks()
        {
            return tasks.Where(t => !t.IsDeleted).OrderBy(t => t.ReminderDateTime).ToList();
        }

        public List<CyberTask> GetPendingTasks()
        {
            return tasks.Where(t => !t.IsDeleted && !t.IsCompleted).OrderBy(t => t.ReminderDateTime).ToList();
        }

        public List<CyberTask> GetCompletedTasks()
        {
            return tasks.Where(t => !t.IsDeleted && t.IsCompleted).OrderBy(t => t.ReminderDateTime).ToList();
        }

        public List<CyberTask> GetOverdueTasks()
        {
            return tasks.Where(t => !t.IsDeleted && !t.IsCompleted && t.ReminderDateTime < DateTime.Now)
                       .OrderBy(t => t.ReminderDateTime).ToList();
        }

        public bool MarkTaskComplete(int taskId)
        {
            var task = tasks.FirstOrDefault(t => t.Id == taskId && !t.IsDeleted);
            if (task != null)
            {
                task.IsCompleted = true;
                return true;
            }
            return false;
        }

        public bool DeleteTask(int taskId)
        {
            var task = tasks.FirstOrDefault(t => t.Id == taskId);
            if (task != null)
            {
                task.IsDeleted = true;
                return true;
            }
            return false;
        }

        public CyberTask GetTask(int taskId)
        {
            return tasks.FirstOrDefault(t => t.Id == taskId && !t.IsDeleted);
        }

        public int GetTaskCount()
        {
            return tasks.Count(t => !t.IsDeleted);
        }

        public int GetPendingTaskCount()
        {
            return tasks.Count(t => !t.IsDeleted && !t.IsCompleted);
        }
    }
}