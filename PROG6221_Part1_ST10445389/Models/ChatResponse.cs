// Kallan Jones
// ST10445389
// GROUP 1

using System.Collections.Generic;

namespace CybersecurityAwarenessBot.Models
{
    // Response class for better organization
    public class ChatResponse
    {
        public List<string> Responses { get; set; }
        public string Topic { get; set; } = "";
        public bool IsFollowUp { get; set; }

        public ChatResponse()
        {
            Responses = new List<string>();
            IsFollowUp = false;
        }
    }
}