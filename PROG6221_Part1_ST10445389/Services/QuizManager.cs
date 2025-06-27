// Kallan Jones
// ST10445389
// GROUP 1

using System;
using System.Collections.Generic;
using System.Linq;
using CybersecurityAwarenessBot.Models;

namespace CybersecurityAwarenessBot.Services
{
    public class QuizManager
    {
        private List<QuizQuestion> allQuestions;
        private List<QuizQuestion> currentQuizQuestions;
        private int currentQuestionIndex;
        private int correctAnswers;
        private Random random;

        public QuizManager()
        {
            random = new Random();
            InitializeQuestions();
        }

        private void InitializeQuestions()
        {
            allQuestions = new List<QuizQuestion>
            {
                // Phishing Questions
                new QuizQuestion
                {
                    Id = 1,
                    Question = "What should you do if you receive an email asking for your password?",
                    Options = new List<string> { "Reply with your password", "Delete the email", "Report the email as phishing", "Ignore it" },
                    CorrectAnswerIndex = 2,
                    Explanation = "Correct! Reporting phishing emails helps prevent scams. Legitimate companies never ask for passwords via email.",
                    Category = "Phishing",
                    IsMultipleChoice = true
                },
                new QuizQuestion
                {
                    Id = 2,
                    Question = "Phishing emails often contain urgent language to pressure you into acting quickly.",
                    Options = new List<string> { "True", "False" },
                    CorrectAnswerIndex = 0,
                    Explanation = "True! Phishing emails use urgency tactics like 'Act now!' or 'Your account will be closed!' to make you panic and respond without thinking.",
                    Category = "Phishing",
                    IsMultipleChoice = false
                },

                // Password Security Questions
                new QuizQuestion
                {
                    Id = 3,
                    Question = "Which of these is the strongest password?",
                    Options = new List<string> { "password123", "MyDog2023", "Tr0ub4dor&3", "P@ssw0rd!" },
                    CorrectAnswerIndex = 2,
                    Explanation = "Correct! 'Tr0ub4dor&3' is strong because it's long, uses mixed characters, and isn't a common pattern. Avoid dictionary words and predictable substitutions.",
                    Category = "Passwords",
                    IsMultipleChoice = true
                },
                new QuizQuestion
                {
                    Id = 4,
                    Question = "You should use the same password for multiple accounts to make it easier to remember.",
                    Options = new List<string> { "True", "False" },
                    CorrectAnswerIndex = 1,
                    Explanation = "False! Using unique passwords for each account is crucial. If one account is compromised, others remain safe. Use a password manager to help remember different passwords.",
                    Category = "Passwords",
                    IsMultipleChoice = false
                },
                new QuizQuestion
                {
                    Id = 5,
                    Question = "What does 2FA (Two-Factor Authentication) require?",
                    Options = new List<string> { "Only a password", "Password + something you have/know", "Just a phone number", "Only biometric data" },
                    CorrectAnswerIndex = 1,
                    Explanation = "Correct! 2FA requires something you know (password) plus something you have (phone, app) or are (fingerprint). This adds an extra security layer.",
                    Category = "Passwords",
                    IsMultipleChoice = true
                },

                // Safe Browsing Questions
                new QuizQuestion
                {
                    Id = 6,
                    Question = "What does HTTPS indicate about a website?",
                    Options = new List<string> { "It's faster", "It's encrypted and secure", "It's free to use", "It's a government site" },
                    CorrectAnswerIndex = 1,
                    Explanation = "Correct! HTTPS means the connection is encrypted and secure. Always look for the padlock icon when entering personal information.",
                    Category = "Safe Browsing",
                    IsMultipleChoice = true
                },
                new QuizQuestion
                {
                    Id = 7,
                    Question = "It's safe to download software from any website as long as it's free.",
                    Options = new List<string> { "True", "False" },
                    CorrectAnswerIndex = 1,
                    Explanation = "False! Only download software from official websites or trusted sources. Free software from unknown sites often contains malware.",
                    Category = "Safe Browsing",
                    IsMultipleChoice = false
                },

                // Social Engineering Questions
                new QuizQuestion
                {
                    Id = 8,
                    Question = "A caller claims to be from your bank and asks for your account details to 'verify your identity.' What should you do?",
                    Options = new List<string> { "Provide the information", "Hang up and call your bank directly", "Ask for their employee ID", "Transfer to a supervisor" },
                    CorrectAnswerIndex = 1,
                    Explanation = "Correct! Always hang up and call your bank using the number on your card or statement. Legitimate institutions won't ask for full details over the phone.",
                    Category = "Social Engineering",
                    IsMultipleChoice = true
                },
                new QuizQuestion
                {
                    Id = 9,
                    Question = "Social engineering attacks only happen through email.",
                    Options = new List<string> { "True", "False" },
                    CorrectAnswerIndex = 1,
                    Explanation = "False! Social engineering can happen via phone calls, text messages, social media, or even in person. Attackers use psychology to manipulate people.",
                    Category = "Social Engineering",
                    IsMultipleChoice = false
                },

                // Privacy Questions
                new QuizQuestion
                {
                    Id = 10,
                    Question = "What information should you avoid posting on social media?",
                    Options = new List<string> { "Your favorite food", "Your exact location and vacation dates", "Photos of your pets", "Your hobbies" },
                    CorrectAnswerIndex = 1,
                    Explanation = "Correct! Posting your location and vacation dates can help criminals know when you're away from home. Keep travel plans private until you return.",
                    Category = "Privacy",
                    IsMultipleChoice = true
                },

                // Malware Questions
                new QuizQuestion
                {
                    Id = 11,
                    Question = "Which of these can help protect against malware?",
                    Options = new List<string> { "Regular software updates", "Antivirus software", "Being cautious with downloads", "All of the above" },
                    CorrectAnswerIndex = 3,
                    Explanation = "Correct! All of these help protect against malware. Updates patch security holes, antivirus detects threats, and caution prevents infections.",
                    Category = "Malware",
                    IsMultipleChoice = true
                },
                new QuizQuestion
                {
                    Id = 12,
                    Question = "USB drives from unknown sources are always safe to use.",
                    Options = new List<string> { "True", "False" },
                    CorrectAnswerIndex = 1,
                    Explanation = "False! USB drives can contain malware that automatically runs when plugged in. Only use USB drives from trusted sources and scan them first.",
                    Category = "Malware",
                    IsMultipleChoice = false
                },

                // Scam Questions
                new QuizQuestion
                {
                    Id = 13,
                    Question = "You receive an email saying you've won a lottery you never entered. What's this likely to be?",
                    Options = new List<string> { "A lucky surprise", "A mistake", "A scam", "A legitimate prize" },
                    CorrectAnswerIndex = 2,
                    Explanation = "Correct! This is a classic lottery scam. You can't win a lottery you didn't enter. These scams try to get your personal information or money.",
                    Category = "Scams",
                    IsMultipleChoice = true
                },
                new QuizQuestion
                {
                    Id = 14,
                    Question = "If an online offer seems too good to be true, it probably is.",
                    Options = new List<string> { "True", "False" },
                    CorrectAnswerIndex = 0,
                    Explanation = "True! This is a golden rule of online safety. Scammers often use unrealistic offers to lure victims. Always research before believing amazing deals.",
                    Category = "Scams",
                    IsMultipleChoice = false
                },

                // General Security Questions
                new QuizQuestion
                {
                    Id = 15,
                    Question = "How often should you back up important data?",
                    Options = new List<string> { "Never", "Once a year", "Regularly (weekly/monthly)", "Only when buying a new computer" },
                    CorrectAnswerIndex = 2,
                    Explanation = "Correct! Regular backups protect against data loss from hardware failure, malware, or theft. Follow the 3-2-1 rule: 3 copies, 2 different media, 1 offsite.",
                    Category = "General Security",
                    IsMultipleChoice = true
                }
            };
        }

