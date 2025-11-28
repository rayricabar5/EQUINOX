using Cosmos.System;
using Cosmos.System.FileSystem;
using Cosmos.System.Graphics;
using System;
using System.Drawing;
using Console = System.Console;
using EQUINOX.Display;

namespace EQUINOX.Features
{
    public class GeneralFunc
    {
        // --- THEME COLORS (Matched to AccountSystem) ---
        private const ConsoleColor ClrBorder = ConsoleColor.Cyan;
        private const ConsoleColor ClrHeader = ConsoleColor.White;
        private const ConsoleColor ClrCmd = ConsoleColor.Yellow;
        private const ConsoleColor ClrDesc = ConsoleColor.Gray;
        private const ConsoleColor ClrSuccess = ConsoleColor.Green;

        public string echo(string[] words)
        {
            return string.Join(" ", words);
        }

        // --- BOOT ANIMATION ---
        public void BootAnimation()
        {
            Console.Clear();

            // Draw Logo
            DrawCenteredLogo();

            Console.WriteLine("\n");

            // Centered Loading Bar
            string loadingText = "System is starting...";
            int centerText = (Console.WindowWidth - loadingText.Length) / 2;
            Console.SetCursorPosition(centerText, Console.CursorTop);
            Console.Write(loadingText);
            Console.WriteLine("\n");

            int barWidth = 30;
            int centerBar = (Console.WindowWidth - barWidth) / 2;

            Console.SetCursorPosition(centerBar - 1, Console.CursorTop);
            Console.Write("[");
            int barStart = Console.CursorLeft;
            Console.Write(new string(' ', barWidth));
            Console.Write("]");

            Console.CursorLeft = barStart;
            Console.ForegroundColor = ClrSuccess;
            for (int i = 0; i < barWidth; i++)
            {
                Console.Write("=");
                // Dummy loop for delay
                for (long x = 0; x < 2000000; x++) {; }
            }
            Console.ResetColor();
            Console.WriteLine("\n");

            PrintBootStatus("Initializing Kernel modules...");
            PrintBootStatus("Mounting Virtual File System...");
            PrintBootStatus("Checking Peripheral Devices...");
            PrintBootStatus("Loading User Shell...");

            // Final delay
            for (long x = 0; x < 3000000; x++) {; }

            // Clear to start fresh
            Console.Clear();

            // Show the main static logo
            DrawLogo();
        }

        private void PrintBootStatus(string text)
        {
            // Center the boot status lines slightly
            int indent = (Console.WindowWidth - 50) / 2;
            if (indent < 0) indent = 0;
            Console.SetCursorPosition(indent, Console.CursorTop);

            Console.Write("[");
            Console.ForegroundColor = ClrSuccess;
            Console.Write(" OK ");
            Console.ResetColor();
            Console.WriteLine($"] {text}");

            // Delay
            for (long x = 0; x < 1500000; x++) {; }
        }

        // --- DRAW INDIEOS LOGO (Centered) ---
        public void DrawLogo()
        {
            Console.Clear();
            DrawCenteredLogo();

            Console.WriteLine("\n");
            CenterText("Welcome to IndieOS v1.0", ConsoleColor.White);
            CenterText("Type 'help' for commands.", ConsoleColor.DarkGray);
            Console.WriteLine();
        }

        private void DrawCenteredLogo()
        {
            string[] logo = new string[]
            {
                @"  _____           _  _        ____   _____ ",
                @" |_   _|         | |(_)      / __ \ / ____|",
                @"   | |  _ __   __| | _  ___ | |  | | (___  ",
                @"   | | | '_ \ / _` || |/ _ \| |  | |\___ \ ",
                @"  _| |_| | | | (_| || |  __/| |__| |____) |",
                @" |_____|_| |_|\__,_||_|\___| \____/|_____/ "
            };

            Console.ForegroundColor = ClrBorder;
            foreach (string line in logo)
            {
                int centerX = (Console.WindowWidth - line.Length) / 2;
                if (centerX < 0) centerX = 0;
                Console.SetCursorPosition(centerX, Console.CursorTop);
                Console.WriteLine(line);
            }
            Console.ResetColor();
        }

