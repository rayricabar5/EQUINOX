using Cosmos.HAL.Audio;
using Cosmos.HAL.BlockDevice.Registers;
using Cosmos.HAL.Drivers.PCI.Audio;
using Cosmos.System;
using Cosmos.System.Audio;
using Cosmos.System.Audio.IO;
using Cosmos.System.FileSystem;
using Cosmos.System.FileSystem.VFS;
using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;
using EQUINOX.Display;
using EQUINOX.Features;
using IL2CPU.API.Attribs;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Console = System.Console;
using Sys = Cosmos.System;
using EQUINOX.Audio;


namespace EQUINOX
{
    public class Kernel : Sys.Kernel
    {

        // FS NEW: file system (manual persistence)
        private readonly FileSystem _fs = new FileSystem();
        private bool logged_in = false;

        // VFS NEW
        private CosmosVFS _vfs;

        //GUI
        public static GUI gui;
        public static Piano piano;
        public static Voice voice;
        public LaunchGUI launcher;

        //GeneralFunc
        public GeneralFunc general;
        public AccountSystem account;

        //Math
        public Calculator calculator;

        protected override void BeforeRun()
        {
            _vfs = new CosmosVFS();
            VFSManager.RegisterVFS(_vfs);

            Console.WriteLine();
            Console.WriteLine("Equinox Booted.");
            Console.WriteLine();
            Console.WriteLine("For the list of functions, type \" help \"");
            Console.WriteLine();

            try
            {
                voice.VoiceOver();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Audio initialization failed:");
                Console.WriteLine(ex.Message);
            }

            // Check for persistent volume
            if (Directory.Exists(@"0:\"))
            {
                Console.WriteLine("Volume 0:\\ available for persistence.");
            }
            else
            {
                Console.WriteLine("Warning: 0:\\ not found. Create or mount a disk if persistence is needed.");
            }

            logged_in = account.CreateAccount();
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
                Console.Write("> ");

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
                    case "fs-help": // This shows the file management commands
                        general.FsHelp();
                        break;
                    case "echo":
                        Console.WriteLine(general.echo(args));
                        break;
                    case "calc":
                        calculator.Calculate(args);
                        break;
                    case "reboot":
                        Cosmos.System.Power.Reboot();
                        break;
                    case "shutdown":
                        Cosmos.System.Power.Shutdown();
                        break;
                    case "canvas":
                        launcher.CanvasCreate();
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
                    case "launch-gui":
                        Console.WriteLine(launcher.LaunchDisplay("GUI", args));
                        break;
                    case "piano":
                        Console.WriteLine(launcher.Piano("Piano", args));
                        break;
                    case "user":
                        account.ViewUsers();
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
    }
}

