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
using CybersecurityAwarenessBot.Services;

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

        // Task Management
        private TaskManager taskManager;
        private bool inTaskMode = false;
        private string taskTitle = "";
        private string taskDescription = "";

        // Quiz Game
        private QuizManager quizManager;
        private bool inQuizMode = false;
        private QuizQuestion currentQuizQuestion;

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
        private Button taskButton;
        private Button viewTasksButton;
        private Button quizButton;

        private NLPProcessor nlpProcessor;

        private ActivityLogger activityLogger;

        public ChatForm()
        {
            InitializeComponent();
            InitializeData();
            PlayVoiceGreeting();
        }

        private void InitializeComponent()
        {
            this.Text = "Enhanced Cybersecurity Awareness Bot for South African Citizens";
            this.Size = new Size(1000, 800);
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

            // Task Management Buttons
            taskButton = new Button
            {
                Text = "Add Task 📝",
                Location = new Point(20, 660),
                Size = new Size(120, 35),
                BackColor = Color.FromArgb(128, 0, 128), // Purple
                ForeColor = Color.White,
                Font = new Font("Arial", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Visible = false
            };
            taskButton.Click += TaskButton_Click;

            viewTasksButton = new Button
            {
                Text = "View Tasks 📋",
                Location = new Point(150, 660),
                Size = new Size(120, 35),
                BackColor = Color.FromArgb(255, 140, 0), // Orange
                ForeColor = Color.White,
                Font = new Font("Arial", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Visible = false
            };
            viewTasksButton.Click += ViewTasksButton_Click;

            // Quiz Button
            quizButton = new Button
            {
                Text = "Start Quiz 🎯",
                Location = new Point(280, 660),
                Size = new Size(120, 35),
                BackColor = Color.FromArgb(0, 150, 0), // Dark Green
                ForeColor = Color.White,
                Font = new Font("Arial", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Visible = false
            };
            quizButton.Click += QuizButton_Click;

            // Typing timer for typewriter effect
            typingTimer = new Timer();
            typingTimer.Interval = 2; // Speed of typing effect
            typingTimer.Tick += TypingTimer_Tick;

            this.Controls.Add(asciiLabel);
            this.Controls.Add(namePanel);
            this.Controls.Add(chatDisplay);
            this.Controls.Add(userInput);
            this.Controls.Add(sendButton);
            this.Controls.Add(taskButton);
            this.Controls.Add(viewTasksButton);
            this.Controls.Add(quizButton);
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
            taskManager = new TaskManager();
            quizManager = new QuizManager();
            nlpProcessor = new NLPProcessor();
            activityLogger = new ActivityLogger();

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
                { "task", new ChatResponse {
                    Responses = new List<string> {
                        "I can help you manage cybersecurity tasks! You can add tasks like 'Set up two-factor authentication' or 'Review account privacy settings'. Would you like to add a new task?",
                        "Task management is important for staying organized with your cybersecurity goals. I can help you create, view, and manage your security tasks.",
                        "Let's help you stay on top of your cybersecurity! You can create tasks with reminders to ensure you don't forget important security measures."
                    },
                    Topic = "task"
                }},
                { "tasks", new ChatResponse {
                    Responses = new List<string> {
                        "Here are your current cybersecurity tasks. You can add new ones or manage existing tasks to stay secure!",
                        "Managing your security tasks helps you stay organized and protected. Let me show you your current tasks.",
                        "Great! Let's look at your cybersecurity task list to see what needs attention."
                    },
                    Topic = "task"
                }},
                { "remind", new ChatResponse {
                    Responses = new List<string> {
                        "I can set up reminders for your cybersecurity tasks! What would you like to be reminded about?",
                        "Reminders are great for maintaining good security habits. What cybersecurity task would you like me to remind you about?",
                        "Let's set up a reminder for an important security task. What do you need to remember to do?"
                    },
                    Topic = "task"
                }},
                { "quiz", new ChatResponse {
                    Responses = new List<string> {
                        "Ready to test your cybersecurity knowledge? I have a fun quiz with 10 questions that will help reinforce what you've learned!",
                        "Let's see how much you know about staying safe online! My quiz covers passwords, phishing, scams, and more.",
                        "Time for a cybersecurity challenge! Take my interactive quiz to test and improve your security knowledge."
                    },
                    Topic = "quiz"
                }},
                { "help", new ChatResponse {
                    Responses = new List<string> {
                        "🔐 I can help you with:\n• Password safety and best practices\n• Phishing scams and how to avoid them\n• Online scams and fraud detection\n• Privacy protection techniques\n• Safe browsing techniques\n• Social engineering tactics\n• Malware protection\n• Task management for cybersecurity goals\n• Interactive quiz to test your knowledge\n\n💬 I understand when you're worried, confused, or frustrated and will adjust my responses accordingly.\n📝 Just type your question, and I'll provide personalized help to keep you safe online!",
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

            // Log user session start
            activityLogger.LogUserLogin(currentUser.Name);

            // Hide welcome elements
            asciiLabel.Visible = false;
            namePanel.Visible = false;

            // Show chat elements
            chatDisplay.Visible = true;
            userInput.Visible = true;
            sendButton.Visible = true;
            taskButton.Visible = true;
            viewTasksButton.Visible = true;
            quizButton.Visible = true;

            string greeting = $"Hello, {currentUser.Name}! It's great to meet you. I'm here to help you learn about cybersecurity with personalized guidance.\n\n" +
                            "💡 HOW TO INTERACT WITH ME:\n" +
                            "• Ask me about cybersecurity topics like 'passwords', 'phishing', 'scams', or 'privacy'\n" +
                            "• I remember our conversation and can provide personalized responses\n" +
                            "• Tell me if you're worried, confused, or frustrated - I'm here to help!\n" +
                            "• Use the 'Add Task' button to create cybersecurity reminders\n" +
                            "• Use the 'View Tasks' button to see your task list\n" +
                            "• Use the 'Start Quiz' button to test your cybersecurity knowledge\n" +
                            "• Type 'show activity log' to see what we've accomplished together\n" +
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

        private void TaskButton_Click(object sender, EventArgs e)
        {
            if (!isTyping)
            {
                StartTaskCreation();
            }
        }

        private void ViewTasksButton_Click(object sender, EventArgs e)
        {
            if (!isTyping)
            {
                ShowTaskList();
            }
        }

        private void QuizButton_Click(object sender, EventArgs e)
        {
            if (!isTyping)
            {
                StartQuiz();
            }
        }

        private void SendMessage()
        {
            string message = userInput.Text.Trim();
            if (string.IsNullOrEmpty(message)) return;

            AddMessage($"👤 {currentUser.Name}", message, Color.FromArgb(150, 255, 150));
            userInput.Clear();

            // Process response
            string response = ProcessUserInput(message);
            if (!string.IsNullOrEmpty(response))
            {
                AddMessageWithTyping("🤖 Bot", response, Color.FromArgb(100, 200, 255));
            }

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
                taskButton.Enabled = false;
                viewTasksButton.Enabled = false;
                quizButton.Enabled = false;

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
                    taskButton.Enabled = true;
                    viewTasksButton.Enabled = true;
                    quizButton.Enabled = true;
                    userInput.Focus();
                }
            }
            catch (ObjectDisposedException)
            {
                typingTimer.Stop();
            }
        }

        // QUIZ METHODS
        private void StartQuiz()
        {
            quizManager.StartNewQuiz();
            inQuizMode = true;
            currentQuizQuestion = quizManager.GetCurrentQuestion();

            string welcomeMessage = "🎯 **CYBERSECURITY QUIZ CHALLENGE** 🎯\n\n" +
                                   "Test your knowledge with 10 questions about online safety!\n\n" +
                                   "You'll get immediate feedback after each answer.\n" +
                                   "Ready? Let's start with question 1!\n\n";

            AddMessageWithTyping("🎯 Quiz", welcomeMessage, Color.FromArgb(255, 215, 0));

            // Show first question immediately after welcome message
            System.Threading.Tasks.Task.Delay(5000).ContinueWith(t => {
                if (!this.IsDisposed)
                {
                    this.Invoke(new Action(() => {
                        ShowCurrentQuestion();
                    }));
                }
            });
        }

        private void ShowCurrentQuestion()
        {
            if (currentQuizQuestion == null) return;

            // Create properly formatted question display
            string questionHeader = $"Question {quizManager.GetCurrentQuestionNumber()}/{quizManager.GetTotalQuestions()}:";
            string questionText = $"{currentQuizQuestion.Question}";
            string options = "";  // Changed from 'optionsText' to 'options'
            string instructions = "";  // Changed from 'instructionText' to 'instructions'

            // Format options based on question type
            if (currentQuizQuestion.IsMultipleChoice && currentQuizQuestion.Options.Count > 2)
            {
                // Multiple choice question
                for (int i = 0; i < currentQuizQuestion.Options.Count; i++)
                {
                    char optionLetter = (char)('A' + i);
                    options += $"{optionLetter}) {currentQuizQuestion.Options[i]}\n";
                }
                instructions = "Type your answer (A, B, C, or D):";
            }
            else
            {
                // True/False question (2 options)
                options = "A) True\nB) False\n";
                instructions = "Type your answer (A for True, B for False):";
            }

            string category = $"Category: {currentQuizQuestion.Category}";  // Changed from 'categoryText' to 'category'
            string quitInfo = "Type 'stop' to exit quiz early";  // Changed from 'quitText' to 'quitInfo'

            // Combine all parts with proper spacing
            string fullQuestion = $"{questionHeader}\n\n{questionText}\n\n{options}\n{category}\n\n{instructions}\n{quitInfo}";

            AddMessageWithTyping("Quiz", fullQuestion, Color.FromArgb(255, 215, 0));
        }

        private string HandleQuizAnswer(string input)
        {
            if (currentQuizQuestion == null) return "No active question. Type 'quiz' to start a new quiz!";

            // Check for early exit commands
            if (input.ToLower().Contains("quit") ||
                input.ToLower().Contains("exit") ||
                input.ToLower().Contains("stop") ||
                input.ToLower().Contains("end quiz") ||
                input.ToLower().Contains("finish"))
            {
                return HandleQuizEarlyExit();
            }

            // Parse answer
            input = input.Trim().ToUpper();
            int answerIndex = -1;

            if (input.Length == 1 && input[0] >= 'A' && input[0] <= 'D')
            {
                answerIndex = input[0] - 'A';
            }
            else if (int.TryParse(input, out int numAnswer) && numAnswer >= 1 && numAnswer <= 4)
            {
                answerIndex = numAnswer - 1;
            }

            // Validate answer
            bool isTrueFalse = currentQuizQuestion.Options.Count == 2;

            if (isTrueFalse)
            {
                if (answerIndex < 0 || answerIndex > 1)
                {
                    return "Please enter **A** for True or **B** for False, or type 'quit' to exit the quiz.";
                }
            }
            else
            {
                if (answerIndex < 0 || answerIndex >= currentQuizQuestion.Options.Count)
                {
                    return "Please enter a valid answer (**A**, **B**, **C**, or **D**), or type 'quit' to exit the quiz.";
                }
            }

            // Check answer
            bool isCorrect = quizManager.SubmitAnswer(answerIndex);
            char selectedLetter = (char)('A' + answerIndex);
            char correctLetter = (char)('A' + currentQuizQuestion.CorrectAnswerIndex);

            string feedback = "";

            if (isCorrect)
            {
                feedback = $"CORRECT! Great job!\n\n" +
          $"Your answer: {selectedLetter}) {currentQuizQuestion.Options[answerIndex]}\n\n" +
          $"Explanation: {currentQuizQuestion.Explanation}";
            }
            else
            {
                feedback = $"INCORRECT. Don't worry, this helps you learn!\n\n" +
                          $"Your answer: {selectedLetter}) {currentQuizQuestion.Options[answerIndex]}\n" +
                          $"Correct answer: {correctLetter}) {currentQuizQuestion.Options[currentQuizQuestion.CorrectAnswerIndex]}\n\n" +
                          $"Why this is wrong: {currentQuizQuestion.Explanation}";
            }

            // Move to next question or end quiz
            currentQuizQuestion = quizManager.GetCurrentQuestion();

            if (quizManager.IsQuizComplete())
            {
                // Quiz finished
                var result = quizManager.GetFinalResult();
                activityLogger.LogQuizCompleted(result.Score, result.TotalQuestions);

                feedback += $"\n\n🏁 **QUIZ COMPLETE!** 🏁\n\n" +
                           $"📊 **Final Score:** {result.CorrectAnswers}/{result.TotalQuestions} ({result.Score}%)\n\n" +
                           $"{result.GetScoreMessage()}\n\n" +
                           $"🎓 Keep practicing to stay safe online!\n" +
                           $"Click 'Start Quiz' anytime to test yourself again!";

                inQuizMode = false;
                currentQuizQuestion = null;
            }
            else
            {
                feedback += $"\n\n⏭️ **Ready for the next question?**\n" +
                           $"📊 **Progress:** {quizManager.GetCurrentQuestionNumber() - 1}/{quizManager.GetTotalQuestions()} completed\n" +
                           "────────────────────────────────────";

                // Show next question after a delay
                System.Threading.Tasks.Task.Delay(3000).ContinueWith(t => {
                    if (!this.IsDisposed && inQuizMode)
                    {
                        this.Invoke(new Action(() => {
                            ShowCurrentQuestion();
                        }));
                    }
                });
            }

            return feedback;
        }

        private string HandleQuizEarlyExit()
        {
            if (!inQuizMode)
            {
                return "You're not currently taking a quiz.";
            }

            // Get current progress
            var partialResult = quizManager.GetPartialResult(); // We'll need to add this method to QuizManager
            int questionsAnswered = quizManager.GetCurrentQuestionNumber() - 1;
            int totalQuestions = quizManager.GetTotalQuestions();

            // Log the early exit
            if (questionsAnswered > 0)
            {
                activityLogger.LogAction(
                    "Quiz Exited Early",
                    $"Quiz ended early after {questionsAnswered} of {totalQuestions} questions",
                    "Quiz Activity",
                    $"Score so far: {partialResult.CorrectAnswers}/{questionsAnswered} questions correct"
                );
            }

            // Reset quiz state
            inQuizMode = false;
            currentQuizQuestion = null;

            string response = "🚪 **QUIZ EXITED**\n\n";

            if (questionsAnswered > 0)
            {
                response += $"📊 **Your Progress:**\n" +
                           $"• Questions answered: {questionsAnswered} out of {totalQuestions}\n" +
                           $"• Correct answers: {partialResult.CorrectAnswers}\n" +
                           $"• Current accuracy: {(questionsAnswered > 0 ? (partialResult.CorrectAnswers * 100 / questionsAnswered) : 0)}%\n\n" +
                           $"💡 You did great! Every question helps you learn more about cybersecurity.\n\n";
            }
            else
            {
                response += "No questions were answered.\n\n";
            }

            response += "🎯 Feel free to start a new quiz anytime by clicking 'Start Quiz' or typing 'quiz'!\n" +
                       "📚 You can also ask me about specific cybersecurity topics to continue learning.";

            return response;
        }

        // TASK MANAGEMENT METHODS
        private void StartTaskCreation()
        {
            inTaskMode = true;
            taskTitle = "";
            taskDescription = "";

            string message = "Let's create a new cybersecurity task! 📝\n\n" +
                            "Please provide a title for your task (e.g., 'Set up two-factor authentication', 'Review privacy settings'):";

            AddMessageWithTyping("🤖 Bot", message, Color.FromArgb(100, 200, 255));
        }

        private void ShowTaskList()
        {
            var allTasks = taskManager.GetAllTasks();
            var pendingTasks = taskManager.GetPendingTasks();
            var completedTasks = taskManager.GetCompletedTasks();
            var overdueTasks = taskManager.GetOverdueTasks();

            string message = "📋 **YOUR CYBERSECURITY TASKS**\n\n";

            if (allTasks.Count == 0)
            {
                message += "You don't have any tasks yet. Click 'Add Task' to create your first cybersecurity task!";
            }
            else
            {
                message += $"📊 **Summary:** {allTasks.Count} total tasks, {pendingTasks.Count} pending, {completedTasks.Count} completed\n\n";

                if (overdueTasks.Any())
                {
                    message += "🚨 **OVERDUE TASKS:**\n";
                    foreach (var task in overdueTasks)
                    {
                        message += $"❗ #{task.Id}: {task.Title}\n   Due: {task.ReminderDateTime:dd/MM/yyyy HH:mm}\n   {task.Description}\n\n";
                    }
                }

                if (pendingTasks.Any())
                {
                    message += "⏰ **PENDING TASKS:**\n";
                    foreach (var task in pendingTasks.Where(t => !overdueTasks.Contains(t)))
                    {
                        message += $"📌 #{task.Id}: {task.Title}\n   Due: {task.ReminderDateTime:dd/MM/yyyy HH:mm}\n   {task.Description}\n\n";
                    }
                }

                if (completedTasks.Any())
                {
                    message += "✅ **COMPLETED TASKS:**\n";
                    foreach (var task in completedTasks.Take(5)) // Show last 5 completed
                    {
                        message += $"✅ #{task.Id}: {task.Title}\n   Completed: {task.ReminderDateTime:dd/MM/yyyy}\n\n";
                    }
                }

                message += "\n💡 **Task Commands:**\n" +
                          "• Type 'complete [task #]' to mark a task as done\n" +
                          "• Type 'delete [task #]' to remove a task\n" +
                          "• Click 'Add Task' to create a new task";
            }

            AddMessageWithTyping("🤖 Bot", message, Color.FromArgb(100, 200, 255));
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

            // ACTIVITY LOG COMMANDS
            if (input.ToLower().Contains("activity log") ||
                input.ToLower().Contains("show activity") ||
                input.ToLower().Contains("what have you done") ||
                input.ToLower().Contains("show log"))
            {
                return ShowActivityLog(input);
            }

            // ENHANCED NLP PROCESSING
            var nlpResult = nlpProcessor.ProcessInput(input);

            // Log NLP interaction
            if (nlpResult.Confidence > 0.3)
            {
                //activityLogger.LogNLPInteraction(input, nlpResult.Intent, nlpResult.Confidence);
            }

            // QUIZ LOGIC
            if (inQuizMode)
            {
                return HandleQuizAnswer(input);
            }

            // Enhanced quiz detection using NLP
            if (nlpResult.Intent == "quiz" || input.ToLower().Contains("quiz") || input.ToLower().Contains("test") || input.ToLower().Contains("game"))
            {
                activityLogger.LogQuizStarted();
                StartQuiz();
                return ""; // StartQuiz handles the response
            }

            // TASK MANAGEMENT LOGIC
            if (inTaskMode)
            {
                return HandleTaskCreation(input);
            }

            // AUTOMATIC TASK CREATION from NLP
            if (nlpResult.Intent == "task" && nlpResult.Entities.ContainsKey("task_info"))
            {
                var taskInfo = (TaskInformation)nlpResult.Entities["task_info"];

                // Create the task automatically
                var createdTask = taskManager.AddTask(taskInfo.Title, taskInfo.Description, taskInfo.ReminderDateTime, taskInfo.Category);

                // Log task creation
                activityLogger.LogTaskCreated(createdTask.Title, createdTask.ReminderDateTime, createdTask.Category);

                return $"✅ **Task Created Successfully!**\n\n" +
                       $"📌 **Title:** {createdTask.Title}\n" +
                       $"📝 **Description:** {createdTask.Description}\n" +
                       $"⏰ **Reminder:** {createdTask.ReminderDateTime:dd/MM/yyyy HH:mm}\n" +
                       $"🏷️ **Category:** {createdTask.Category}\n" +
                       $"🆔 **Task ID:** #{createdTask.Id}\n\n" +
                       $"Perfect! I understood your request and set everything up automatically. 🎯";
            }

            // Enhanced task command handling using NLP
            if (nlpResult.Intent == "complete" || input.ToLower().StartsWith("complete "))
            {
                return HandleCompleteTaskNLP(nlpResult, input);
            }

            if (nlpResult.Intent == "delete" || input.ToLower().StartsWith("delete "))
            {
                return HandleDeleteTaskNLP(nlpResult, input);
            }

            // Enhanced task viewing
            if (nlpResult.Intent == "view" || input.ToLower().Contains("my tasks"))
            {
                ShowTaskList();
                return "Here are your current cybersecurity tasks and their status.";
            }

            // Check for help request
            if (nlpResult.Intent == "help" || input.ToLower().Contains("help"))
            {
                activityLogger.LogHelpRequested();
                return GetHelpResponse();
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
                    waitingForYesNo = false;
                    pendingTopic = "";
                }
            }

            // Check for cybersecurity topic discussions
            string detectedTopic = DetectTopicFromNLP(nlpResult);
            if (!string.IsNullOrEmpty(detectedTopic))
            {
                activityLogger.LogTopicDiscussion(detectedTopic, input);
                UpdateUserContext(detectedTopic, input);
                string response = GetPersonalizedResponse(detectedTopic, "neutral");
                string followUp = OfferFollowUp(detectedTopic);
                if (!string.IsNullOrEmpty(followUp))
                {
                    response += "\n\n" + followUp;
                }
                return response;
            }

            // Rest of existing logic...
            string worryResponse = HandleWorryWithTopic(input);
            if (!string.IsNullOrEmpty(worryResponse))
            {
                return worryResponse;
            }

            string confusionResponse = CheckForConfusion(input);
            if (!string.IsNullOrEmpty(confusionResponse))
            {
                return confusionResponse;
            }

            // Fallback to original logic
            string sentiment = DetectSentiment(input);
            string matchedKey = FindBestMatchingKey(input, sentiment);
            UpdateUserContext(matchedKey, input);
            string originalResponse = GetPersonalizedResponse(matchedKey, sentiment);
            string originalFollowUp = OfferFollowUp(matchedKey);
            if (!string.IsNullOrEmpty(originalFollowUp))
            {
                originalResponse += "\n\n" + originalFollowUp;
            }

            return originalResponse;
        }



        private string HandleTaskCreation(string input)
        {
            if (string.IsNullOrEmpty(taskTitle))
            {
                // Getting title
                taskTitle = input.Trim();
                return "Great! Now please provide a description for this task (e.g., 'Enable 2FA on all important accounts'):";
            }
            else if (string.IsNullOrEmpty(taskDescription))
            {
                // Getting description
                taskDescription = input.Trim();
                return "Perfect! When would you like to be reminded about this task? Please enter the date and time (e.g., 'tomorrow 2pm' or '25/12/2024 14:30'):";
            }
            else
            {
                // Getting reminder date
                DateTime reminderDate;
                if (TryParseDateTime(input, out reminderDate))
                {
                    // Determine category based on title/description
                    string category = DetermineTaskCategory(taskTitle + " " + taskDescription);

                    var task = taskManager.AddTask(taskTitle, taskDescription, reminderDate, category);

                    inTaskMode = false;
                    taskTitle = "";
                    taskDescription = "";

                    return $"✅ Task created successfully!\n\n" +
                           $"📌 **{task.Title}**\n" +
                           $"📝 Description: {task.Description}\n" +
                           $"⏰ Reminder: {task.ReminderDateTime:dd/MM/yyyy HH:mm}\n" +
                           $"🏷️ Category: {task.Category}\n" +
                           $"🆔 Task ID: #{task.Id}\n\n" +
                           $"I'll help you stay on track with your cybersecurity goals! Type 'tasks' to view all your tasks.";
                }
                else
                {
                    return "I couldn't understand that date format. Please try again with formats like:\n" +
                           "• 'tomorrow 2pm'\n" +
                           "• '25/12/2024 14:30'\n" +
                           "• 'next week'\n" +
                           "• 'in 3 days'";
                }
            }
        }

        private string HandleCompleteTask(string input)
        {
            string taskIdStr = input.Substring(9).Trim(); // Remove "complete "
            if (int.TryParse(taskIdStr, out int taskId))
            {
                if (taskManager.MarkTaskComplete(taskId))
                {
                    var task = taskManager.GetTask(taskId);
                    return $"🎉 Great job! Task completed: **{task?.Title}**\n\n" +
                           $"You're making excellent progress on your cybersecurity journey! " +
                           $"Staying organized with security tasks helps protect you online.";
                }
                else
                {
                    return $"❌ I couldn't find task #{taskId}. Type 'tasks' to see your current task list.";
                }
            }
            else
            {
                return "Please specify a task ID number. For example: 'complete 1' to mark task #1 as completed.";
            }
        }

        private string HandleDeleteTask(string input)
        {
            string taskIdStr = input.Substring(7).Trim(); // Remove "delete "
            if (int.TryParse(taskIdStr, out int taskId))
            {
                var task = taskManager.GetTask(taskId);
                if (task != null && taskManager.DeleteTask(taskId))
                {
                    return $"🗑️ Task deleted: **{task.Title}**\n\n" +
                           $"The task has been removed from your list. You can always create new tasks to stay organized!";
                }
                else
                {
                    return $"❌ I couldn't find task #{taskId}. Type 'tasks' to see your current task list.";
                }
            }
            else
            {
                return "Please specify a task ID number. For example: 'delete 1' to remove task #1.";
            }
        }

        private string DetermineTaskCategory(string text)
        {
            text = text.ToLower();

            if (text.Contains("password") || text.Contains("2fa") || text.Contains("two-factor") || text.Contains("authentication"))
                return "Password & Authentication";
            else if (text.Contains("privacy") || text.Contains("setting") || text.Contains("profile"))
                return "Privacy";
            else if (text.Contains("update") || text.Contains("software") || text.Contains("antivirus"))
                return "Software & Updates";
            else if (text.Contains("backup") || text.Contains("data"))
                return "Data Protection";
            else if (text.Contains("email") || text.Contains("phishing"))
                return "Email Security";
            else
                return "General Security";
        }

        private bool TryParseDateTime(string input, out DateTime dateTime)
        {
            dateTime = DateTime.Now;
            input = input.ToLower().Trim();

            try
            {
                // Handle relative dates
                if (input.Contains("tomorrow"))
                {
                    dateTime = DateTime.Today.AddDays(1);
                    if (input.Contains("pm") || input.Contains("afternoon"))
                        dateTime = dateTime.AddHours(14);
                    else if (input.Contains("morning"))
                        dateTime = dateTime.AddHours(9);
                    else
                        dateTime = dateTime.AddHours(12);
                    return true;
                }

                if (input.Contains("next week"))
                {
                    dateTime = DateTime.Today.AddDays(7).AddHours(12);
                    return true;
                }

                if (input.Contains("in ") && input.Contains("day"))
                {
                    var parts = input.Split(' ');
                    for (int i = 0; i < parts.Length - 1; i++)
                    {
                        if (parts[i] == "in" && int.TryParse(parts[i + 1], out int days))
                        {
                            dateTime = DateTime.Today.AddDays(days).AddHours(12);
                            return true;
                        }
                    }
                }

                // Try to parse exact date/time
                if (DateTime.TryParse(input, out dateTime))
                {
                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        private string HandleNLPIntent(NLPResult nlpResult, string naturalResponse)
        {
            switch (nlpResult.Intent)
            {
                case "help":
                    return naturalResponse + "\n\n" + GetHelpResponse();

                case "view":
                    // Don't show tasks immediately, just acknowledge
                    return naturalResponse;

                default:
                    return naturalResponse;
            }
        }

        private string HandleCompleteTaskNLP(NLPResult nlpResult, string input)
        {
            if (nlpResult.Entities.ContainsKey("numbers"))
            {
                var numbers = (List<int>)nlpResult.Entities["numbers"];
                int taskId = numbers.First();

                var task = taskManager.GetTask(taskId);
                if (task != null && taskManager.MarkTaskComplete(taskId))
                {
                    activityLogger.LogTaskCompleted(task.Title, taskId);
                    return $"🎉 Perfect! I understood you wanted to complete task #{taskId}: **{task.Title}**\n\n" +
                           $"Great job staying on top of your cybersecurity tasks!";
                }
                else
                {
                    return $"❌ I couldn't find task #{taskId}. Type 'tasks' to see your current task list.";
                }
            }
            else
            {
                return HandleCompleteTask(input);
            }
        }

        private string HandleDeleteTaskNLP(NLPResult nlpResult, string input)
        {
            if (nlpResult.Entities.ContainsKey("numbers"))
            {
                var numbers = (List<int>)nlpResult.Entities["numbers"];
                int taskId = numbers.First();

                var task = taskManager.GetTask(taskId);
                if (task != null && taskManager.DeleteTask(taskId))
                {
                    activityLogger.LogTaskDeleted(task.Title, taskId);
                    return $"🗑️ I understood you wanted to delete task #{taskId}: **{task.Title}**\n\n" +
                           $"The task has been removed from your list successfully!";
                }
                else
                {
                    return $"❌ I couldn't find task #{taskId}. Type 'tasks' to see your current task list.";
                }
            }
            else
            {
                return HandleDeleteTask(input);
            }
        }

        private string GetPersonalizedSummary()
        {
            var allTasks = taskManager.GetAllTasks();
            var completedTasks = taskManager.GetCompletedTasks();
            var pendingTasks = taskManager.GetPendingTasks();

            string summary = $"📋 **Here's what I've helped you with, {currentUser.Name}:**\n\n";

            if (allTasks.Count == 0)
            {
                summary += "We haven't created any cybersecurity tasks yet, but I'm ready to help you stay organized and secure online!";
            }
            else
            {
                summary += "🎯 **Task Summary:**\n";
                for (int i = 0; i < Math.Min(allTasks.Count, 5); i++) // Show last 5 tasks
                {
                    var task = allTasks[i];
                    string status = task.IsCompleted ? "✅ Completed" : "⏰ Pending";
                    summary += $"{i + 1}. {status}: '{task.Title}' (Due: {task.ReminderDateTime:dd/MM/yyyy})\n";
                }

                summary += $"\n📊 **Overall Progress:**\n";
                summary += $"• Total tasks created: {allTasks.Count}\n";
                summary += $"• Completed: {completedTasks.Count}\n";
                summary += $"• Still pending: {pendingTasks.Count}\n";

                if (completedTasks.Count > 0)
                {
                    summary += $"\n🎉 You're doing great with your cybersecurity goals!";
                }
            }

            if (currentUser.TopicInteractions.Any())
            {
                summary += $"\n\n💬 **Topics we've discussed:**\n";
                foreach (var topic in currentUser.TopicInteractions.Take(3))
                {
                    summary += $"• {topic.Key}: {topic.Value} time(s)\n";
                }
            }

            summary += $"\n\n💡 I'm here to help you stay safe online! What would you like to work on next?";

            return summary;
        }

        private string DetectTopicFromNLP(NLPResult nlpResult)
        {
            if (nlpResult.Entities.ContainsKey("security_topics"))
            {
                var topics = (List<string>)nlpResult.Entities["security_topics"];
                return topics.First(); // Return the first detected security topic
            }

            // Check if the intent itself is a security topic
            string[] securityIntents = { "password", "phishing", "scam", "privacy", "malware", "browsing" };
            if (securityIntents.Contains(nlpResult.Intent))
            {
                return nlpResult.Intent;
            }

            return "";
        }

        private string GetHelpResponse()
        {
            return "🔐 I can help you with:\n" +
                   "• Password safety and best practices\n" +
                   "• Phishing scams and how to avoid them\n" +
                   "• Online scams and fraud detection\n" +
                   "• Privacy protection techniques\n" +
                   "• Safe browsing techniques\n" +
                   "• Social engineering tactics\n" +
                   "• Malware protection\n" +
                   "• Task management for cybersecurity goals\n" +
                   "• Interactive quiz to test your knowledge\n\n" +
                   "💬 I understand natural language, so feel free to ask in your own words!\n" +
                   "📝 Just tell me what you'd like to know about staying safe online!";
        }

        private string ShowActivityLog(string input)
        {
            var recentActivity = activityLogger.GetRecentActivity(10);
            var todaysActivity = activityLogger.GetTodaysActivity();
            var summary = activityLogger.GetActivitySummary();

            if (input.ToLower().Contains("show more") || input.ToLower().Contains("full"))
            {
                return ShowDetailedActivityLog();
            }

            // Check if this is the first time showing activity log
            if (!recentActivity.Any())
            {
                return "📊 **ACTIVITY LOG**\n\nNo activities recorded yet. Start using the chatbot to see your progress here!";
            }

            // Create user-friendly summary instead of technical details
            string response = "**Here's a summary of recent actions:**\n\n";

            var userFriendlyActions = ConvertToUserFriendlyActions(recentActivity);

            for (int i = 0; i < userFriendlyActions.Count; i++)
            {
                response += $"{i + 1}. {userFriendlyActions[i]}\n";
            }

            // Add helpful note
            if (userFriendlyActions.Count >= 5)
            {
                response += "\n💡 Type 'show activity log full' to see complete details with timestamps.";
            }

            return response;
        }

        private string FormatActivityLog(List<ActivityLogEntry> activities, string title, bool detailed = false)
        {
            string response = $"📋 **{title}**\n\n";

            if (!activities.Any())
            {
                response += "No activities recorded yet.\n";
                return response;
            }

            foreach (var activity in activities)
            {
                if (detailed)
                {
                    response += activity.GetDetailedString() + "\n\n";
                }
                else
                {
                    response += $"   {activity}\n";
                }
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

        private List<string> ConvertToUserFriendlyActions(List<ActivityLogEntry> activities)
        {
            var userFriendlyActions = new List<string>();

            foreach (var activity in activities.Take(10))
            {
                string friendlyAction = "";

                switch (activity.Action)
                {
                    case "Task Created":
                        // Extract task name from description
                        var taskMatch = System.Text.RegularExpressions.Regex.Match(activity.Description, @"New task: '(.+?)'");
                        if (taskMatch.Success)
                        {
                            string taskName = taskMatch.Groups[1].Value;
                            // Extract reminder date from details
                            var dateMatch = System.Text.RegularExpressions.Regex.Match(activity.Details, @"Reminder set for (.+?) \|");
                            if (dateMatch.Success)
                            {
                                string reminderDate = dateMatch.Groups[1].Value;
                                friendlyAction = $"Task added: '{taskName}' (Reminder set for {reminderDate}).";
                            }
                            else
                            {
                                friendlyAction = $"Task added: '{taskName}'.";
                            }
                        }
                        else
                        {
                            friendlyAction = "Task created.";
                        }
                        break;

                    case "Task Completed":
                        var completedMatch = System.Text.RegularExpressions.Regex.Match(activity.Description, @"Completed task #\d+: '(.+?)'");
                        if (completedMatch.Success)
                        {
                            string taskName = completedMatch.Groups[1].Value;
                            friendlyAction = $"Task completed: '{taskName}'.";
                        }
                        else
                        {
                            friendlyAction = "Task marked as completed.";
                        }
                        break;

                    case "Task Deleted":
                        var deletedMatch = System.Text.RegularExpressions.Regex.Match(activity.Description, @"Deleted task #\d+: '(.+?)'");
                        if (deletedMatch.Success)
                        {
                            string taskName = deletedMatch.Groups[1].Value;
                            friendlyAction = $"Task removed: '{taskName}'.";
                        }
                        else
                        {
                            friendlyAction = "Task deleted.";
                        }
                        break;

                    case "Quiz Started":
                        friendlyAction = "Quiz started - 10 questions selected.";
                        break;

                    case "Quiz Completed":
                        var scoreMatch = System.Text.RegularExpressions.Regex.Match(activity.Description, @"score: (\d+)% \((\d+)/(\d+)\)");
                        if (scoreMatch.Success)
                        {
                            string percentage = scoreMatch.Groups[1].Value;
                            string correct = scoreMatch.Groups[2].Value;
                            string total = scoreMatch.Groups[3].Value;
                            friendlyAction = $"Quiz completed - {correct} out of {total} questions answered correctly ({percentage}%).";
                        }
                        else
                        {
                            friendlyAction = "Quiz completed.";
                        }
                        break;

                    case "Topic Discussion":
                        var topicMatch = System.Text.RegularExpressions.Regex.Match(activity.Description, @"Discussed (.+?) security");
                        if (topicMatch.Success)
                        {
                            string topic = topicMatch.Groups[1].Value;
                            friendlyAction = $"Discussed {topic} security topics.";
                        }
                        else
                        {
                            friendlyAction = "Discussed cybersecurity topics.";
                        }
                        break;

                    case "Session Started":
                        friendlyAction = $"Started new session with {currentUser.Name}.";
                        break;

                    case "Help Requested":
                        friendlyAction = "Requested help information.";
                        break;

                    case "NLP Processing":
                        // Skip NLP technical entries or convert to something meaningful
                        continue;

                    default:
                        friendlyAction = activity.Description;
                        break;
                }

                if (!string.IsNullOrEmpty(friendlyAction))
                {
                    userFriendlyActions.Add(friendlyAction);
                }
            }

            return userFriendlyActions;
        }
        private string ShowDetailedActivityLog()
        {
            var allActivity = activityLogger.GetRecentActivity(15);
            var summary = activityLogger.GetActivitySummary();

            string response = $"📊 **DETAILED ACTIVITY LOG FOR {currentUser.Name}**\n\n";

            // Activity summary
            if (summary.Any())
            {
                response += "📈 **Activity Summary:**\n";
                foreach (var category in summary.Where(kvp => kvp.Key != "NLP" && kvp.Key != "System").OrderByDescending(kvp => kvp.Value))
                {
                    response += $"   • {category.Key}: {category.Value} action(s)\n";
                }
                response += "\n";
            }

            // Recent detailed actions
            response += "🕒 **Recent Actions with Timestamps:**\n";
            if (allActivity.Any())
            {
                var userFriendlyActions = ConvertToUserFriendlyActions(allActivity);
                var activityWithTime = allActivity.Where(a => a.Action != "NLP Processing").Take(10);

                int index = 1;
                foreach (var activity in activityWithTime)
                {
                    var friendlyActions = ConvertToUserFriendlyActions(new List<ActivityLogEntry> { activity });
                    if (friendlyActions.Any())
                    {
                        response += $"   {index}. [{activity.Timestamp:HH:mm}] {friendlyActions[0]}\n";
                        index++;
                    }
                }
            }
            else
            {
                response += "   No activities recorded yet.\n";
            }

            response += $"\n💡 Great progress, {currentUser.Name}! Keep using the chatbot to build your cybersecurity knowledge.";

            return response;
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

            // Display task summary
            var pendingTasks = taskManager.GetPendingTaskCount();
            var totalTasks = taskManager.GetTaskCount();
            if (totalTasks > 0)
            {
                farewell += $"\n📋 Task Summary: {pendingTasks} pending tasks out of {totalTasks} total tasks\n";
                farewell += "Don't forget to complete your cybersecurity tasks!";
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