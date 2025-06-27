// Kallan Jones
// ST10445389
// GROUP 1

using System;
using System.Collections.Generic;
using System.Linq;
using CybersecurityAwarenessBot.Models;

namespace CybersecurityAwarenessBot.Services
{
    public class ActivityLogger
    {
        private List<ActivityLogEntry> activityLog;
        private int nextId;

        public ActivityLogger()
        {
            activityLog = new List<ActivityLogEntry>();
            nextId = 1;
        }

        public void LogAction(string action, string description, string category = "General", string details = "")
        {
            var entry = new ActivityLogEntry(action, description, category, details)
            {
                Id = nextId++
            };

            activityLog.Add(entry);

            // Keep only last 100 entries to prevent memory issues
            if (activityLog.Count > 100)
            {
                activityLog.RemoveAt(0);
            }
        }

        public void LogTaskCreated(string taskTitle, DateTime reminderDate, string category)
        {
            LogAction(
                "Task Created",
                $"New task: '{taskTitle}'",
                "Task Management",
                $"Reminder set for {reminderDate:dd/MM/yyyy HH:mm} | Category: {category}"
            );
        }

        public void LogTaskCompleted(string taskTitle, int taskId)
        {
            LogAction(
                "Task Completed",
                $"Completed task #{taskId}: '{taskTitle}'",
                "Task Management",
                "Task marked as completed successfully"
            );
        }

        public void LogTaskDeleted(string taskTitle, int taskId)
        {
            LogAction(
                "Task Deleted",
                $"Deleted task #{taskId}: '{taskTitle}'",
                "Task Management",
                "Task removed from list"
            );
        }

        public void LogQuizStarted()
        {
            LogAction(
                "Quiz Started",
                "Started cybersecurity knowledge quiz",
                "Quiz Activity",
                "10 random questions selected"
            );
        }

        public void LogQuizCompleted(int score, int totalQuestions)
        {
            string performance = score >= 80 ? "Excellent" : score >= 60 ? "Good" : "Needs Improvement";
            LogAction(
                "Quiz Completed",
                $"Quiz finished with score: {score}% ({score}/{totalQuestions})",
                "Quiz Activity",
                $"Performance level: {performance}"
            );
        }

        public void LogTopicDiscussion(string topic, string userInput)
        {
            LogAction(
                "Topic Discussion",
                $"Discussed {topic} security",
                "Learning",
                $"User query: '{userInput.Substring(0, Math.Min(userInput.Length, 50))}{(userInput.Length > 50 ? "..." : "")}'"
            );
        }

        public void LogNLPInteraction(string originalInput, string detectedIntent, double confidence)
        {
            LogAction(
                "NLP Processing",
                $"Processed natural language request",
                "NLP",
                $"Input: '{originalInput}' | Intent: {detectedIntent} | Confidence: {confidence:P0}"
            );
        }

        public void LogUserLogin(string userName)
        {
            LogAction(
                "Session Started",
                $"User '{userName}' started new session",
                "System",
                "Chatbot initialized and ready"
            );
        }

        public void LogHelpRequested()
        {
            LogAction(
                "Help Requested",
                "User requested help information",
                "Support",
                "Displayed available features and commands"
            );
        }

        public List<ActivityLogEntry> GetRecentActivity(int count = 10)
        {
            return activityLog.OrderByDescending(entry => entry.Timestamp)
                            .Take(count)
                            .ToList();
        }

        public List<ActivityLogEntry> GetActivityByCategory(string category, int count = 10)
        {
            return activityLog.Where(entry => entry.Category.Equals(category, StringComparison.OrdinalIgnoreCase))
                            .OrderByDescending(entry => entry.Timestamp)
                            .Take(count)
                            .ToList();
        }

        public List<ActivityLogEntry> GetAllActivity()
        {
            return activityLog.OrderByDescending(entry => entry.Timestamp).ToList();
        }

        public int GetTotalActions()
        {
            return activityLog.Count;
        }

        public Dictionary<string, int> GetActivitySummary()
        {
            return activityLog.GroupBy(entry => entry.Category)
                            .ToDictionary(group => group.Key, group => group.Count());
        }

        public List<ActivityLogEntry> GetTodaysActivity()
        {
            DateTime today = DateTime.Today;
            return activityLog.Where(entry => entry.Timestamp.Date == today)
                            .OrderByDescending(entry => entry.Timestamp)
                            .ToList();
        }
    }
}