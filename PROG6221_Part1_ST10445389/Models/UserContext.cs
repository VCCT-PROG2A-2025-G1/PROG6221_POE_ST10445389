// Kallan Jones
// ST10445389
// GROUP 1

using System;
using System.Collections.Generic;

namespace CybersecurityAwarenessBot.Models
{
    public class UserContext
    {
        public string Name { get; set; } = "";
        public List<string> InterestTopics { get; set; }
        public Dictionary<string, int> TopicInteractions { get; set; }
        public string LastTopic { get; set; } = "";
        public DateTime LastInteraction { get; set; }

        public UserContext()
        {
            InterestTopics = new List<string>();
            TopicInteractions = new Dictionary<string, int>();
            LastInteraction = DateTime.Now;
        }
    }
}