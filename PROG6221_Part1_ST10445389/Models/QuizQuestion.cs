// Kallan Jones
// ST10445389
// GROUP 1

using System.Collections.Generic;

namespace CybersecurityAwarenessBot.Models
{
    public class QuizQuestion
    {
        public int Id { get; set; }
        public string Question { get; set; } = "";
        public List<string> Options { get; set; }
        public int CorrectAnswerIndex { get; set; }
        public string Explanation { get; set; } = "";
        public string Category { get; set; } = "";
        public bool IsMultipleChoice { get; set; } = true;

        public QuizQuestion()
        {
            Options = new List<string>();
        }
    }

    public class QuizResult
    {
        public int TotalQuestions { get; set; }
        public int CorrectAnswers { get; set; }
        public int Score => (int)((double)CorrectAnswers / TotalQuestions * 100);
        public string GetScoreMessage()
        {
            if (Score >= 90) return "🏆 Excellent! You're a cybersecurity pro!";
            else if (Score >= 80) return "🎉 Great job! You have strong cybersecurity knowledge!";
            else if (Score >= 70) return "👍 Good work! You're on the right track with cybersecurity!";
            else if (Score >= 60) return "📚 Not bad! Keep learning to improve your cybersecurity skills!";
            else return "💪 Keep learning to stay safe online! Practice makes perfect!";
        }
    }
}