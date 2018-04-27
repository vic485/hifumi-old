using Hifumi.Enums;
using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using Console = Colorful.Console;

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

        static void Append(string text, Color color)
        {
            Console.ForegroundColor = color;
            Console.Write(text);
            Console.ResetColor();
        }

        public static void Write(LogSource source, string text, Color color)
        {
            string date = DateTime.Now.ToShortTimeString().Length <= 4 ?
                $"0{DateTime.Now.ToShortTimeString()}" : DateTime.Now.ToShortTimeString();
            Console.Write(Environment.NewLine);
            Append($"-> {date} ", Color.DarkGray);
            Append($"[{source}]", color);
            Append($" {text}", Color.WhiteSmoke);
            _ = LogAsync($"[{DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss")}] [{source}] {text}");
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

            foreach (string line in header)
                Append($"{line}\n", Color.DarkMagenta);
            Append("-> INFORMATION\n", Color.PaleVioletRed);
            Append("\tAuthor: vic485\n\tVersion: 2018-Beta-04-27\n", Color.LightSalmon);
        }
    }
}
