// Kallan Jones
// ST10445389
// GROUP 1

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Media;
using CybersecurityAwarenessBot.Models;

namespace CybersecurityAwarenessBot
{
    public partial class ChatForm : Form
    {
        private UserContext currentUser;
        private Random random;
        private Dictionary<string, ChatResponse> responseDatabase;
        private Dictionary<string, List<string>> sentimentKeywords;
        private bool waitingForYesNo = false;
        private string pendingTopic = "";

        private RichTextBox chatDisplay;
        private TextBox userInput;
        private Button sendButton;
        private Label nameLabel;
        private TextBox nameInput;
        private Button startButton;
        private Panel namePanel;
        private Label asciiLabel;
        private Timer typingTimer;
        private string currentTypingText = "";
        private int typingIndex = 0;
        private bool isTyping = false;

        public ChatForm()
        {
            InitializeComponent();
            InitializeData();
            PlayVoiceGreeting();
        }

        private void InitializeComponent()
        {
            this.Text = "Enhanced Cybersecurity Awareness Bot for South African Citizens";
            this.Size = new Size(1000, 750);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(20, 20, 30);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // ASCII Art Label
            asciiLabel = new Label
            {
                Text = @"
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
 |_______________________________________________|",
                Location = new Point(20, 20),
                Size = new Size(950, 200),
                Font = new Font("Courier New", 10, FontStyle.Bold),
                ForeColor = Color.Lime,
                BackColor = Color.Transparent
            };

            // Name input panel
            namePanel = new Panel
            {
                Size = new Size(500, 200),
                Location = new Point(250, 250),
                BackColor = Color.FromArgb(40, 40, 60),
                BorderStyle = BorderStyle.FixedSingle
            };

            nameLabel = new Label
            {
                Text = "🔊 Welcome to the Enhanced Cybersecurity Awareness Assistant!\n\nWhat is your name?",
                Location = new Point(20, 20),
                Size = new Size(450, 80),
                Font = new Font("Arial", 12, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.Transparent
            };

            nameInput = new TextBox
            {
                Location = new Point(20, 110),
                Size = new Size(350, 30),
                Font = new Font("Arial", 11),
                BackColor = Color.White,
                ForeColor = Color.Black
            };

            startButton = new Button
            {
                Text = "Start Chat",
                Location = new Point(20, 150),
                Size = new Size(120, 35),
                BackColor = Color.Green,
                ForeColor = Color.White,
                Font = new Font("Arial", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat
            };
            startButton.Click += StartButton_Click;

            namePanel.Controls.Add(nameLabel);
            namePanel.Controls.Add(nameInput);
            namePanel.Controls.Add(startButton);

            // Chat display with enhanced styling
            chatDisplay = new RichTextBox
            {
                Location = new Point(20, 240),
                Size = new Size(940, 350),
                ReadOnly = true,
                Font = new Font("Arial", 10),
                BackColor = Color.FromArgb(25, 25, 35),
                ForeColor = Color.White,
                Visible = false,
                BorderStyle = BorderStyle.FixedSingle
            };

            // User input with enhanced styling
            userInput = new TextBox
            {
                Location = new Point(20, 610),
                Size = new Size(780, 35),
                Font = new Font("Arial", 11),
                BackColor = Color.FromArgb(40, 40, 50),
                ForeColor = Color.White,
                Visible = false,
                BorderStyle = BorderStyle.FixedSingle
            };
            userInput.KeyPress += UserInput_KeyPress;

            // Enhanced send button
            sendButton = new Button
            {
                Text = "Send 📤",
                Location = new Point(820, 610),
                Size = new Size(140, 35),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                Font = new Font("Arial", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Visible = false
            };
            sendButton.Click += SendButton_Click;

            // Typing timer for typewriter effect
            typingTimer = new Timer();
            typingTimer.Interval = 30; // Speed of typing effect
            typingTimer.Tick += TypingTimer_Tick;

            this.Controls.Add(asciiLabel);
            this.Controls.Add(namePanel);
            this.Controls.Add(chatDisplay);
            this.Controls.Add(userInput);
            this.Controls.Add(sendButton);
        }

        private void PlayVoiceGreeting()
        {
            try
            {
                using (SoundPlayer player = new SoundPlayer("greeting.wav"))
                {
                    player.Play();
                }
            }
            catch
            {
                // If no audio file, just continue silently
            }
        }

        private void InitializeData()
        {
            currentUser = new UserContext();
            random = new Random();

            responseDatabase = new Dictionary<string, ChatResponse>(StringComparer.OrdinalIgnoreCase)
            {
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
                { "help", new ChatResponse {
                    Responses = new List<string> {
                        "🔐 I can help you with:\n• Password safety and best practices\n• Phishing scams and how to avoid them\n• Online scams and fraud detection\n• Privacy protection techniques\n• Safe browsing techniques\n• Social engineering tactics\n• Malware protection\n\n💬 I understand when you're worried, confused, or frustrated and will adjust my responses accordingly.\n📝 Just type your question, and I'll provide personalized help to keep you safe online!",
                        "Ask me about cybersecurity topics like 'passwords', 'phishing', 'scams', or 'privacy'. I'm here to help keep you safe online!",
                        "Type 'passwords' for password safety, 'phishing' for email security, 'scams' for fraud protection, or 'privacy' for online privacy tips."
                    },
                    Topic = "help"
                }},
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

            sentimentKeywords = new Dictionary<string, List<string>>
            {
                { "worried", new List<string> { "worried", "scared", "afraid", "anxious", "concerned", "nervous" } },
                { "confused", new List<string> { "confused", "lost", "don't understand", "unclear", "puzzled" } },
                { "frustrated", new List<string> { "frustrated", "annoyed", "difficult", "hard", "complicated" } }
            };
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(nameInput.Text))
            {
                MessageBox.Show("I need to know your name to personalize our conversation. Please try again.",
                    "Name Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            currentUser.Name = nameInput.Text.Trim();

            // Hide welcome elements
            asciiLabel.Visible = false;
            namePanel.Visible = false;

            // Show chat elements
            chatDisplay.Visible = true;
            userInput.Visible = true;
            sendButton.Visible = true;

            // Add personalized greeting with typewriter effect
            string greeting = $"Hello, {currentUser.Name}! It's great to meet you. I'm here to help you learn about cybersecurity with personalized guidance.\n\n" +
                            "💡 HOW TO INTERACT WITH ME:\n" +
                            "• Ask me about cybersecurity topics like 'passwords', 'phishing', 'scams', or 'privacy'\n" +
                            "• I remember our conversation and can provide personalized responses\n" +
                            "• Tell me if you're worried, confused, or frustrated - I'm here to help!\n" +
                            "• Type 'exit' or 'quit' to end our conversation\n" +
                            "• Type 'help' if you need assistance\n\n" +
                            "What would you like to learn about today?";

            AddMessageWithTyping("🤖 Bot", greeting, Color.FromArgb(100, 200, 255));
            userInput.Focus();
        }

        private void UserInput_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter && !isTyping)
            {
                SendMessage();
                e.Handled = true;
            }
        }

        private void SendButton_Click(object sender, EventArgs e)
        {
            if (!isTyping)
                SendMessage();
        }

        private void SendMessage()
        {
            string message = userInput.Text.Trim();
            if (string.IsNullOrEmpty(message)) return;

            AddMessage($"👤 {currentUser.Name}", message, Color.FromArgb(150, 255, 150));
            userInput.Clear();

            // Process response
            string response = ProcessUserInput(message);
            AddMessageWithTyping("🤖 Bot", response, Color.FromArgb(100, 200, 255));

            userInput.Focus();
        }

        private void AddMessage(string sender, string message, Color color)
        {
            if (chatDisplay == null || chatDisplay.IsDisposed || this.IsDisposed)
                return;

            try
            {
                chatDisplay.SelectionStart = chatDisplay.TextLength;
                chatDisplay.SelectionLength = 0;
                chatDisplay.SelectionColor = color;
                chatDisplay.SelectionFont = new Font("Arial", 10, FontStyle.Bold);
                chatDisplay.AppendText($"{sender}: ");
                chatDisplay.SelectionFont = new Font("Arial", 10, FontStyle.Regular);
                chatDisplay.SelectionColor = Color.White;
                chatDisplay.AppendText($"{message}\n\n");
                chatDisplay.ScrollToCaret();
            }
            catch (ObjectDisposedException)
            {
                // Form is closing, ignore this update
            }
        }

        private void AddMessageWithTyping(string sender, string message, Color color)
        {
            if (chatDisplay == null || chatDisplay.IsDisposed || this.IsDisposed)
                return;

            try
            {
                chatDisplay.SelectionStart = chatDisplay.TextLength;
                chatDisplay.SelectionLength = 0;
                chatDisplay.SelectionColor = color;
                chatDisplay.SelectionFont = new Font("Arial", 10, FontStyle.Bold);
                chatDisplay.AppendText($"{sender}: ");

                // Prepare for typing effect
                currentTypingText = message;
                typingIndex = 0;
                isTyping = true;

                // Disable input while typing
                userInput.Enabled = false;
                sendButton.Enabled = false;

                typingTimer.Start();
            }
            catch (ObjectDisposedException)
            {
                // Form is closing, ignore this update
            }
        }

        private void TypingTimer_Tick(object sender, EventArgs e)
        {
            if (chatDisplay == null || chatDisplay.IsDisposed || this.IsDisposed)
            {
                typingTimer.Stop();
                return;
            }

            try
            {
                if (typingIndex < currentTypingText.Length)
                {
                    chatDisplay.SelectionStart = chatDisplay.TextLength;
                    chatDisplay.SelectionLength = 0;
                    chatDisplay.SelectionColor = Color.White;
                    chatDisplay.SelectionFont = new Font("Arial", 10, FontStyle.Regular);
                    chatDisplay.AppendText(currentTypingText[typingIndex].ToString());
                    chatDisplay.ScrollToCaret();
                    typingIndex++;
                }
                else
                {
                    // Typing finished
                    typingTimer.Stop();
                    chatDisplay.AppendText("\n\n");
                    isTyping = false;

                    // Re-enable input
                    userInput.Enabled = true;
                    sendButton.Enabled = true;
                    userInput.Focus();
                }
            }
            catch (ObjectDisposedException)
            {
                typingTimer.Stop();
            }
        }

        private string ProcessUserInput(string input)
        {
            // Handle exit
            if (input.Equals("exit", StringComparison.OrdinalIgnoreCase) ||
                input.Equals("quit", StringComparison.OrdinalIgnoreCase))
            {
                ExitChatbot();
                return "Goodbye! Stay safe online!";
            }

            // Check if we're waiting for a yes/no response
            if (waitingForYesNo)
            {
                if (input.Equals("yes", StringComparison.OrdinalIgnoreCase) ||
                    input.Equals("y", StringComparison.OrdinalIgnoreCase))
                {
                    return HandleYesResponse();
                }
                else if (input.Equals("no", StringComparison.OrdinalIgnoreCase) ||
                         input.Equals("n", StringComparison.OrdinalIgnoreCase))
                {
                    return HandleNoResponse();
                }
                else
                {
                    // Reset and process as normal question
                    waitingForYesNo = false;
                    pendingTopic = "";
                }
            }

            // Check for worried + topic combination first
            string worryResponse = HandleWorryWithTopic(input);
            if (!string.IsNullOrEmpty(worryResponse))
            {
                return worryResponse;
            }

            // Check for confusion or request for more details
            string confusionResponse = CheckForConfusion(input);
            if (!string.IsNullOrEmpty(confusionResponse))
            {
                return confusionResponse;
            }

            // Detect sentiment
            string sentiment = DetectSentiment(input);

            // Find best matching response
            string matchedKey = FindBestMatchingKey(input, sentiment);

            // Update user context
            UpdateUserContext(matchedKey, input);

            // Get personalized response
            string response = GetPersonalizedResponse(matchedKey, sentiment);

            // Offer follow-up if appropriate
            string followUp = OfferFollowUp(matchedKey);
            if (!string.IsNullOrEmpty(followUp))
            {
                response += "\n\n" + followUp;
            }

            return response;
        }

        private string HandleWorryWithTopic(string input)
        {
            string lowerInput = input.ToLower();

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
                    currentUser.LastTopic = topic;
                    return GetWorryResponse(topic);
                }
            }

            return "";
        }

        private string GetWorryResponse(string topic)
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

        private string CheckForConfusion(string input)
        {
            string lowerInput = input.ToLower();

            if (lowerInput.Contains("confus") || lowerInput.Contains("don't understand") ||
                lowerInput.Contains("explain") || lowerInput.Contains("more detail") ||
                lowerInput.Contains("tell me more") || lowerInput.Contains("how") ||
                lowerInput.Contains("why"))
            {
                string topicInInput = "";
                if (lowerInput.Contains("password")) topicInInput = "password";
                else if (lowerInput.Contains("phishing")) topicInInput = "phishing";
                else if (lowerInput.Contains("scam")) topicInInput = "scam";
                else if (lowerInput.Contains("privacy")) topicInInput = "privacy";
                else if (lowerInput.Contains("malware")) topicInInput = "malware";

                string topicToExplain = !string.IsNullOrEmpty(topicInInput) ? topicInInput : currentUser.LastTopic;

                if (!string.IsNullOrEmpty(topicToExplain))
                {
                    currentUser.LastTopic = topicToExplain;
                    return GetDetailedExplanation(topicToExplain);
                }
            }

            return "";
        }

        private string GetDetailedExplanation(string topic)
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

        private string HandleYesResponse()
        {
            waitingForYesNo = false;

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
                pendingTopic = "";
                return response;
            }
            else
            {
                pendingTopic = "";
                return "Great! I'm glad you're interested in learning more about cybersecurity. What specific aspect would you like to explore further?";
            }
        }

