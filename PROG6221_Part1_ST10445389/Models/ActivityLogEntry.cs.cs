// Kallan Jones
// ST10445389
// GROUP 1

using System;

namespace CybersecurityAwarenessBot.Models
{
    public class ActivityLogEntry
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string Action { get; set; } = "";
        public string Description { get; set; } = "";
        public string Category { get; set; } = "";
        public string Details { get; set; } = "";

        public ActivityLogEntry()
        {
            Timestamp = DateTime.Now;
        }

        public ActivityLogEntry(string action, string description, string category = "General", string details = "")
        {
            Timestamp = DateTime.Now;
            Action = action;
            Description = description;
            Category = category;
            Details = details;
        }

        public override string ToString()
        {
            return $"[{Timestamp:HH:mm}] {Action}: {Description}";
        }

        public string GetDetailedString()
        {
            string result = $"🕒 **{Timestamp:dd/MM/yyyy HH:mm}** - {Action}\n";
            result += $"   📝 {Description}";

            if (!string.IsNullOrEmpty(Details))
            {
                result += $"\n   ℹ️ {Details}";
            }

            return result;
        }
    }
}