        // --- MAIN HELP MENU ---
        public void Help()
        {
            DrawHeader("INDIEOS MENU");

            Console.WriteLine();
            PrintSection(" [ SYSTEM ]");
            PrintItem("help", "Show this system menu");
            PrintItem("fs-help", "Show file system commands");
            PrintItem("echo <txt>", "Repeat input text");
            PrintItem("reboot", "Restart the machine");
            PrintItem("shutdown", "Power off the system");
            PrintItem("cls", "Clear the screen");
            PrintItem("launch-gui", "Enter GUI mode");
            PrintItem("user", "View username and password");

            Console.WriteLine();
            PrintSection(" [ APPLICATIONS ]");
            PrintItem("wordle", "Play Wordle");
            PrintItem("tictactoe", "Play Tic-Tac-Toe");
            PrintItem("calc", "Calculator tool");
            PrintItem("piano", "Play Piano Synth");
            PrintItem("typerace", "Play Type Racing");
            PrintItem("canvas", "Enter drawing mode");

            Console.WriteLine();
            DrawHeader("END OF MENU");
        }

        // --- FILE SYSTEM HELP ---
        public void FsHelp()
        {
            DrawHeader("INDIEOS FILESYSTEM");

            Console.WriteLine();
            PrintSection(" [ NAVIGATION & INFO ]");
            PrintItem("pwd", "Print working directory");
            PrintItem("ls [path]", "List directory contents");
            PrintItem("cd <path>", "Change directory");
            PrintItem("find <txt>", "Search for files");

            Console.WriteLine();
            PrintSection(" [ MANIPULATION ]");
            PrintItem("mkdir <path>", "Create new directory");
            PrintItem("touch <path>", "Create file / update time");
            PrintItem("cp <src> <dst>", "Copy file or directory");
            PrintItem("mv <src> <new>", "Rename or move item");
            PrintItem("rm <path>", "Remove file or directory");

            Console.WriteLine();
            PrintSection(" [ I/O OPERATIONS ]");
            PrintItem("write <p> <t>", "Overwrite file content");
            PrintItem("append <p> <t>", "Add text to end of file");
            PrintItem("cat <path>", "Read file contents");

            Console.WriteLine();
            PrintSection(" [ STORAGE ]");
            PrintItem("fs-save", "Save Virtual File System");
            PrintItem("fs-load", "Load Virtual File System");

            Console.WriteLine();
            DrawHeader("END OF LIST");
        }

        // --- HELPER: PRINTING ---

        private void PrintItem(string cmd, string desc)
        {
            // Indent items for cleaner look
            Console.Write("    ");

            Console.ForegroundColor = ClrCmd;
            Console.Write(cmd.PadRight(18));

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write("| ");

            Console.ForegroundColor = ClrDesc;
            Console.WriteLine(desc);

            Console.ResetColor();
        }

        private void PrintSection(string title)
        {
            Console.ForegroundColor = ClrSuccess;
            Console.WriteLine(" " + title);
            Console.ResetColor();
        }

        private void DrawHeader(string text)
        {
            int totalWidth = Console.WindowWidth;
            int textLen = text.Length;
            int dashLen = (totalWidth - textLen) / 2;
            if (dashLen < 0) dashLen = 0;

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write(new string('-', dashLen));

            Console.ForegroundColor = ClrBorder;
            Console.Write(text);

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(new string('-', dashLen));
            Console.ResetColor();
        }

        public void LaunchTyperace()
        {
            Console.WriteLine("Initializing Typerace (Graphics Mode)...");
            try
            {
                Canvas canvas = FullScreenCanvas.GetFullScreenCanvas(new Mode(800, 600, ColorDepth.ColorDepth32));
                canvas.Clear(Color.Black);
                Typeracing game = new Typeracing(canvas);
                game.Run();
                canvas.Disable();
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Returned to Text Mode.");
                Console.ForegroundColor = ConsoleColor.White;
                Help();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error launching Typerace: " + e.Message);
            }
        }

        private void CenterText(string text, ConsoleColor color)
        {
            int x = (Console.WindowWidth - text.Length) / 2;
            if (x < 0) x = 0;
            Console.SetCursorPosition(x, Console.CursorTop);
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ResetColor();
        }

        private void PrintError(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("ERROR: " + msg);
            Console.ResetColor();
        }

        // --- LAUNCHERS ---
        public void LaunchTicTacToe()
        {
            Console.Clear();
            try
            {
                TicTacToe game = new TicTacToe();
                game.Run();
                DrawLogo(); // Reset screen to logo on exit
            }
            catch (Exception e)
            {
                PrintError(e.Message);
            }
        }

        public void LaunchWordle()
        {
            Console.WriteLine("Initializing Graphics Mode...");
            try
            {
                Canvas canvas = FullScreenCanvas.GetFullScreenCanvas(new Mode(800, 600, ColorDepth.ColorDepth32));
                canvas.Clear(Color.Black);
                WordleGame game = new WordleGame(canvas);
                game.Run();

                // Disable graphics to return to text mode
                canvas.Disable();

                DrawLogo(); // Reset screen to logo on exit
            }
            catch (Exception e)
            {
                PrintError(e.Message);
            }
        }
    }
}