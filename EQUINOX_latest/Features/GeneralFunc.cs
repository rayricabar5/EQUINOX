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
        // 1. Define a Color Palette for consistency
        private const ConsoleColor HeaderColor = ConsoleColor.Cyan;
        private const ConsoleColor CmdColor = ConsoleColor.Yellow;
        private const ConsoleColor DescColor = ConsoleColor.Gray;
        private const ConsoleColor BorderColor = ConsoleColor.DarkGray;

        public string echo(string[] words)
        {
            return string.Join(" ", words);
        }

        // 2. Main Help Menu with Visual Improvements
        public void Help()
        {
            DrawHeader("SYSTEM MENU");

            Console.WriteLine();
            PrintSection("  [ SYSTEM ]");
            PrintItem("help", "Show this system menu");
            PrintItem("fs-help", "Show file system commands");
            PrintItem("echo <txt>", "Repeat input text");
            PrintItem("reboot", "Restart the machine");
            PrintItem("shutdown", "Power off the system");

            Console.WriteLine();
            PrintSection("  [ APPLICATIONS ]");
            PrintItem("wordle", "Play Wordle (GUI Mode)");
            PrintItem("tictactoe", "Play Tic-Tac-Toe (Console)");
            PrintItem("calc", "Calculator tool");
            PrintItem("canvas", "Enter drawing mode");

            DrawFooter();
        }

        // 3. File System Menu with Tables
        public void FsHelp()
        {
            DrawHeader("FILE SYSTEM MANAGER");

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

        // --- VISUAL HELPER METHODS ---

        // Prints a command and description in two columns with colors
        private void PrintItem(string cmd, string desc)
        {
            Console.Write("    "); // Indent
            Console.ForegroundColor = CmdColor;

            // PadRight ensures the description always starts at the same column
            Console.Write(cmd.PadRight(18));

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write("| "); // Separator

            Console.ForegroundColor = DescColor;
            Console.WriteLine(desc);

            Console.ResetColor();
        }

        // Prints a section title (e.g., [ SYSTEM ])
        private void PrintSection(string title)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(title);
            Console.ResetColor();
        }

        // Draws a bar at the top
        private void DrawHeader(string title)
        {
            Console.ForegroundColor = BorderColor;
            Console.WriteLine(new string('=', 50));

            Console.ForegroundColor = HeaderColor;
            // Center the title roughly
            int spaces = (50 - title.Length) / 2;
            Console.WriteLine(new string(' ', spaces) + title);

            Console.ForegroundColor = BorderColor;
            Console.WriteLine(new string('=', 50));
            Console.ResetColor();
        }

        // Draws a bar at the bottom
        private void DrawFooter()
        {
            Console.ForegroundColor = BorderColor;
            Console.WriteLine(new string('-', 50));
            Console.ResetColor();
        }

        // --- GAME LAUNCHERS (Kept mostly the same, just added cleanup) ---

        public void LaunchTicTacToe()
        {
            Console.Clear(); // Clean slate for the game
            DrawHeader("TIC TAC TOE");
            try
            {
                TicTacToe game = new TicTacToe();
                game.Run();

                Console.Clear();
                // Instead of just text, maybe a welcome back message?
                Help();
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
                canvas.Disable();

                Console.Clear();
                Help();
            }
            catch (Exception e)
            {
                PrintError(e.Message);
            }
        }

        private void PrintError(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("ERROR: " + msg);
            Console.ResetColor();
        }

        public void DrawLogo()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(@"
  ______ ____   _    _  _____ _   _  _____  __   __
 |  ____/ __ \ | |  | ||_   _| \ | |/ __  \ \ \ / /
 | |__ | |  | || |  | |  | | |  \| || |  | | \ V / 
 |  __|| |  | || |  | |  | | | . ` || |  | |  > <  
 | |___| |__| || |__| | _| |_| |\  || |__| | / . \ 
 |______\___\_\ \____/ |_____|_| \_|\_____/ /_/ \_\
                                                     
    ");
            Console.ResetColor();
        }
    }
}