        public void StartNewQuiz()
        {
            // Select 10 random questions
            currentQuizQuestions = allQuestions.OrderBy(x => random.Next()).Take(10).ToList();
            currentQuestionIndex = 0;
            correctAnswers = 0;
        }

        public QuizQuestion GetCurrentQuestion()
        {
            if (currentQuestionIndex < currentQuizQuestions.Count)
            {
                return currentQuizQuestions[currentQuestionIndex];
            }
            return null;
        }

        public bool SubmitAnswer(int answerIndex)
        {
            var currentQuestion = GetCurrentQuestion();
            if (currentQuestion == null) return false;

            bool isCorrect = answerIndex == currentQuestion.CorrectAnswerIndex;
            if (isCorrect)
            {
                correctAnswers++;
            }

            currentQuestionIndex++;
            return isCorrect;
        }

        public bool IsQuizComplete()
        {
            return currentQuestionIndex >= currentQuizQuestions.Count;
        }

        public QuizResult GetFinalResult()
        {
            return new QuizResult
            {
                TotalQuestions = currentQuizQuestions.Count,
                CorrectAnswers = correctAnswers
            };
        }

        public int GetCurrentQuestionNumber()
        {
            return currentQuestionIndex + 1;
        }

        public int GetTotalQuestions()
        {
            return currentQuizQuestions?.Count ?? 10;
        }

        public QuizResult GetPartialResult()
        {
            return new QuizResult
            {
                TotalQuestions = Math.Max(currentQuestionIndex, 1), // Prevent division by zero
                CorrectAnswers = correctAnswers
            };
        }
    }
}