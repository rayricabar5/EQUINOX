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

namespace EQUINOX
{
    public class Kernel : Sys.Kernel
    {
        // FS NEW: file system (manual persistence)
        private readonly FileSystem _fs = new FileSystem();

        // VFS NEW
        private CosmosVFS _vfs;

        //protected override void Start()
        //{
        //    CanvasCreate();
        //}

        protected override void BeforeRun()
        {
            _vfs = new CosmosVFS();
            VFSManager.RegisterVFS(_vfs);

            Console.WriteLine();
            Console.WriteLine("Equinox Booted.");
            Console.WriteLine();
            Console.WriteLine("For the list of function, type \" help \"");
            Console.WriteLine();

            // Check the Availabile Volume\s for File Persistence
            if (Directory.Exists(@"0:\"))
            {
                Console.WriteLine("Volume 0:\\ available for persistence.");
            }
            else
            {
                Console.WriteLine("Warning: 0:\\ not found. Create or mount a disk if persistence is needed.");
            }
        }

        protected override void Run()
        {
            string choice;

            Console.WriteLine();
            Console.Write("> ");

            choice = Console.ReadLine();

            commandHandler(choice);
        }

        protected override void AfterRun()
        {
            Console.WriteLine("Farewell");
        }

        void commandHandler(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return;
            }

            void main(string command, string[] args)
            {
                switch (command)
                {
                    case "help":
                        Help();
                        break;
                    case "fs-help": // This shows the file management commands
                        FsHelp();
                        break;
                    case "echo":
                        Console.WriteLine(echo(args));
                        break;
                    case "calc":
                        Calculate(args);
                        break;
                    case "reboot":
                        Cosmos.System.Power.Reboot();
                        break;
                    case "shutdown":
                        Cosmos.System.Power.Shutdown();
                        break;
                    case "canvas":
                        CanvasCreate();
                        break;

                    // File Management Commands
                    case "pwd":
                        Console.WriteLine(_fs.Pwd());
                        break;
                    case "ls":
                        {
                            var list = _fs.Ls(args.Length > 0 ? args[0] : null);
                            foreach (var l in list) Console.WriteLine(l);
                        }
                        break;
                    case "mkdir":
                        if (args.Length < 1) { Console.WriteLine("Usage: mkdir <path>"); break; }
                        _fs.Mkdir(args[0], out var mkdirMsg);
                        Console.WriteLine(mkdirMsg);
                        break;
                    case "touch":
                        if (args.Length < 1) { Console.WriteLine("Usage: touch <path>"); break; }
                        _fs.Touch(args[0], out var touchMsg);
                        Console.WriteLine(touchMsg);
                        break;
                    case "cat":
                        if (args.Length < 1) { Console.WriteLine("Usage: cat <path>"); break; }
                        if (_fs.ReadFile(args[0], out var content)) Console.WriteLine(content);
                        else Console.WriteLine("File not found or is a directory.");
                        break;
                    case "write":
                        if (args.Length < 2) { Console.WriteLine("Usage: write <path> <text>"); break; }
                        {
                            var path = args[0];
                            var text = string.Join(" ", args, 1, args.Length - 1);
                            _fs.WriteFile(path, text, append: false, out var wmsg);
                            Console.WriteLine(wmsg);
                        }
                        break;
                    case "append":
                        if (args.Length < 2) { Console.WriteLine("Usage: append <path> <text>"); break; }
                        {
                            var path = args[0];
                            var text = string.Join(" ", args, 1, args.Length - 1);
                            _fs.WriteFile(path, text, append: true, out var amsg);
                            Console.WriteLine(amsg);
                        }
                        break;
                    case "rm":
                        if (args.Length < 1) { Console.WriteLine("Usage: rm <path>"); break; }
                        _fs.Delete(args[0], out var dmsg);
                        Console.WriteLine(dmsg);
                        break;
                    case "cd":
                        if (args.Length < 1) { Console.WriteLine("Usage: cd <path>"); break; }
                        _fs.Cd(args[0], out var cdmsg);
                        Console.WriteLine(cdmsg);
                        break;
                    case "mv":
                        if (args.Length < 2) { Console.WriteLine("Usage: mv <path> <newname>"); break; }
                        _fs.Rename(args[0], args[1], out var mvmsg);
                        Console.WriteLine(mvmsg);
                        break;
                    case "cp":
                        if (args.Length < 2) { Console.WriteLine("Usage: cp <srcPath> <destPathOrDir>"); break; }
                        if (_fs.Copy(args[0], args[1], out var cpMsg)) Console.WriteLine(cpMsg);
                        else Console.WriteLine(cpMsg);
                        break;
                    case "find":
                        if (args.Length < 1) { Console.WriteLine("Usage: find <term>"); break; }
                        {
                            var hits = _fs.Find(args[0]);
                            if (hits.Length == 0) Console.WriteLine("No matches.");
                            else
                            {
                                Console.WriteLine("Matches (" + hits.Length + "):");
                                for (int i = 0; i < hits.Length; i++)
                                    Console.WriteLine(hits[i]);
                            }
                        }
                        break;
                    case "fs-save":
                        if (args.Length < 1) { Console.WriteLine("Usage: fs-save <vfsPath> (e.g. 0:\\equifs.dat)"); break; }
                        if (_fs.SaveToVfs(args[0], out var sMsg)) Console.WriteLine(sMsg);
                        else Console.WriteLine($"Save failed: {sMsg}");
                        break;
                    case "fs-load":
                        if (args.Length < 1) { Console.WriteLine("Usage: fs-load <vfsPath>"); break; }
                        if (_fs.LoadFromVfs(args[0], out var lMsg)) Console.WriteLine(lMsg);
                        else Console.WriteLine($"Load failed: {lMsg}");
                        break;
                    case "vfs-cat":
                        if (args.Length < 1) { Console.WriteLine("Usage: vfs-cat <diskFilePath>"); break; }
                        try
                        {
                            var diskPath = args[0];
                            if (!File.Exists(diskPath))
                            {
                                Console.WriteLine("Disk file not found.");
                                break;
                            }
                            var rawLines = File.ReadAllLines(diskPath);
                            foreach (var line in rawLines)
                                Console.WriteLine(line);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Read failed: " + ex.Message);
                        }
                        break;
                    case "vfs-stat":
                        if (args.Length < 1) { Console.WriteLine("Usage: vfs-stat <diskFilePath>"); break; }
                        try
                        {
                            var p = args[0];
                            if (!File.Exists(p))
                            {
                                Console.WriteLine("Missing: " + p);
                                break;
                            }
                            var bytes = File.ReadAllBytes(p);
                            int lineCount = 0;
                            try { lineCount = File.ReadAllLines(p).Length; } catch { }
                            Console.WriteLine("Exists: " + p);
                            Console.WriteLine("Size  : " + bytes.Length + " bytes");
                            Console.WriteLine("Lines : " + lineCount);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Stat failed: " + ex.Message);
                        }
                        break;
                    default:
                        Console.WriteLine("Invalid command");
                        break;
                }
            }

            var sections = input.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (sections.Length == 0) { return; }
            string command = sections[0];
            string[] args = new string[sections.Length - 1];
            for (int i = 1; i < sections.Length; i++) { args[i - 1] = sections[i]; }
            main(command, args);
        }

        string echo(string[] words)
        {
            string output = string.Join(" ", words);
            return output;
        }

        void Help()
        {
            Console.WriteLine();
            Console.WriteLine("System Commands:");
            Console.WriteLine("- help");
            Console.WriteLine("- fs-help");
            Console.WriteLine("- echo <text>");
            Console.WriteLine("- calc");
            Console.WriteLine("- reboot");
            Console.WriteLine("- shutdown");
            Console.WriteLine("- canvas");
            Console.WriteLine();
        }

        // File System Console
        void FsHelp()
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

        float Calculate(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: calc <operation> <num1> <num2> ...");
                Console.WriteLine("Operations: -a (add), -s (subtract), -m (multiply), -d (divide)");
                return 0;
            }

            float[] numbers = new float[args.Length - 1];
            string operation = args[0];
            float answer;

            for (int i = 0; i < args.Length - 1; i++)
            {
                try
                {
                    numbers[i] = float.Parse(args[i + 1]);
                }
                catch (Exception err)
                {
                    Console.WriteLine(err.Message);
                    Console.WriteLine("One of the input is unable to be added");
                    Console.WriteLine($"Invalid number: {args[i + 1]}");
                    return 0;
                }
            }

            answer = numbers[0];

            if (numbers.Length == 1)
            {
                if (operation != "-a" && operation != "-s" && operation != "-m" && operation != "-d")
                {
                    Console.WriteLine($"Operation {operation} is not recognized.");
                    return 0;
                }
            }

            for (int i = 1; i < numbers.Length; i++)
            {
                switch (operation)
                {
                    case "-a":
                        answer += numbers[i];
                        break;
                    case "-s":
                        answer -= numbers[i];
                        break;
                    case "-m":
                        answer *= numbers[i];
                        break;
                    case "-d":
                        if (numbers[i] == 0)
                        {
                            Console.WriteLine("Error: Division by zero.");
                            return 0;
                        }
                        answer /= numbers[i];
                        break;
                    default:
                        Console.WriteLine($"Operation {operation} is not recognized.");
                        return 0;
                }
            }

            Console.WriteLine(answer);
            return answer;
        }

