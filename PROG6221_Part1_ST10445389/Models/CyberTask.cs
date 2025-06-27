// Kallan Jones
// ST10445389
// GROUP 1

using System;

namespace CybersecurityAwarenessBot.Models
{
    public class CyberTask
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public DateTime ReminderDateTime { get; set; }
        public bool IsCompleted { get; set; } = false;
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public string Category { get; set; } = "General"; // e.g., "Password", "Privacy", etc.

        public override string ToString()
        {
            string status = IsCompleted ? "✅" : "⏰";
            return $"{status} {Title} - Due: {ReminderDateTime:dd/MM/yyyy HH:mm}";
        }
    }
}
