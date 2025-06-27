// Kallan Jones
// ST10445389
// GROUP 1

using System;
using System.Windows.Forms;

namespace CybersecurityAwarenessBot
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ChatForm());
        }
    }
}