        void CanvasCreate()
        {
            Canvas canvas;
            Pen yellow = new Pen(Color.Yellow);
            var font = PCScreenFont.Default;

            canvas = FullScreenCanvas.GetFullScreenCanvas(new Mode(1024, 768, ColorDepth.ColorDepth32));
            canvas.Clear(Color.DarkSlateGray);

            // E
            canvas.DrawFilledRectangle(yellow, 50, 50, 100, 30);
            canvas.DrawFilledRectangle(yellow, 50, 135, 100, 30);
            canvas.DrawFilledRectangle(yellow, 50, 220, 100, 30);
            canvas.DrawFilledRectangle(yellow, 50, 50, 30, 200);

            // Q
            canvas.DrawFilledRectangle(yellow, 170, 50, 95, 30);
            canvas.DrawFilledRectangle(yellow, 170, 50, 30, 200);
            canvas.DrawFilledRectangle(yellow, 265, 50, 30, 200);
            canvas.DrawFilledRectangle(yellow, 170, 220, 95, 30);
            canvas.DrawFilledRectangle(yellow, 230, 200, 20, 70);

            // U
            canvas.DrawFilledRectangle(yellow, 325, 50, 30, 200);
            canvas.DrawFilledRectangle(yellow, 420, 50, 30, 200);
            canvas.DrawFilledRectangle(yellow, 325, 220, 95, 30);

            // I
            canvas.DrawFilledRectangle(yellow, 475, 50, 30, 200);

            // N
            canvas.DrawFilledRectangle(yellow, 530, 50, 30, 200);
            canvas.DrawFilledRectangle(yellow, 620, 50, 30, 200);
            canvas.DrawFilledRectangle(yellow, 560, 80, 20, 42);
            canvas.DrawFilledRectangle(yellow, 575, 120, 22, 47);
            canvas.DrawFilledRectangle(yellow, 595, 160, 22, 47);
            canvas.DrawFilledRectangle(yellow, 615, 200, 28, 50);

            // O
            canvas.DrawFilledRectangle(yellow, 670, 50, 100, 30);
            canvas.DrawFilledRectangle(yellow, 670, 50, 30, 200);
            canvas.DrawFilledRectangle(yellow, 770, 50, 30, 200);
            canvas.DrawFilledRectangle(yellow, 670, 220, 100, 30);

            // X
            canvas.DrawFilledRectangle(yellow, 830, 50, 25, 46);
            canvas.DrawFilledRectangle(yellow, 850, 90, 25, 46);
            canvas.DrawFilledRectangle(yellow, 870, 130, 25, 46);
            canvas.DrawFilledRectangle(yellow, 890, 170, 25, 46);
            canvas.DrawFilledRectangle(yellow, 910, 200, 25, 50);

            canvas.DrawFilledRectangle(yellow, 830, 200, 22, 50);
            canvas.DrawFilledRectangle(yellow, 850, 170, 22, 46);
            canvas.DrawFilledRectangle(yellow, 890, 90, 22, 46);
            canvas.DrawFilledRectangle(yellow, 910, 50, 22, 50);


            canvas.Display();
            Console.ReadKey();
            canvas.Disable();
        }
    }
}
