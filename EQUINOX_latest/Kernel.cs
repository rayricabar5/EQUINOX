using Cosmos.System.FileSystem;
using Cosmos.System.FileSystem.VFS;
using EQUINOX.Audio;
using EQUINOX.Display;
using EQUINOX.Features;
using System;
using System.IO;
using Console = System.Console;
using Sys = Cosmos.System;

namespace EQUINOX
{
    public class Kernel : Sys.Kernel
    {
        // FS NEW: file system (manual persistence)
        private readonly FileSystem _fs = new FileSystem();
        private bool logged_in = false;

        // VFS NEW
        private CosmosVFS _vfs;

        // GUI
        public static GUI gui;
        public static Piano piano;
        public static Voice voice;
        public LaunchGUI launcher;

        // GeneralFunc (Holds Boot Animation & Commands)
        public GeneralFunc general;
        public AccountSystem account;

        // Math
        public Calculator calculator;

        protected override void BeforeRun()
        {
            // Initialize File System
            _vfs = new CosmosVFS();
            VFSManager.RegisterVFS(_vfs);

            // Initialize General Functions
            general = new GeneralFunc();

            // Play Boot Animation
            general.BootAnimation();

            // Initialize other systems
            launcher = new LaunchGUI();
            calculator = new Calculator();
            account = new AccountSystem();
            voice = new Voice();

            Console.WriteLine();
            Console.WriteLine("IndieOS Booted.");

            try
            {
                // Ensure 'voice' is initialized
                if (voice != null)
                {
                    voice.VoiceOver();
                }
            }
            catch (Exception ex)
            {
                PrintFileSystemError("Audio initialization failed.");
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("Details: " + ex.Message);
                Console.ResetColor();
            }

            // Check for persistent volume
            if (Directory.Exists(@"0:\"))
            {
                PrintSystemSuccess("Volume 0:\\ detected and mounted.");
            }
            else
            {
                PrintSystemWarning("Volume 0:\\ not found.");
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("          (Persistence is disabled. Mount a disk to fix.)");
                Console.ResetColor();
            }

            // Ensure account system is initialized before use
            if (account != null)
            {
                logged_in = account.CreateAccount();
            }
            else
            {
                // Temporary bypass if account system isn't set up yet
                logged_in = true;
            }
        }

        protected override void Run()
        {
            if (logged_in)
            {
                if (Kernel.gui != null)
                {
                    Kernel.gui.HandleGUIinputs();
                    return;
                }

                if (Kernel.piano != null)
                {
                    Kernel.piano.HandleGUIinputs();
                    return;
                }

                string choice;

                Console.WriteLine();
                // Updated Prompt
                Console.Write("IndieOS> ");

                choice = Console.ReadLine();

                commandHandler(choice);
            }
            else if (!logged_in)
            {
                Cosmos.System.Power.Shutdown();
            }
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
                        general.Help();
                        break;
                    case "fs-help":
                        general.FsHelp();
                        break;
                    case "cls":
                    case "clear":
                        Console.Clear();
                        general.DrawLogo(); // Redraw Logo on Clear
                        break;
                    case "echo":
                        Console.WriteLine(general.echo(args));
                        break;
                    case "wordle":
                        general.LaunchWordle();
                        break;
                    case "tictactoe":
                        general.LaunchTicTacToe();
                        break;
                    case "calc":
                        if (calculator != null) calculator.Calculate(args);
                        else PrintSystemWarning("Calculator module not initialized.");
                        break;
                    case "reboot":
                        Cosmos.System.Power.Reboot();
                        break;
                    case "shutdown":
                        Cosmos.System.Power.Shutdown();
                        break;
                    case "canvas":
                        if (launcher != null) launcher.CanvasCreate();
                        else PrintSystemWarning("Launcher module not initialized.");
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
                        if (args.Length < 1) { PrintUsage("mkdir <path>"); break; }
                        _fs.Mkdir(args[0], out var mkdirMsg);
                        Console.WriteLine(mkdirMsg);
                        break;
                    case "touch":
                        if (args.Length < 1) { PrintUsage("touch <path>"); break; }
                        _fs.Touch(args[0], out var touchMsg);
                        Console.WriteLine(touchMsg);
                        break;
                    case "cat":
                        if (args.Length < 1) { PrintUsage("cat <path>"); break; }
                        if (_fs.ReadFile(args[0], out var content)) Console.WriteLine(content);
                        else PrintFileSystemError("File not found or is a directory.");
                        break;
                    case "write":
                        if (args.Length < 2) { PrintUsage("write <path> <text>"); break; }
                        {
                            var path = args[0];
                            var text = string.Join(" ", args, 1, args.Length - 1);
                            _fs.WriteFile(path, text, append: false, out var wmsg);
                            Console.WriteLine(wmsg);
                        }
                        break;
                    case "append":
                        if (args.Length < 2) { PrintUsage("append <path> <text>"); break; }
                        {
                            var path = args[0];
                            var text = string.Join(" ", args, 1, args.Length - 1);
                            _fs.WriteFile(path, text, append: true, out var amsg);
                            Console.WriteLine(amsg);
                        }
                        break;
                    case "rm":
                        if (args.Length < 1) { PrintUsage("rm <path>"); break; }
                        _fs.Delete(args[0], out var dmsg);
                        Console.WriteLine(dmsg);
                        break;
                    case "cd":
                        if (args.Length < 1) { PrintUsage("cd <path>"); break; }
                        _fs.Cd(args[0], out var cdmsg);
                        Console.WriteLine(cdmsg);
                        break;
                    case "mv":
                        if (args.Length < 2) { PrintUsage("mv <path> <newname>"); break; }
                        _fs.Rename(args[0], args[1], out var mvmsg);
                        Console.WriteLine(mvmsg);
                        break;
                    case "cp":
                        if (args.Length < 2) { PrintUsage("cp <srcPath> <destPathOrDir>"); break; }
                        if (_fs.Copy(args[0], args[1], out var cpMsg)) Console.WriteLine(cpMsg);
                        else PrintFileSystemError(cpMsg);
                        break;
                    case "find":
                        if (args.Length < 1) { PrintUsage("find <term>"); break; }
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
                        if (args.Length < 1) { PrintUsage("fs-save <vfsPath>"); break; }
                        if (_fs.SaveToVfs(args[0], out var sMsg)) Console.WriteLine(sMsg);
                        else PrintFileSystemError($"Save failed: {sMsg}");
                        break;
                    case "fs-load":
                        if (args.Length < 1) { PrintUsage("fs-load <vfsPath>"); break; }
                        if (_fs.LoadFromVfs(args[0], out var lMsg)) Console.WriteLine(lMsg);
                        else PrintFileSystemError($"Load failed: {lMsg}");
                        break;
                    case "vfs-cat":
                        if (args.Length < 1) { PrintUsage("vfs-cat <diskFilePath>"); break; }
                        try
                        {
                            var diskPath = args[0];
                            if (!File.Exists(diskPath))
                            {
                                PrintFileSystemError("Disk file not found.");
                                break;
                            }
                            var rawLines = File.ReadAllLines(diskPath);
                            foreach (var line in rawLines)
                                Console.WriteLine(line);
                        }
                        catch (Exception ex)
                        {
                            PrintFileSystemError("Read failed: " + ex.Message);
                        }
                        break;
                    case "vfs-stat":
                        if (args.Length < 1) { PrintUsage("vfs-stat <diskFilePath>"); break; }
                        try
                        {
                            var p = args[0];
                            if (!File.Exists(p))
                            {
                                PrintFileSystemError("Missing: " + p);
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
                            PrintFileSystemError("Stat failed: " + ex.Message);
                        }
                        break;
                    case "launch-gui":
                        if (launcher != null) Console.WriteLine(launcher.LaunchDisplay("GUI", args));
                        else PrintSystemWarning("Launcher module not initialized.");
                        break;
                    case "piano":
                        if (launcher != null) Console.WriteLine(launcher.Piano("Piano", args));
                        else PrintSystemWarning("Launcher module not initialized.");
                        break;
                    case "user":
                        if (account != null) account.ViewUsers();
                        else PrintSystemWarning("Account system not initialized.");
                        break;
                    case "version":
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("  ");
                        Console.WriteLine("IndieOS Version 1.0.0");
                        Console.WriteLine("  ");
                        Console.ResetColor();
                        break;
                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("  [!] Unknown command.");
                        Console.ResetColor();
                        Console.WriteLine("      Type 'help' for a list of commands.");
                        break;
                }
            }

            var sections = input.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (sections.Length == 0) { return; }
            string command = sections[0].ToLower(); // Made lowercase for consistency
            string[] args = new string[sections.Length - 1];
            for (int i = 1; i < sections.Length; i++) { args[i - 1] = sections[i]; }
            main(command, args);
        }

        // --- Visual Helper Methods ---
        private void PrintSystemSuccess(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("[ OK ] ");
            Console.ResetColor();
            Console.WriteLine(message);
        }

        private void PrintSystemWarning(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("[ WARNING ] ");
            Console.ResetColor();
            Console.WriteLine(message);
        }

        private void PrintFileSystemError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("[ ERROR ] ");
            Console.ResetColor();
            Console.WriteLine(message);
        }

        private void PrintUsage(string usage)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("[ USAGE ] ");
            Console.ResetColor();
            Console.WriteLine(usage);
        }
    }
}