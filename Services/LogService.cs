using System;
using System.IO;
using System.Threading.Tasks;

namespace Hifumi.Services
{
    public class LogService
    {
        public void Initialize()
        {
            string logPath = Path.Combine(Directory.GetCurrentDirectory(), "log.txt");
            if (!File.Exists(logPath)) File.Create(logPath);
            PrintApplicationInformation();
        }

        static async Task LogAsync(string message)
            => await File.AppendAllTextAsync(Path.Combine(Directory.GetCurrentDirectory(), "log.txt"), message + Environment.NewLine);

        static void Append(string text, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.Write(text);
        }

        public static void Write(string source, string text, ConsoleColor color)
        {
            string date = DateTime.Now.ToShortTimeString().Length <= 4 ?
                $"0{DateTime.Now.ToShortTimeString()}" : DateTime.Now.ToShortTimeString();
            Console.Write(Environment.NewLine);
            Append($"-> {date} ", ConsoleColor.DarkGray);
            Append($"[{source}]", color);
            Append($" {text}", ConsoleColor.White);
            _ = LogAsync($"[{DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss")}] [{source}] {text}");
            Console.ResetColor();
        }

        public void PrintApplicationInformation()
        {
            string[] header =
            {
                @"",
                @" ██╗  ██╗██╗███████╗██╗   ██╗███╗   ███╗██╗",
                @" ██║  ██║██║██╔════╝██║   ██║████╗ ████║██║",
                @" ███████║██║█████╗  ██║   ██║██╔████╔██║██║",
                @" ██╔══██║██║██╔══╝  ██║   ██║██║╚██╔╝██║██║",
                @" ██║  ██║██║██║     ╚██████╔╝██║ ╚═╝ ██║██║",
                @" ╚═╝  ╚═╝╚═╝╚═╝      ╚═════╝ ╚═╝     ╚═╝╚═╝",
                @""
            };

            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            foreach (string line in header)
                Console.WriteLine(line);
            Append("-> INFORMATION\n", ConsoleColor.Red);
            Append("\tAuthor: vic485\n\tVersion: 2018-Beta-04-25\n", ConsoleColor.Yellow);
        }
    }
}