        private string HandleNoResponse()
        {
            waitingForYesNo = false;
            pendingTopic = "";

            string[] noResponses = {
                "No problem! Feel free to ask me about any other cybersecurity topics.",
                "That's okay! What else would you like to know about staying safe online?",
                "Alright! Is there another cybersecurity topic I can help you with?"
            };

            return noResponses[random.Next(noResponses.Length)];
        }

        private string DetectSentiment(string input)
        {
            string lowerInput = input.ToLower();
            foreach (var sentiment in sentimentKeywords)
            {
                foreach (var keyword in sentiment.Value)
                {
                    if (lowerInput.Contains(keyword))
                        return sentiment.Key;
                }
            }
            return "neutral";
        }

        private string FindBestMatchingKey(string input, string sentiment)
        {
            // Check sentiment first
            if (sentiment != "neutral" && responseDatabase.ContainsKey(sentiment))
                return sentiment;

            // Check for exact matches
            if (responseDatabase.ContainsKey(input))
                return input;

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

        private void UpdateUserContext(string topic, string userInput)
        {
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
                currentUser.LastInteraction = DateTime.Now;
            }
        }

        private string GetPersonalizedResponse(string matchedKey, string sentiment)
        {
            if (!responseDatabase.ContainsKey(matchedKey))
                matchedKey = "default";

            var chatResponse = responseDatabase[matchedKey];
            var responses = chatResponse.Responses;

            // SIMPLE MEMORY AND RECALL - Enhanced for GUI
            if (chatResponse.Topic == "password" && currentUser.TopicInteractions.ContainsKey("password") && currentUser.TopicInteractions["password"] > 1)
            {
                return $"As someone interested in passwords, here's a tip: {responses[random.Next(responses.Count)]}";
            }
            if (chatResponse.Topic == "phishing" && currentUser.TopicInteractions.ContainsKey("phishing") && currentUser.TopicInteractions["phishing"] > 1)
            {
                return $"As someone interested in phishing, here's a tip: {responses[random.Next(responses.Count)]}";
            }
            if (chatResponse.Topic == "scam" && currentUser.TopicInteractions.ContainsKey("scam") && currentUser.TopicInteractions["scam"] > 1)
            {
                return $"As someone interested in scams, here's a tip: {responses[random.Next(responses.Count)]}";
            }
            if (chatResponse.Topic == "privacy" && currentUser.TopicInteractions.ContainsKey("privacy") && currentUser.TopicInteractions["privacy"] > 1)
            {
                return $"As someone interested in privacy, here's a tip: {responses[random.Next(responses.Count)]}";
            }
            if (chatResponse.Topic == "browsing" && currentUser.TopicInteractions.ContainsKey("browsing") && currentUser.TopicInteractions["browsing"] > 1)
            {
                return $"As someone interested in safe browsing, here's a tip: {responses[random.Next(responses.Count)]}";
            }
            if (chatResponse.Topic == "malware" && currentUser.TopicInteractions.ContainsKey("malware") && currentUser.TopicInteractions["malware"] > 1)
            {
                return $"As someone interested in malware protection, here's a tip: {responses[random.Next(responses.Count)]}";
            }

            // Add name personalization occasionally
            if (random.Next(1, 4) == 1 &&
                chatResponse.Topic != "greeting" &&
                !string.IsNullOrEmpty(currentUser.Name))
            {
                string baseResponse = responses[random.Next(responses.Count)];
                return $"{currentUser.Name}, {baseResponse.ToLower()}";
            }

            return responses[random.Next(responses.Count)];
        }

