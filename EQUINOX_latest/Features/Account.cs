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

namespace EQUINOX.Features
{
    public class AccountSystem
    {
        private const string UserDbPath = @"0:\users.txt";

        // --- VISUAL THEME ---
        private const ConsoleColor ClrBorder = ConsoleColor.Cyan;
        private const ConsoleColor ClrText = ConsoleColor.White;
        private const ConsoleColor ClrLabel = ConsoleColor.Gray;
        private const ConsoleColor ClrError = ConsoleColor.Red;
        private const ConsoleColor ClrSuccess = ConsoleColor.Green;
        private const ConsoleColor ClrInput = ConsoleColor.Yellow;

        public bool CreateAccount()
        {
            // 1. Ensure drive exists
            if (!Directory.Exists(@"0:\"))
            {
                PrintWarning("Volume 0:\\ not found. Persistence disabled.");
                return true; // Bypass login if no drive
            }

            // 2. If no users file -> Setup Mode
            if (!File.Exists(UserDbPath))
            {
                File.WriteAllText(UserDbPath, ""); // Avoid IL2CPU zero-byte bug
                return RegisterNewUser();
            }

            // 3. Normal Login Mode
            return LoginUser();
        }

        private bool RegisterNewUser()
        {
            DrawScreen("SYSTEM SETUP");

            int boxWidth = 44;
            int startY = 11;
            int centerX = (Console.WindowWidth - boxWidth) / 2;

            DrawBox(boxWidth, 10, startY);

            // Instructions
            Console.SetCursorPosition(centerX + 2, startY + 2);
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write("Create the Administrator account.");

            // Username Input
            Console.SetCursorPosition(centerX + 4, startY + 4);
            Console.ForegroundColor = ClrLabel;
            Console.Write("USERNAME: ");
            Console.ForegroundColor = ClrInput;
            string user = Console.ReadLine();

            // Password Input
            Console.SetCursorPosition(centerX + 4, startY + 6);
            Console.ForegroundColor = ClrLabel;
            Console.Write("PASSWORD: ");
            Console.ForegroundColor = ClrInput;
            string pass = ReadPassword();

            // Saving
            try
            {
                File.AppendAllText(UserDbPath, $"{user}:{pass}\n");

                Console.SetCursorPosition(centerX + 4, startY + 8);
                Console.ForegroundColor = ClrSuccess;
                Console.Write("[ ACCOUNT CREATED SUCCESSFULLY ]");
                Console.ResetColor();

                System.Threading.Thread.Sleep(1500); // Pause for effect

                // Show standard OS screen before returning
                ShowSystemWelcome();
                return true;
            }
            catch
            {
                PrintError("Failed to write to disk.");
                return false;
            }
        }

        private bool LoginUser()
        {
            int tries = 0;
            while (tries < 3)
            {
                DrawScreen("SECURE LOGIN");

                int boxWidth = 44;
                int startY = 11;
                int centerX = (Console.WindowWidth - boxWidth) / 2;

                DrawBox(boxWidth, 9, startY);

                // Username
                Console.SetCursorPosition(centerX + 4, startY + 3);
                Console.ForegroundColor = ClrLabel;
                Console.Write("USERNAME: ");
                Console.ForegroundColor = ClrInput;
                string userInput = Console.ReadLine();

                // Password
                Console.SetCursorPosition(centerX + 4, startY + 5);
                Console.ForegroundColor = ClrLabel;
                Console.Write("PASSWORD: ");
                Console.ForegroundColor = ClrInput;
                string passInput = ReadPassword();

                // Validation
                if (ValidateCredentials(userInput, passInput))
                {
                    Console.SetCursorPosition(centerX + 4, startY + 7);
                    Console.ForegroundColor = ClrSuccess;
                    Console.Write("     [ ACCESS GRANTED ]     ");
                    Console.ResetColor();

                    System.Threading.Thread.Sleep(1000); // Loading delay

                    // Show standard OS screen before returning
                    ShowSystemWelcome();
                    return true;
                }
                else
                {
                    tries++;
                    Console.SetCursorPosition(centerX + 4, startY + 7);
                    Console.ForegroundColor = ClrError;
                    Console.Write("     [ ACCESS DENIED ]      ");
                    Console.ResetColor();
                    System.Threading.Thread.Sleep(1000); // Penalty delay
                }
            }

            Console.Clear();
            DrawScreen("SYSTEM LOCKED");
            PrintError("Too many failed attempts. System Halted.");
            return false;
        }

        private bool ValidateCredentials(string user, string pass)
        {
            try
            {
                string content = File.ReadAllText(UserDbPath);
                var lines = content.Split('\n');

                foreach (var line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    var parts = line.Split(':');
                    if (parts.Length != 2) continue;

                    if (parts[0] == user && parts[1] == pass)
                    {
                        return true;
                    }
                }
            }
            catch
            {
                return false;
            }
            return false;
        }

        // --- HELPER: Secure Password Input (Cosmos-friendly) ---
        private string ReadPassword()
        {
            var sb = new StringBuilder();

            while (true)
            {
                if (!Cosmos.System.KeyboardManager.TryReadKey(out var keyInfo))
                {
                    // No key available; yield briefly
                    continue;
                }

                // Enter finalizes input
                if (keyInfo.Key == ConsoleKeyEx.Enter)
                {
                    Console.WriteLine();
                    break;
                }

                // Backspace handling
                if (keyInfo.Key == ConsoleKeyEx.Backspace)
                {
                    if (sb.Length > 0)
                    {
                        sb.Remove(sb.Length - 1, 1);

                        int curLeft = Console.CursorLeft;
                        if (curLeft > 0)
                        {
                            Console.SetCursorPosition(curLeft - 1, Console.CursorTop);
                            Console.Write(' ');
                            Console.SetCursorPosition(curLeft - 1, Console.CursorTop);
                        }
                    }
                    // If buffer empty, do nothing
                    continue;
                }

                // Ignore other control keys
                if (char.IsControl(keyInfo.KeyChar))
                {
                    continue;
                }

                // Normal character
                sb.Append(keyInfo.KeyChar);
                Console.Write('*');
            }

            return sb.ToString();
        }

        // --- VISUAL HELPERS ---

        // This replicates the "CLS" command output
        private void ShowSystemWelcome()
        {
            Console.Clear();
            DrawCenteredLogo();
            Console.WriteLine("\n");

            string welcome = "Welcome to IndieOS v1.0";
            int pad = (Console.WindowWidth - welcome.Length) / 2;
            if (pad < 0) pad = 0;
            Console.WriteLine(new string(' ', pad) + welcome);

            Console.ForegroundColor = ConsoleColor.DarkGray;
            string help = "Type 'help' for commands.";
            pad = (Console.WindowWidth - help.Length) / 2;
            if (pad < 0) pad = 0;
            Console.WriteLine(new string(' ', pad) + help);

            Console.ResetColor();
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

        private void DrawScreen(string title)
        {
            Console.Clear();
            DrawCenteredLogo();
            Console.WriteLine();
            DrawHeaderLine($" {title} ");
        }

        private void DrawHeaderLine(string text)
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

        private void DrawBox(int width, int height, int startY)
        {
            int startX = (Console.WindowWidth - width) / 2;
            if (startX < 0) startX = 0;

            Console.ForegroundColor = ClrBorder;

            // Top
            Console.SetCursorPosition(startX, startY);
            Console.Write("+" + new string('-', width - 2) + "+");

            // Sides
            for (int i = 1; i < height - 1; i++)
            {
                Console.SetCursorPosition(startX, startY + i);
                Console.Write("|");
                Console.SetCursorPosition(startX + width - 1, startY + i);
                Console.Write("|");
            }

            // Bottom
            Console.SetCursorPosition(startX, startY + height - 1);
            Console.Write("+" + new string('-', width - 2) + "+");

            Console.ResetColor();
        }

        public void ViewUsers()
        {
            if (!Directory.Exists(@"0:\"))
            {
                PrintWarning("No persistent storage found.");
                return;
            }

            Console.WriteLine();
            DrawHeaderLine(" REGISTERED USERS ");
            Console.WriteLine();

            if (File.Exists(UserDbPath))
            {
                try
                {
                    string[] lines = File.ReadAllLines(UserDbPath);
                    foreach (var line in lines)
                    {
                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            var parts = line.Split(':');
                            if (parts.Length > 0)
                            {
                                Console.ForegroundColor = ClrBorder;
                                Console.Write("  [-] ");
                                Console.ForegroundColor = ClrText;
                                Console.WriteLine(parts[0]);
                            }
                        }
                    }
                }
                catch { PrintError("Could not read database."); }
            }
            else
            {
                Console.WriteLine("  No user database found.");
            }
            Console.ResetColor();
            Console.WriteLine();
        }

        private void PrintWarning(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(" [!] " + msg);
            Console.ResetColor();
        }

        private void PrintError(string msg)
        {
            Console.ForegroundColor = ClrError;
            Console.WriteLine(" [X] " + msg);
            Console.ResetColor();
        }
    }
}