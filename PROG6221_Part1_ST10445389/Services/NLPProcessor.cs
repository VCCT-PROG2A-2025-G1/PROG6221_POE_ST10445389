// Kallan Jones
// ST10445389
// GROUP 1

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace CybersecurityAwarenessBot.Services
{
    public class NLPProcessor
    {
        private Dictionary<string, List<string>> intentKeywords;
        private Dictionary<string, List<string>> synonyms;
        private List<string> stopWords;

        public NLPProcessor()
        {
            InitializeKeywords();
            InitializeSynonyms();
            InitializeStopWords();
        }

        private void InitializeKeywords()
        {
            intentKeywords = new Dictionary<string, List<string>>
            {
                // Task-related keywords
                { "task", new List<string> {
                    "task", "reminder", "remind", "schedule", "create", "add", "set", "make",
                    "todo", "to-do", "note", "notify", "alert", "appointment", "meeting"
                }},
                
                // Quiz-related keywords
                { "quiz", new List<string> {
                    "quiz", "test", "game", "challenge", "question", "exam", "assessment",
                    "check", "evaluate", "practice", "try", "play"
                }},
                
                // View/Show keywords
                { "view", new List<string> {
                    "view", "show", "list", "display", "see", "check", "look", "find",
                    "get", "retrieve", "what", "which", "my"
                }},
                
                // Complete keywords
                { "complete", new List<string> {
                    "complete", "done", "finish", "finished", "completed", "mark", "tick",
                    "check off", "accomplish", "achieve"
                }},
                
                // Delete keywords
                { "delete", new List<string> {
                    "delete", "remove", "cancel", "clear", "erase", "eliminate", "drop",
                    "get rid", "take away", "destroy"
                }},
                
                // Help keywords
                { "help", new List<string> {
                    "help", "assist", "support", "guide", "explain", "how", "what", "info",
                    "information", "instructions", "tutorial"
                }},
                
                // Cybersecurity topic keywords
                { "password", new List<string> {
                    "password", "passwords", "pass", "login", "credentials", "authentication",
                    "2fa", "two-factor", "multi-factor", "mfa", "passcode"
                }},

                { "phishing", new List<string> {
                    "phishing", "phish", "fake email", "suspicious email", "scam email",
                    "email scam", "spam", "fraudulent"
                }},

                { "scam", new List<string> {
                    "scam", "scams", "fraud", "fraudulent", "fake", "deception", "trick",
                    "con", "swindle", "cheat"
                }},

                { "privacy", new List<string> {
                    "privacy", "private", "personal", "data", "information", "settings",
                    "profile", "account", "social media"
                }},

                { "malware", new List<string> {
                    "malware", "virus", "trojan", "spyware", "ransomware", "worm",
                    "infection", "antivirus", "security software"
                }},

                { "browsing", new List<string> {
                    "browsing", "browser", "website", "internet", "online", "web",
                    "https", "ssl", "secure", "safe browsing"
                }}
            };
        }

        private void InitializeSynonyms()
        {
            synonyms = new Dictionary<string, List<string>>
            {
                { "create", new List<string> { "make", "add", "set up", "establish", "build", "generate" }},
                { "show", new List<string> { "display", "view", "list", "see", "look at", "check" }},
                { "help", new List<string> { "assist", "support", "guide", "aid", "explain" }},
                { "safe", new List<string> { "secure", "protected", "safety", "security" }},
                { "update", new List<string> { "change", "modify", "edit", "alter", "revise" }},
                { "check", new List<string> { "verify", "confirm", "validate", "examine", "review" }}
            };
        }

        private void InitializeStopWords()
        {
            stopWords = new List<string>
            {
                "the", "a", "an", "and", "or", "but", "in", "on", "at", "to", "for", "of", "with",
                "by", "from", "up", "about", "into", "through", "during", "before", "after",
                "above", "below", "between", "among", "is", "am", "are", "was", "were", "be",
                "been", "being", "have", "has", "had", "do", "does", "did", "will", "would",
                "could", "should", "may", "might", "must", "can", "i", "you", "he", "she",
                "it", "we", "they", "me", "him", "her", "us", "them", "my", "your", "his",
                "this", "that", "these", "those", "please", "thanks", "thank", "okay", "ok"
            };
        }

        public NLPResult ProcessInput(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return new NLPResult { Intent = "unknown", Confidence = 0.0 };

            // Clean and normalize input
            string cleanedInput = CleanInput(input);
            List<string> tokens = Tokenize(cleanedInput);
            List<string> filteredTokens = RemoveStopWords(tokens);

            // Detect intent
            var intentResult = DetectIntent(filteredTokens, cleanedInput);

            // Extract entities (numbers, specific terms)
            var entities = ExtractEntities(tokens, cleanedInput);

            // ENHANCED: Extract complete task information
            var taskInfo = ExtractTaskInformation(cleanedInput, tokens);
            if (taskInfo != null)
            {
                entities["task_info"] = taskInfo;
            }

            return new NLPResult
            {
                Intent = intentResult.Intent,
                Confidence = intentResult.Confidence,
                Entities = entities,
                ProcessedTokens = filteredTokens,
                OriginalInput = input
            };
        }

        private TaskInformation ExtractTaskInformation(string input, List<string> tokens)
        {
            // Check if this is a task creation request
            if (!IsTaskCreationRequest(input))
                return null;

            var taskInfo = new TaskInformation();

            // Extract title and description from common patterns
            ExtractTitleAndDescription(input, taskInfo);

            // Extract time information
            ExtractTimeInformation(input, tokens, taskInfo);

            // Extract category
            taskInfo.Category = ExtractCategory(input);

            return taskInfo;
        }

        private bool IsTaskCreationRequest(string input)
        {
            string[] taskPatterns = {
        "add a task for",
        "add a task to",
        "create a task for",
        "create a task to",
        "remind me to",
        "add a reminder for",
        "add a reminder to",
        "create a reminder to",
        "create a reminder for",
        "set a reminder to",
        "set a reminder for",
        "help me remember to",
        "i need to",
        "don't forget to",
        "schedule a task to",
        "schedule a task for"
    };

            string lowerInput = input.ToLower();
            return taskPatterns.Any(pattern => lowerInput.Contains(pattern));
        }

        private void ExtractTitleAndDescription(string input, TaskInformation taskInfo)
        {
            // Common patterns for task extraction - FIXED to handle "for"
            var patterns = new List<(Regex regex, string titleGroup, string descGroup)>
    {
        // "Add a task for [action]" - FIXED PATTERN
        (new Regex(@"add a task for (.+?)(?:\s+(?:tomorrow|today|next week|in \d+|on \w+|at \d+).*)?$", RegexOptions.IgnoreCase), "$1", ""),
        
        // "Add a task to [action]" 
        (new Regex(@"add a task to (.+?)(?:\s+(?:tomorrow|today|next week|in \d+|on \w+|at \d+).*)?$", RegexOptions.IgnoreCase), "$1", ""),
        
        // "Remind me to [action]"
        (new Regex(@"remind me to (.+?)(?:\s+(?:tomorrow|today|next week|in \d+|on \w+|at \d+).*)?$", RegexOptions.IgnoreCase), "$1", ""),
        
        // "Create a reminder for [action]"
        (new Regex(@"create a reminder for (.+?)(?:\s+(?:tomorrow|today|next week|in \d+|on \w+|at \d+).*)?$", RegexOptions.IgnoreCase), "$1", ""),
        
        // "Create a reminder to [action]"
        (new Regex(@"create a reminder to (.+?)(?:\s+(?:tomorrow|today|next week|in \d+|on \w+|at \d+).*)?$", RegexOptions.IgnoreCase), "$1", ""),
        
        // "Set a reminder to [action]"
        (new Regex(@"set a reminder to (.+?)(?:\s+(?:tomorrow|today|next week|in \d+|on \w+|at \d+).*)?$", RegexOptions.IgnoreCase), "$1", ""),
        
        // "I need to [action]"
        (new Regex(@"i need to (.+?)(?:\s+(?:tomorrow|today|next week|in \d+|on \w+|at \d+).*)?$", RegexOptions.IgnoreCase), "$1", "")
    };

            foreach (var (regex, titleGroup, descGroup) in patterns)
            {
                var match = regex.Match(input);
                if (match.Success)
                {
                    string extractedAction = match.Groups[1].Value.Trim();

                    // Generate title and description
                    taskInfo.Title = CapitalizeFirst(extractedAction);
                    taskInfo.Description = GenerateDescription(extractedAction);
                    return;
                }
            }

            // Fallback - simple text replacement
            string cleanInput = input.ToLower()
                .Replace("add a task for", "")
                .Replace("add a task to", "")
                .Replace("remind me to", "")
                .Replace("create a reminder to", "")
                .Replace("create a reminder for", "")
                .Replace("set a reminder to", "")
                .Replace("set a reminder for", "")
                .Replace("i need to", "")
                .Trim();

            if (!string.IsNullOrEmpty(cleanInput) && cleanInput.Length > 2)
            {
                taskInfo.Title = CapitalizeFirst(cleanInput);
                taskInfo.Description = GenerateDescription(cleanInput);
            }
            else
            {
                // Final fallback
                taskInfo.Title = "Enable two-factor authentication";
                taskInfo.Description = "Set up 2FA for enhanced security";
            }
        }

        private string GenerateDescription(string action)
        {
            // Generate appropriate descriptions based on the action
            action = action.ToLower();

            if (action.Contains("password"))
            {
                if (action.Contains("update") || action.Contains("change"))
                    return "Update passwords for better security";
                else if (action.Contains("create") || action.Contains("set"))
                    return "Create strong, unique passwords";
                else
                    return "Manage password security";
            }
            else if (action.Contains("2fa") || action.Contains("two-factor") || action.Contains("authentication"))
            {
                return "Enable two-factor authentication for enhanced security";
            }
            else if (action.Contains("privacy"))
            {
                return "Review and update privacy settings";
            }
            else if (action.Contains("backup"))
            {
                return "Create secure backups of important data";
            }
            else if (action.Contains("update") && action.Contains("software"))
            {
                return "Update software and security patches";
            }
            else if (action.Contains("antivirus") || action.Contains("scan"))
            {
                return "Run antivirus scan and update protection";
            }
            else
            {
                return $"Complete: {CapitalizeFirst(action)}";
            }
        }

        private void ExtractTimeInformation(string input, List<string> tokens, TaskInformation taskInfo)
        {
            DateTime reminderTime = DateTime.Now.AddHours(1); // Default to 1 hour from now

            // Time extraction patterns
            if (input.Contains("tomorrow"))
            {
                reminderTime = DateTime.Today.AddDays(1);
                if (input.Contains("morning"))
                    reminderTime = reminderTime.AddHours(9);
                else if (input.Contains("afternoon"))
                    reminderTime = reminderTime.AddHours(14);
                else if (input.Contains("evening"))
                    reminderTime = reminderTime.AddHours(18);
                else
                    reminderTime = reminderTime.AddHours(12); // Default to noon
            }
            else if (input.Contains("today"))
            {
                reminderTime = DateTime.Today;
                if (input.Contains("afternoon"))
                    reminderTime = reminderTime.AddHours(14);
                else if (input.Contains("evening"))
                    reminderTime = reminderTime.AddHours(18);
                else
                    reminderTime = reminderTime.AddHours(DateTime.Now.Hour + 2); // 2 hours from now
            }
            else if (input.Contains("next week"))
            {
                reminderTime = DateTime.Today.AddDays(7).AddHours(12);
            }
            else if (Regex.IsMatch(input, @"in (\d+) days?"))
            {
                var match = Regex.Match(input, @"in (\d+) days?");
                if (int.TryParse(match.Groups[1].Value, out int days))
                {
                    reminderTime = DateTime.Today.AddDays(days).AddHours(12);
                }
            }
            else if (Regex.IsMatch(input, @"in (\d+) hours?"))
            {
                var match = Regex.Match(input, @"in (\d+) hours?");
                if (int.TryParse(match.Groups[1].Value, out int hours))
                {
                    reminderTime = DateTime.Now.AddHours(hours);
                }
            }

            taskInfo.ReminderDateTime = reminderTime;
        }

        private string ExtractCategory(string input)
        {
            input = input.ToLower();

            if (input.Contains("password") || input.Contains("2fa") || input.Contains("two-factor") || input.Contains("authentication"))
                return "Password & Authentication";
            else if (input.Contains("privacy") || input.Contains("setting") || input.Contains("profile"))
                return "Privacy";
            else if (input.Contains("update") || input.Contains("software") || input.Contains("antivirus"))
                return "Software & Updates";
            else if (input.Contains("backup") || input.Contains("data"))
                return "Data Protection";
            else if (input.Contains("email") || input.Contains("phishing"))
                return "Email Security";
            else
                return "General Security";
        }

        private string CapitalizeFirst(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            return char.ToUpper(text[0]) + text.Substring(1);
        }

        // Rest of your existing methods (CleanInput, Tokenize, etc.) remain the same...
        private string CleanInput(string input)
        {
            string cleaned = input.ToLower().Trim();
            cleaned = Regex.Replace(cleaned, @"\s+", " ");
            cleaned = cleaned.Replace("can't", "cannot");
            cleaned = cleaned.Replace("won't", "will not");
            cleaned = cleaned.Replace("i'm", "i am");
            cleaned = cleaned.Replace("you're", "you are");
            cleaned = cleaned.Replace("it's", "it is");
            cleaned = cleaned.Replace("don't", "do not");
            cleaned = cleaned.Replace("doesn't", "does not");
            return cleaned;
        }

        private List<string> Tokenize(string input)
        {
            string[] tokens = Regex.Split(input, @"[\s,;.!?]+")
                                  .Where(t => !string.IsNullOrWhiteSpace(t))
                                  .ToArray();
            return tokens.ToList();
        }

        private List<string> RemoveStopWords(List<string> tokens)
        {
            return tokens.Where(token => !stopWords.Contains(token)).ToList();
        }

        private (string Intent, double Confidence) DetectIntent(List<string> tokens, string fullInput)
        {
            var intentScores = new Dictionary<string, double>();

            foreach (var intent in intentKeywords.Keys)
            {
                intentScores[intent] = 0.0;
            }

            // Check for task creation patterns first
            if (IsTaskCreationRequest(fullInput))
            {
                intentScores["task"] += 3.0; // High priority for task creation
            }

            foreach (var intent in intentKeywords)
            {
                foreach (var keyword in intent.Value)
                {
                    if (fullInput.Contains(keyword))
                    {
                        if (keyword.Contains(" "))
                        {
                            intentScores[intent.Key] += 2.0;
                        }
                        else
                        {
                            intentScores[intent.Key] += 1.0;
                        }
                    }
                }
            }

            foreach (string token in tokens)
            {
                foreach (var intent in intentKeywords)
                {
                    if (intent.Value.Contains(token))
                    {
                        intentScores[intent.Key] += 0.5;
                    }
                }
            }

            intentScores = HandleSpecialPatterns(fullInput, tokens, intentScores);

            var bestIntent = intentScores.OrderByDescending(kvp => kvp.Value).First();
            double maxPossibleScore = tokens.Count * 2.0;
            double confidence = Math.Min(bestIntent.Value / Math.Max(maxPossibleScore, 1.0), 1.0);

            if (confidence < 0.1)
            {
                return ("unknown", confidence);
            }

            return (bestIntent.Key, confidence);
        }

        private Dictionary<string, double> HandleSpecialPatterns(string fullInput, List<string> tokens, Dictionary<string, double> scores)
        {
            if (ContainsPattern(fullInput, new[] { "add", "task" }) ||
                ContainsPattern(fullInput, new[] { "create", "reminder" }) ||
                ContainsPattern(fullInput, new[] { "set", "reminder" }) ||
                ContainsPattern(fullInput, new[] { "remind", "me" }))
            {
                scores["task"] += 1.5;
            }

            if (ContainsPattern(fullInput, new[] { "show", "tasks" }) ||
                ContainsPattern(fullInput, new[] { "view", "tasks" }) ||
                ContainsPattern(fullInput, new[] { "my", "tasks" }) ||
                ContainsPattern(fullInput, new[] { "list", "tasks" }))
            {
                scores["view"] += 1.5;
                scores["task"] += 1.0;
            }

            return scores;
        }

        private bool ContainsPattern(string input, string[] words)
        {
            return words.All(word => input.Contains(word));
        }

        private Dictionary<string, object> ExtractEntities(List<string> tokens, string fullInput)
        {
            var entities = new Dictionary<string, object>();

            var numbers = new List<int>();
            foreach (string token in tokens)
            {
                if (int.TryParse(token, out int number))
                {
                    numbers.Add(number);
                }
            }

            if (numbers.Any())
            {
                entities["numbers"] = numbers;
            }

            var timeEntities = new List<string>();
            string[] timeKeywords = { "tomorrow", "today", "yesterday", "next week", "next month",
                                    "morning", "afternoon", "evening", "night", "am", "pm" };

            foreach (string keyword in timeKeywords)
            {
                if (fullInput.Contains(keyword))
                {
                    timeEntities.Add(keyword);
                }
            }

            if (timeEntities.Any())
            {
                entities["time"] = timeEntities;
            }

            var topics = new List<string>();
            string[] securityTopics = { "password", "phishing", "scam", "privacy", "malware", "browsing" };

            foreach (string topic in securityTopics)
            {
                if (fullInput.Contains(topic))
                {
                    topics.Add(topic);
                }
            }

            if (topics.Any())
            {
                entities["security_topics"] = topics;
            }

            return entities;
        }

        public string GetNaturalResponse(NLPResult nlpResult)
        {
            string intent = nlpResult.Intent;
            var entities = nlpResult.Entities;

            switch (intent)
            {
                case "task":
                    if (entities.ContainsKey("task_info"))
                    {
                        var taskInfo = (TaskInformation)entities["task_info"];
                        return $"Perfect! I'll create your cybersecurity task: '{taskInfo.Title}' scheduled for {taskInfo.ReminderDateTime:dd/MM/yyyy HH:mm}.";
                    }
                    return "I understand you want to create a cybersecurity task. Let me help you set that up!";

                case "view":
                    return "I'll show you your current cybersecurity tasks and their status.";

                case "quiz":
                    return "Great! I have an interactive cybersecurity quiz ready for you.";

                case "complete":
                    if (entities.ContainsKey("numbers"))
                    {
                        var numbers = (List<int>)entities["numbers"];
                        int taskId = numbers.First();
                        return $"I understand you want to mark task #{taskId} as complete.";
                    }
                    return "I'll help you mark a task as completed.";

                case "delete":
                    if (entities.ContainsKey("numbers"))
                    {
                        var numbers = (List<int>)entities["numbers"];
                        int taskId = numbers.First();
                        return $"I understand you want to delete task #{taskId}.";
                    }
                    return "I can help you delete a task.";

                case "help":
                    return "I'm here to help with cybersecurity advice, task management, and quizzes!";

                default:
                    return "";
            }
        }
    }

    public class TaskInformation
    {
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public DateTime ReminderDateTime { get; set; } = DateTime.Now.AddHours(1);
        public string Category { get; set; } = "General Security";
    }

    public class NLPResult
    {
        public string Intent { get; set; } = "";
        public double Confidence { get; set; }
        public Dictionary<string, object> Entities { get; set; } = new Dictionary<string, object>();
        public List<string> ProcessedTokens { get; set; } = new List<string>();
        public string OriginalInput { get; set; } = "";
    }
}