        private string OfferFollowUp(string lastTopic)
        {
            if (string.IsNullOrEmpty(lastTopic) ||
                !responseDatabase.ContainsKey(lastTopic) ||
                lastTopic == "default")
            {
                return "";
            }

            var followUpQuestions = new Dictionary<string, List<string>>
            {
                { "password", new List<string> {
                    "💡 Would you like to know about password managers?",
                    "💡 Are you interested in learning about two-factor authentication?"
                }},
                { "phishing", new List<string> {
                    "💡 Would you like tips on identifying phishing emails?",
                    "💡 Should I explain more about email security?"
                }},
                { "privacy", new List<string> {
                    "💡 Would you like to know about social media privacy settings?",
                    "💡 Are you interested in learning about VPNs?"
                }},
                { "scam", new List<string> {
                    "💡 Would you like to know about specific types of scams?",
                    "💡 Should I explain how to report scams?"
                }},
                { "browsing", new List<string> {
                    "💡 Would you like to know more about secure browsing practices?",
                    "💡 Should I explain about browser security settings?"
                }},
                { "malware", new List<string> {
                    "💡 Would you like to know about antivirus software?",
                    "💡 Should I explain how to avoid malware infections?"
                }}
            };

            try
            {
                string topicName = responseDatabase[lastTopic].Topic;
                if (!string.IsNullOrEmpty(topicName) &&
                    followUpQuestions.ContainsKey(topicName))
                {
                    var questions = followUpQuestions[topicName];
                    string question = questions[random.Next(questions.Count)];

                    // Set up for yes/no response
                    waitingForYesNo = true;
                    pendingTopic = topicName;

                    return question;
                }
            }
            catch
            {
                // Silently handle any unexpected errors in follow-up suggestions
                return "";
            }

            return "";
        }

        private void ExitChatbot()
        {
            string farewell = $"Thank you for chatting with me today, {currentUser.Name}! ";

            if (currentUser.InterestTopics.Any())
            {
                farewell += $"I noticed you're particularly interested in: {string.Join(", ", currentUser.InterestTopics)}. ";
            }

            farewell += "Keep practicing good cybersecurity habits and stay safe online!";

            // Display conversation summary
            if (currentUser.TopicInteractions.Any())
            {
                farewell += "\n\n📈 Topics we discussed:\n";
                foreach (var topic in currentUser.TopicInteractions)
                {
                    farewell += $"   • {topic.Key}: {topic.Value} time(s)\n";
                }
            }

            MessageBox.Show(farewell, "Goodbye & Summary", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (typingTimer != null)
            {
                typingTimer.Stop();
                typingTimer.Dispose();
            }
            base.OnFormClosing(e);
        }
    }
}