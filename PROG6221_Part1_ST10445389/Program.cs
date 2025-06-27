// Kallan Jones
// ST10445389
// GROUP 1

// REFERENCES: 
// claude.ai

using System;
using CybersecurityAwarenessBot.Models;
using CybersecurityAwarenessBot.Services;

namespace CybersecurityAwarenessBot
{
    class Program
    {
        static void Main(string[] args)
        {
            // Initialize console settings
            Console.Title = "Enhanced Cybersecurity Awareness Bot for South African Citizens";

            var chatService = new ChatService();
            chatService.StartChat();
        }
    }
}