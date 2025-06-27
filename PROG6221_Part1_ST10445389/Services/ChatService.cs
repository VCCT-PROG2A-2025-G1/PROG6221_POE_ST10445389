// Kallan Jones
// ST10445389
// GROUP 1

using System;
using System.Collections.Generic;
using System.Media;
using System.Threading;
using System.IO;
using System.Linq;
using Microsoft.VisualBasic;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Runtime.ConstrainedExecution;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using CybersecurityAwarenessBot.Models;

namespace CybersecurityAwarenessBot.Services
{
    public class ChatService
    {
        // ASCII art logo for the chatbot
        static readonly string asciiLogo = @"
 _________________________________________________
 |  _____      _               ____        _     |
 | / ____|    | |             |  _ \      | |    |
 || |    _   _| |__   ___ _ __| |_) | ___ | |_   |
 || |   | | | | '_ \ / _ \ '__|  _ < / _ \| __|  |
 || |___| |_| | |_) |  __/ |  | |_) | (_) | |_   |
 | \_____\__, |_.__/ \___|_|  |____/ \___/ \__|  |
 |        __/ |                                  |
 |       |___/                                   |
 |                                               |
 |    CYBERSECURITY AWARENESS ASSISTANT          |
 |_______________________________________________| 

";

        // Enhanced response database with multiple responses per topic
        static Dictionary<string, ChatResponse> responseDatabase = new Dictionary<string, ChatResponse>(StringComparer.OrdinalIgnoreCase)
        {
            // General questions
            { "hello", new ChatResponse {
                Responses = new List<string> {
                    "Hello! How can I help you with cybersecurity today?",
                    "Hi there! Ready to learn about staying safe online?",
                    "Hello! What cybersecurity topic would you like to explore?"
                },
                Topic = "greeting"
            }},

            { "hi", new ChatResponse {
                Responses = new List<string> {
                    "Hi there! I'm here to help with your cybersecurity questions.",
                    "Hello! What would you like to know about online safety?",
                    "Hi! Ready to boost your cybersecurity knowledge?"
                },
                Topic = "greeting"
            }},

            // Password security - Enhanced with multiple responses
            { "password", new ChatResponse {
                Responses = new List<string> {
                    "Strong passwords are crucial for online security. Use a mix of letters, numbers, and symbols with at least 12 characters.",
                    "Consider using a password manager to create and store strong, unique passwords for each account.",
                    "Never reuse passwords across different accounts. Each account should have a unique password.",
                    "Enable two-factor authentication (2FA) whenever possible for additional security.",
                    "Avoid using personal information like birthdays or names in your passwords.",
                    "Change your passwords regularly, especially for important accounts like banking or email."
                },
                Topic = "password"
            }},
            
            // Phishing - Enhanced responses
            { "phishing", new ChatResponse {
                Responses = new List<string> {
                    "Phishing attacks try to trick you into revealing personal information through fake emails or websites.",
                    "Be cautious of emails with urgent calls to action, spelling errors, or suspicious attachments.",
                    "Always verify the sender's email address and never click on suspicious links.",
                    "When in doubt, contact the company directly using their official website or phone number.",
                    "Look for signs like generic greetings, urgent language, and requests for sensitive information.",
                    "Legitimate companies will never ask for passwords or PINs via email."
                },
                Topic = "phishing"
            }},

            // Scam detection
            { "scam", new ChatResponse {
                Responses = new List<string> {
                    "Common scams include fake lottery wins, romance scams, and tech support fraud.",
                    "Be skeptical of unsolicited offers that seem too good to be true.",
                    "Never send money or personal information to someone you've only met online.",
                    "Verify any claims independently before taking action.",
                    "Trust your instincts - if something feels wrong, it probably is."
                },
                Topic = "scam"
            }},

            // Privacy protection
            { "privacy", new ChatResponse {
                Responses = new List<string> {
                    "Privacy is crucial in the digital age. Review and adjust your social media privacy settings regularly.",
                    "Be careful about what personal information you share online - it can be used against you.",
                    "Use privacy-focused browsers and search engines to limit data collection.",
                    "Read privacy policies of apps and services before agreeing to them.",
                    "Consider using VPNs to protect your browsing activity, especially on public Wi-Fi."
                },
                Topic = "privacy"
            }},
            
            // Safe browsing
            { "browsing", new ChatResponse {
                Responses = new List<string> {
                    "Always check that websites have HTTPS (look for the padlock icon) before entering personal information.",
                    "Keep your browser and operating system updated to protect against security vulnerabilities.",
                    "Consider using a reputable VPN, especially when using public Wi-Fi networks.",
                    "Be cautious about downloading files or clicking on pop-ups from unknown websites.",
                    "Use ad blockers to prevent malicious advertisements from appearing."
                },
                Topic = "browsing"
            }},
            
            // Social engineering
            { "social engineering", new ChatResponse {
                Responses = new List<string> {
                    "Social engineering manipulates people into revealing confidential information or performing actions that compromise security.",
                    "Be skeptical of unsolicited communications, especially those creating urgency or fear.",
                    "Verify the identity of anyone requesting sensitive information, even from trusted organizations.",
                    "Remember that legitimate organizations will never ask for full passwords or PINs over phone or email.",
                    "Attackers often use psychological manipulation - stay calm and think critically."
                },
                Topic = "social engineering"
            }},
            
            // Malware
            { "malware", new ChatResponse {
                Responses = new List<string> {
                    "Malware is malicious software designed to damage or gain unauthorized access to your system.",
                    "Install reputable antivirus software and keep it updated.",
                    "Avoid downloading software from unofficial sources.",
                    "Regularly scan your devices for malware and suspicious activities.",
                    "Be cautious with email attachments and USB drives from unknown sources."
                },
                Topic = "malware"
            }},

            // Sentiment-based responses
            { "worried", new ChatResponse {
                Responses = new List<string> {
                    "It's completely understandable to feel worried about online security. Let me help you feel more confident.",
                    "Your concern shows you're taking cybersecurity seriously, which is great! What specific area worries you most?",
                    "Don't worry - with the right knowledge and practices, you can stay safe online. What would you like to learn about?"
                },
                Topic = "support"
            }},

            { "confused", new ChatResponse {
                Responses = new List<string> {
                    "No worries! Cybersecurity can seem complex, but I'm here to break it down for you.",
                    "Let's take it step by step. What specific aspect would you like me to explain more clearly?",
                    "It's okay to feel confused - cybersecurity is a big topic. Let's focus on one area at a time."
                },
                Topic = "support"
            }},

            { "frustrated", new ChatResponse {
                Responses = new List<string> {
                    "I understand your frustration. Cybersecurity can feel overwhelming, but you're doing great by learning.",
                    "Take a deep breath - you're on the right track by seeking information. How can I help make this easier?",
                    "Frustration is normal when learning new things. Let's break this down into manageable pieces."
                },
                Topic = "support"
            }},
            
            // Default responses for unknown inputs
            { "default", new ChatResponse {
                Responses = new List<string> {
                    "I'm not sure I understand that. Could you rephrase your question?",
                    "I'm still learning! Try asking about password safety, phishing, scams, or privacy.",
                    "That's not something I'm familiar with. Can you ask about a cybersecurity topic?",
                    "I don't quite follow. Would you like to know about passwords, phishing, or safe browsing?",
                    "Could you try asking your question differently? I'm here to help with cybersecurity topics."
                },
                Topic = "unknown"
            }}
        };

        // Conversation context variables
        static UserContext currentUser = new UserContext();
        static Random random = new Random();
        static string lastBotResponse = "";
        static List<string> conversationHistory = new List<string>();


        // Add these variables for follow-up handling
        static bool waitingForYesNo = false;
        static string pendingTopic = "";

        // Sentiment detection keywords
        static Dictionary<string, List<string>> sentimentKeywords = new Dictionary<string, List<string>>
        {
            { "worried", new List<string> { "worried", "scared", "afraid", "anxious", "concerned", "nervous" } },
            { "confused", new List<string> { "confused", "lost", "don't understand", "unclear", "puzzled" } },
            { "frustrated", new List<string> { "frustrated", "annoyed", "difficult", "hard", "complicated" } }
        };

        public void StartChat()
        {
            // Play voice greeting
            PlayVoiceGreeting();

            // Display ASCII logo with color
            DisplayAsciiLogo();

            // Get user name and personalized greeting
            PersonalizedGreeting();

            // Main conversation loop
            ConversationLoop();
        }

        static void PlayVoiceGreeting()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            TypeWriteEffect("🔊 Playing voice greeting...", 0);
            Console.WriteLine();

            try
            {
                using (SoundPlayer player = new SoundPlayer("greeting.wav"))
                {
                    player.Play();
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Could not play audio greeting: {ex.Message}");
                Console.WriteLine("Make sure 'greeting.wav' is in the same folder as the application.");
                Console.WriteLine("\nPress any key to continue...");
                Console.ResetColor();
                Console.ReadKey();
            }

            Console.ResetColor();
        }

        static void DisplayAsciiLogo()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(asciiLogo);
            Console.ResetColor();
            Thread.Sleep(1000);
        }

        static void PersonalizedGreeting()
        {
            DisplayBorderedText("Welcome to the Enhanced Cybersecurity Awareness Assistant");

            bool validName = false;
            while (!validName)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                TypeWriteEffect("What is your name? ", 40);
                Console.ResetColor();

                string userName = Console.ReadLine().Trim();

                if (string.IsNullOrWhiteSpace(userName))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("I need to know your name to personalize our conversation. Please try again.");
                    Console.ResetColor();
                }
                else
                {
                    currentUser.Name = userName;
                    validName = true;
                }
            }

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Magenta;
            TypeWriteEffect($"Hello, {currentUser.Name}! It's great to meet you. I'm here to help you learn about cybersecurity with personalized guidance.", 30);
            Console.WriteLine("\n");
            Console.ResetColor();

            DisplayBorderedText("HOW TO INTERACT WITH ME", ConsoleColor.Cyan);
            Console.WriteLine("• Ask me about cybersecurity topics like 'passwords', 'phishing', 'scams', or 'privacy'");
            Console.WriteLine("• I remember our conversation and can provide personalized responses");
            Console.WriteLine("• Tell me if you're worried, confused, or frustrated - I'm here to help!");
            Console.WriteLine("• Type 'exit' or 'quit' to end our conversation");
            Console.WriteLine("• Type 'help' if you need assistance\n");

            Thread.Sleep(500);
        }

        static void ConversationLoop()
        {
            bool conversationActive = true;

            while (conversationActive)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write($"{currentUser.Name}> ");
                Console.ResetColor();

                string userInput = Console.ReadLine().Trim();
                conversationHistory.Add($"User: {userInput}");

                // Check for exit command
                if (userInput.Equals("exit", StringComparison.OrdinalIgnoreCase) ||
                    userInput.Equals("quit", StringComparison.OrdinalIgnoreCase))
                {
                    ExitChatbot();
                    conversationActive = false;
                    continue;
                }

                // Check for empty input - Enhanced error handling
                if (string.IsNullOrWhiteSpace(userInput))
                {
                    DisplayErrorMessage("Text can't be empty. Please type your question or 'help' for assistance.");
                    continue;
                }

                // Check for help command
                if (userInput.Equals("help", StringComparison.OrdinalIgnoreCase))
                {
                    DisplayHelpInformation();
                    continue;
                }

                // Process and respond to user input
                ProcessUserInput(userInput);
                currentUser.LastInteraction = DateTime.Now;
            }
        }

        static void ProcessUserInput(string input)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("🤖 Bot> ");
            Console.ResetColor();

            // Check if we're waiting for a yes/no response
            if (waitingForYesNo)
            {
                if (input.Equals("yes", StringComparison.OrdinalIgnoreCase) ||
                    input.Equals("y", StringComparison.OrdinalIgnoreCase))
                {
                    HandleYesResponse();
                    return;
                }
                else if (input.Equals("no", StringComparison.OrdinalIgnoreCase) ||
                         input.Equals("n", StringComparison.OrdinalIgnoreCase))
                {
                    HandleNoResponse();
                    return;
                }
                else
                {
                    // Reset and process as normal question
                    waitingForYesNo = false;
                    pendingTopic = "";
                }
            }

            // Check for worried + topic combination first
            if (HandleWorryWithTopic(input))
            {
                return;
            }

            // Check for confusion or request for more details
            if (CheckForConfusion(input))
            {
                return;
            }

            // Detect sentiment first
            string detectedSentiment = DetectSentiment(input);

            // Find the most relevant response key
            string matchedKey = FindBestMatchingKey(input, detectedSentiment);

            // Update user context
            UpdateUserContext(matchedKey, input);

            // Get personalized response
            string response = GetPersonalizedResponse(matchedKey, detectedSentiment);

            // Display the response with typing effect
            TypeWriteEffect(response, 20);
            Console.WriteLine();

            // Store last response and add to conversation history
            lastBotResponse = response;
            conversationHistory.Add($"Bot: {response}");

            // Offer follow-up if appropriate
            OfferFollowUp(matchedKey);
        }

        static bool HandleWorryWithTopic(string input)
        {
            string lowerInput = input.ToLower();

            // Check if they're worried about a specific topic
            if (lowerInput.Contains("worried") || lowerInput.Contains("scared") || lowerInput.Contains("afraid"))
            {
                string topic = "";

                if (lowerInput.Contains("scam")) topic = "scam";
                else if (lowerInput.Contains("phishing")) topic = "phishing";
                else if (lowerInput.Contains("password")) topic = "password";
                else if (lowerInput.Contains("privacy")) topic = "privacy";
                else if (lowerInput.Contains("malware")) topic = "malware";

                if (!string.IsNullOrEmpty(topic))
                {
                    string response = GetWorryResponse(topic);
                    TypeWriteEffect(response, 20);
                    Console.WriteLine();

                    // Update context and offer follow-up
                    currentUser.LastTopic = topic;
                    OfferFollowUp(topic);
                    return true;
                }
            }

            return false;
        }

        static string GetWorryResponse(string topic)
        {
            switch (topic.ToLower())
            {
                case "scam":
                    return "It's completely understandable to feel that way. Scammers can be very convincing. Let me share some tips to help you stay safe.";
                case "phishing":
                    return "It's natural to worry about phishing - these attacks are getting more sophisticated. But don't worry, I can help you spot the warning signs.";
                case "password":
                    return "Password security can feel overwhelming, but you're taking the right step by being concerned. Let me help you create a strong defense.";
                case "privacy":
                    return "Your privacy concerns are completely valid in today's digital world. The good news is there are simple steps to protect yourself.";
                case "malware":
                    return "Malware is definitely something to be cautious about, but with the right knowledge, you can protect yourself effectively.";
                default:
                    return "It's completely understandable to feel worried about online security. Let me help you feel more confident.";
            }
        }

        static bool CheckForConfusion(string input)
        {
            string lowerInput = input.ToLower();

            // Check if user is asking for more details or is confused
            if (lowerInput.Contains("confus") || lowerInput.Contains("don't understand") ||
                lowerInput.Contains("explain") || lowerInput.Contains("more detail") ||
                lowerInput.Contains("tell me more") || lowerInput.Contains("how") ||
                lowerInput.Contains("why"))
            {
                // First check if they mention a specific topic in their current input
                string topicInInput = "";
                if (lowerInput.Contains("password")) topicInInput = "password";
                else if (lowerInput.Contains("phishing")) topicInInput = "phishing";
                else if (lowerInput.Contains("scam")) topicInInput = "scam";
                else if (lowerInput.Contains("privacy")) topicInInput = "privacy";
                else if (lowerInput.Contains("malware")) topicInInput = "malware";

                // Use the topic from current input, or fall back to last topic
                string topicToExplain = !string.IsNullOrEmpty(topicInInput) ? topicInInput : currentUser.LastTopic;

                if (!string.IsNullOrEmpty(topicToExplain))
                {
                    string response = GetDetailedExplanation(topicToExplain);
                    TypeWriteEffect(response, 20);
                    Console.WriteLine();
                    // Update the last topic to the one we just explained
                    currentUser.LastTopic = topicToExplain;
                    return true;
                }
            }

            return false;
        }

        static string GetDetailedExplanation(string topic)
        {
            switch (topic.ToLower())
            {
                case "password":
                    return "Let me explain passwords in more detail: A strong password should be at least 12 characters long with uppercase letters, lowercase letters, numbers, and symbols. Avoid dictionary words and personal information. Consider using a password manager like LastPass or 1Password to generate and store unique passwords for each account.";

                case "phishing":
                    return "Here's a detailed explanation of phishing: Criminals send fake emails that look like they're from real companies. They want you to click links or give them your passwords. Always check the sender's email address carefully - scammers often use similar but fake addresses like 'yourbank-security@gmail.com' instead of 'security@yourbank.com'.";

                case "scam":
                    return "Let me break down common scams: Romance scams involve fake online relationships to get money. Tech support scams are calls claiming to fix your computer. Lottery scams say you won something you never entered. The key rule: Never give money or personal information to someone who contacted you first.";

                case "privacy":
                    return "Privacy protection explained: Control what personal information you share online including photos, location, and daily activities. Review your social media settings regularly - make sure only friends can see your posts. Don't share your exact location publicly or post photos showing your home address.";

                default:
                    return $"I understand you'd like to know more about {topic}. What specific aspect would you like me to explain further?";
            }
        }

        static void HandleYesResponse()
        {
            waitingForYesNo = false;

            // Provide additional information based on the pending topic
            Dictionary<string, List<string>> additionalInfo = new Dictionary<string, List<string>>
            {
                { "password", new List<string> {
                    "Great! Password managers like LastPass, 1Password, or Bitwarden can generate and store unique passwords for all your accounts. They only require you to remember one master password.",
                    "Two-factor authentication (2FA) adds an extra layer of security. Even if someone gets your password, they'd still need your phone or authenticator app to access your account."
                }},
                { "phishing", new List<string> {
                    "Here are key signs of phishing emails: urgent language like 'act now', generic greetings like 'Dear Customer', suspicious sender addresses, and requests for personal information.",
                    "Always hover over links to see the real URL before clicking. Legitimate companies use their official domain names, not random shortened links."
                }},
                { "privacy", new List<string> {
                    "For social media privacy: Review who can see your posts, limit personal information in your profile, be careful about location sharing, and regularly audit your friend/follower lists.",
                    "VPNs (Virtual Private Networks) encrypt your internet connection and hide your real IP address. They're especially important when using public Wi-Fi at cafes, airports, or hotels."
                }},
                { "scam", new List<string> {
                    "Common scam types: Romance scams (fake relationships to get money), tech support scams (fake calls about computer problems), lottery scams (you 'won' something you never entered), and investment scams (get-rich-quick schemes).",
                    "To report scams in South Africa: Contact the South African Police Service, report to the National Consumer Commission, or use the SA Fraud Prevention Service hotline."
                }}
            };

            if (additionalInfo.ContainsKey(pendingTopic))
            {
                var responses = additionalInfo[pendingTopic];
                string response = responses[random.Next(responses.Count)];
                TypeWriteEffect(response, 20);
                Console.WriteLine();
            }
            else
            {
                TypeWriteEffect("Great! I'm glad you're interested in learning more about cybersecurity. What specific aspect would you like to explore further?", 20);
                Console.WriteLine();
            }

            pendingTopic = "";
        }

        static void HandleNoResponse()
        {
            waitingForYesNo = false;
            pendingTopic = "";

            string[] noResponses = {
                "No problem! Feel free to ask me about any other cybersecurity topics.",
                "That's okay! What else would you like to know about staying safe online?",
                "Alright! Is there another cybersecurity topic I can help you with?"
            };

            TypeWriteEffect(noResponses[random.Next(noResponses.Length)], 20);
            Console.WriteLine();
        }

        static string DetectSentiment(string input)
        {
            string lowerInput = input.ToLower();

            foreach (var sentiment in sentimentKeywords)
            {
                foreach (var keyword in sentiment.Value)
                {
                    if (lowerInput.Contains(keyword))
                    {
                        return sentiment.Key;
                    }
                }
            }

            return "neutral";
        }

        static string FindBestMatchingKey(string input, string sentiment)
        {
            // Check sentiment first if detected
            if (sentiment != "neutral" && responseDatabase.ContainsKey(sentiment))
            {
                return sentiment;
            }

            // Check for exact matches
            if (responseDatabase.ContainsKey(input))
            {
                return input;
            }

            // Enhanced keyword matching with scoring
            var matches = new Dictionary<string, int>();

            foreach (var key in responseDatabase.Keys)
            {
                if (key == "default") continue;

                // Check if key is contained in input
                if (input.Contains(key, StringComparison.OrdinalIgnoreCase))
                {
                    matches[key] = key.Length; // Longer matches get higher scores
                }

                // Check for partial matches (for compound topics like "social engineering")
                string[] keyWords = key.Split(' ');
                if (keyWords.Length > 1)
                {
                    int wordMatches = keyWords.Count(word =>
                        input.Contains(word, StringComparison.OrdinalIgnoreCase));

                    if (wordMatches > 0)
                    {
                        matches[key] = wordMatches * 10; // Boost multi-word matches
                    }
                }
            }

            // Return the best match
            if (matches.Any())
            {
                return matches.OrderByDescending(m => m.Value).First().Key;
            }

            return "default";
        }

        static void UpdateUserContext(string topic, string userInput)
        {
            // Track topic interactions
            if (responseDatabase.ContainsKey(topic) && !string.IsNullOrEmpty(topic))
            {
                string topicName = responseDatabase[topic].Topic;

                // Safely initialize and increment topic interactions
                if (!currentUser.TopicInteractions.ContainsKey(topicName))
                {
                    currentUser.TopicInteractions[topicName] = 0;
                }
                currentUser.TopicInteractions[topicName]++;

                // Add to interest topics if frequently asked
                if (currentUser.TopicInteractions[topicName] >= 2 &&
                    !currentUser.InterestTopics.Contains(topicName) &&
                    topicName != "unknown" && topicName != "greeting")
                {
                    currentUser.InterestTopics.Add(topicName);
                }

                currentUser.LastTopic = topicName;
            }
        }

        static string GetPersonalizedResponse(string matchedKey, string sentiment)
        {
            if (!responseDatabase.ContainsKey(matchedKey))
            {
                matchedKey = "default";
            }

            var chatResponse = responseDatabase[matchedKey];
            List<string> possibleResponses = new List<string>(chatResponse.Responses);

            // SIMPLE MEMORY CHECK
            if (chatResponse.Topic == "password" && currentUser.TopicInteractions.ContainsKey("password") && currentUser.TopicInteractions["password"] > 1)
            {
                return "As someone interested in passwords, here's a tip: " + possibleResponses[random.Next(possibleResponses.Count)];
            }
            if (chatResponse.Topic == "phishing" && currentUser.TopicInteractions.ContainsKey("phishing") && currentUser.TopicInteractions["phishing"] > 1)
            {
                return "As someone interested in phishing, here's a tip: " + possibleResponses[random.Next(possibleResponses.Count)];
            }
            if (chatResponse.Topic == "scam" && currentUser.TopicInteractions.ContainsKey("scam") && currentUser.TopicInteractions["scam"] > 1)
            {
                return "As someone interested in scam, here's a tip: " + possibleResponses[random.Next(possibleResponses.Count)];
            }
            if (chatResponse.Topic == "privacy" && currentUser.TopicInteractions.ContainsKey("privacy") && currentUser.TopicInteractions["privacy"] > 1)
            {
                return "As someone interested in privacy, here's a tip: " + possibleResponses[random.Next(possibleResponses.Count)];
            }

            // Regular response
            return possibleResponses[random.Next(possibleResponses.Count)];
        }

        static void OfferFollowUp(string lastTopic)
        {
            // Add null/empty checks to prevent errors
            if (string.IsNullOrEmpty(lastTopic) ||
                !responseDatabase.ContainsKey(lastTopic) ||
                lastTopic == "default")
            {
                return;
            }

            var followUpQuestions = new Dictionary<string, List<string>>
            {
                { "password", new List<string> {
                    "Would you like to know about password managers?",
                    "Are you interested in learning about two-factor authentication?"
                }},
                { "phishing", new List<string> {
                    "Would you like tips on identifying phishing emails?",
                    "Should I explain more about email security?"
                }},
                { "privacy", new List<string> {
                    "Would you like to know about social media privacy settings?",
                    "Are you interested in learning about VPNs?"
                }},
                { "scam", new List<string> {
                    "Would you like to know about specific types of scams?",
                    "Should I explain how to report scams?"
                }}
            };

            try
            {
                string topicName = responseDatabase[lastTopic].Topic;
                if (!string.IsNullOrEmpty(topicName) &&
                    followUpQuestions.ContainsKey(topicName))
                {
                    Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    var questions = followUpQuestions[topicName];
                    string question = questions[random.Next(questions.Count)];
                    TypeWriteEffect($"💡 {question}", 15);
                    Console.WriteLine();
                    Console.ResetColor();

                    // Set up for yes/no response
                    waitingForYesNo = true;
                    pendingTopic = topicName;
                }
            }
            catch (Exception)
            {
                // Silently handle any unexpected errors in follow-up suggestions
                return;
            }
        }

        static void DisplayHelpInformation()
        {
            DisplayBorderedText("ENHANCED HELP INFORMATION", ConsoleColor.Green);
            Console.WriteLine($"Hello {currentUser.Name}! I'm your enhanced Cybersecurity Awareness Assistant!");
            Console.WriteLine("\n🔐 I can help you with:");
            Console.WriteLine("• Password safety and best practices");
            Console.WriteLine("• Phishing scams and how to avoid them");
            Console.WriteLine("• Online scams and fraud detection");
            Console.WriteLine("• Privacy protection techniques");
            Console.WriteLine("• Safe browsing techniques");
            Console.WriteLine("• Social engineering tactics");
            Console.WriteLine("• Malware protection");

            if (currentUser.InterestTopics.Any())
            {
                Console.WriteLine($"\n📊 Your main interests: {string.Join(", ", currentUser.InterestTopics)}");
            }

            Console.WriteLine("\n💬 I understand when you're worried, confused, or frustrated and will adjust my responses accordingly.");
            Console.WriteLine("\n📝 Just type your question, and I'll provide personalized help to keep you safe online!");
            Console.WriteLine();
        }

        static void ExitChatbot()
        {
            DisplayBorderedText("GOODBYE & SUMMARY", ConsoleColor.Yellow);
            Console.ForegroundColor = ConsoleColor.Magenta;

            string farewell = $"Thank you for chatting with me today, {currentUser.Name}! ";

            if (currentUser.InterestTopics.Any())
            {
                farewell += $"I noticed you're particularly interested in: {string.Join(", ", currentUser.InterestTopics)}. ";
            }

            farewell += "Keep practicing good cybersecurity habits and stay safe online!";

            TypeWriteEffect(farewell, 20);
            Console.WriteLine("\n");
            Console.ResetColor();

            // Display conversation summary
            if (currentUser.TopicInteractions.Any())
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("📈 Topics we discussed:");
                foreach (var topic in currentUser.TopicInteractions)
                {
                    Console.WriteLine($"   • {topic.Key}: {topic.Value} time(s)");
                }
                Console.ResetColor();
            }
        }

        static void DisplayErrorMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n🤖 Bot> {message}");
            Console.WriteLine("💡 Try asking about: passwords, phishing, scams, privacy, or browsing safety.\n");
            Console.ResetColor();
        }

        static void DisplayBorderedText(string text, ConsoleColor color = ConsoleColor.Blue)
        {
            int padding = 4;
            int width = text.Length + (padding * 2);

            Console.ForegroundColor = color;
            Console.WriteLine("╔" + new string('═', width) + "╗");
            Console.WriteLine("║" + new string(' ', padding) + text + new string(' ', padding) + "║");
            Console.WriteLine("╚" + new string('═', width) + "╝");
            Console.ResetColor();
            Console.WriteLine();
        }

        static void TypeWriteEffect(string text, int delayMilliseconds)
        {
            foreach (char c in text)
            {
                Console.Write(c);
                if (delayMilliseconds > 0)
                    Thread.Sleep(delayMilliseconds / 2);
            }
        }
    }
}

