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
        // --- THEME COLORS ---
        private const ConsoleColor HeaderColor = ConsoleColor.Cyan;
        private const ConsoleColor CmdColor = ConsoleColor.Yellow;
        private const ConsoleColor DescColor = ConsoleColor.Gray;
        private const ConsoleColor BorderColor = ConsoleColor.DarkGray;

        public string echo(string[] words)
        {
            return string.Join(" ", words);
        }

        // --- BOOT ANIMATION ---
        public void BootAnimation()
        {
            Console.Clear();

            // Draw Logo
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n\n");
            Console.WriteLine(@"
  _____           _  _        ____   _____ 
 |_   _|         | |(_)      / __ \ / ____|
   | |  _ __   __| | _  ___ | |  | | (___  
   | | | '_ \ / _` || |/ _ \| |  | |\___ \ 
  _| |_| | | | (_| || |  __/| |__| |____) |
 |_____|_| |_|\__,_||_|\___| \____/|_____/ 
            ");
            Console.ResetColor();

            Console.WriteLine("\n");
            Console.Write("    System is starting... ");

            // Manual Progress Bar
            int barStart = Console.CursorLeft;
            Console.Write("[                              ]"); // 30 spaces
            Console.CursorLeft = barStart + 1;

            Console.ForegroundColor = ConsoleColor.Green;
            for (int i = 0; i < 30; i++)
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
            Console.Write("    [");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(" OK ");
            Console.ResetColor();
            Console.WriteLine($"] {text}");

            // Delay
            for (long x = 0; x < 1500000; x++) {; }
        }

        // --- DRAW INDIEOS LOGO ---
        public void DrawLogo()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(@"
  _____           _  _        ____   _____ 
 |_   _|         | |(_)      / __ \ / ____|
   | |  _ __   __| | _  ___ | |  | | (___  
   | | | '_ \ / _` || |/ _ \| |  | |\___ \ 
  _| |_| | | | (_| || |  __/| |__| |____) |
 |_____|_| |_|\__,_||_|\___| \____/|_____/ 
            ");
            Console.ResetColor();
            Console.WriteLine("\n       Welcome to IndieOS v1.0");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("       Type 'help' for commands.");
            Console.ResetColor();
            Console.WriteLine();
        }

        // --- MAIN HELP MENU ---
        public void Help()
        {
            DrawHeader("INDIEOS MENU");

            Console.WriteLine();
            PrintSection("  [ SYSTEM ]");
            PrintItem("help", "Show this system menu");
            PrintItem("fs-help", "Show file system commands");
            PrintItem("echo <txt>", "Repeat input text");
            PrintItem("reboot", "Restart the machine");
            PrintItem("shutdown", "Power off the system");
            PrintItem("cls", "Clear the screen");

            Console.WriteLine();
            PrintSection("  [ APPLICATIONS ]");
            PrintItem("wordle", "Play Wordle (GUI Mode)");
            PrintItem("tictactoe", "Play Tic-Tac-Toe (Console)");
            PrintItem("calc", "Calculator tool");
            PrintItem("canvas", "Enter drawing mode");

            DrawFooter();
        }

        // --- FILE SYSTEM HELP ---
        public void FsHelp()
        {
            DrawHeader("INDIEOS FILESYSTEM");

            Console.WriteLine();
            PrintSection("  [ NAVIGATION & INFO ]");
            PrintItem("pwd", "Print working directory");
            PrintItem("ls [path]", "List directory contents");
            PrintItem("cd <path>", "Change directory");
            PrintItem("find <txt>", "Search for files");

            Console.WriteLine();
            PrintSection("  [ MANIPULATION ]");
            PrintItem("mkdir <path>", "Create new directory");
            PrintItem("touch <path>", "Create file / update time");
            PrintItem("cp <src> <dst>", "Copy file or directory");
            PrintItem("mv <src> <new>", "Rename or move item");
            PrintItem("rm <path>", "Remove file or directory");

            Console.WriteLine();
            PrintSection("  [ I/O OPERATIONS ]");
            PrintItem("write <p> <t>", "Overwrite file content");
            PrintItem("append <p> <t>", "Add text to end of file");
            PrintItem("cat <path>", "Read file contents");

            Console.WriteLine();
            PrintSection("  [ STORAGE ]");
            PrintItem("fs-save", "Save Virtual File System");
            PrintItem("fs-load", "Load Virtual File System");

            DrawFooter();
        }

        // --- HELPER: PRINTING ---
        private void PrintItem(string cmd, string desc)
        {
            Console.Write("    ");
            Console.ForegroundColor = CmdColor;
            Console.Write(cmd.PadRight(18));
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write("| ");
            Console.ForegroundColor = DescColor;
            Console.WriteLine(desc);
            Console.ResetColor();
        }

        private void PrintSection(string title)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(title);
            Console.ResetColor();
        }

        private void DrawHeader(string title)
        {
            Console.ForegroundColor = BorderColor;
            Console.WriteLine(new string('=', 50));
            Console.ForegroundColor = HeaderColor;
            int spaces = (50 - title.Length) / 2;
            Console.WriteLine(new string(' ', spaces) + title);
            Console.ForegroundColor = BorderColor;
            Console.WriteLine(new string('=', 50));
            Console.ResetColor();
        }

        private void DrawFooter()
        {
            Console.ForegroundColor = BorderColor;
            Console.WriteLine(new string('-', 50));
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
                Console.Clear();
                DrawLogo(); // Redraw logo when coming back
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

                Console.Clear();
                DrawLogo(); // Redraw logo when coming back
            }
            catch (Exception e)
            {
                PrintError(e.Message);
            }
        }
    }
}