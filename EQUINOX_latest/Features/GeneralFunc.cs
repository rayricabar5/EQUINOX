using Cosmos.System;
using Cosmos.System.FileSystem;
using Cosmos.System.FileSystem.VFS;
using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Console = System.Console;
using Sys = Cosmos.System;
using EQUINOX.Display;

namespace EQUINOX.Features
{
    public class GeneralFunc
    {
        public string echo(string[] words)
        {
            string output = string.Join(" ", words);
            return output;
        }

        public void Help()
        {
            Console.WriteLine();
            Console.WriteLine("System Commands:");
            Console.WriteLine("- help");
            Console.WriteLine("- fs-help");
            Console.WriteLine("- echo <text>");
            Console.WriteLine("- wordle");
            Console.WriteLine("- tictactoe");    
            Console.WriteLine("- calc");
            Console.WriteLine("- reboot");
            Console.WriteLine("- shutdown");
            Console.WriteLine("- canvas");
            Console.WriteLine();
        }

        // File System Console
        public void FsHelp()
        {
            Console.WriteLine("File / Directory management commands:");
            Console.WriteLine("- pwd                 : Show current working directory.");
            Console.WriteLine("- ls [path]           : List entries (current or specified path).");
            Console.WriteLine("- mkdir <path>        : Create directory.");
            Console.WriteLine("- touch <path>        : Create empty file or update timestamp.");
            Console.WriteLine("- write <path> <text> : Overwrite file content.");
            Console.WriteLine("- append <path> <text>: Append to file.");
            Console.WriteLine("- cat <path>          : Display file contents.");
            Console.WriteLine("- mv <path> <newname> : Rename (same directory).");
            Console.WriteLine("- cp <src> <dest>     : Copy file or directory (dest may be existing dir or new path).");
            Console.WriteLine("- rm <path>           : Delete file or empty directory.");
            Console.WriteLine("- cd <path>           : Change directory (supports .. and absolute).");
            Console.WriteLine("- find <term>         : Search names containing term.");
            Console.WriteLine("- fs-save <vfsPath>   : Persist all (e.g. 0:\\equifs.dat).");
            Console.WriteLine("- fs-load <vfsPath>   : Load persisted state.");
            Console.WriteLine("- vfs-cat <diskPath>  : Show raw persistence file.");
            Console.WriteLine();
        }
        public void LaunchTicTacToe()
        {
            Console.WriteLine("Launching Tic-Tac-Toe (Console Mode)...");
            try
            {
                TicTacToe game = new TicTacToe();
                game.Run();

                Console.Clear();
                Console.WriteLine("Returned to System.");
                Help();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error launching game: " + e.Message);
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
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Returned to Text Mode.");
                Console.ForegroundColor = ConsoleColor.White;
                Help();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error launching game: " + e.Message);
            }
        }

    }
}
