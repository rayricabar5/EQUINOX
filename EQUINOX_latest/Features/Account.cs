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

        public bool CreateAccount()
        {
            //
            // 1. Ensure drive exists
            //
            if (!Directory.Exists(@"0:\"))
            {
                Console.WriteLine("Warning: 0:\\ not found. Cannot use account features.");
                return true;
            }

            //
            // 2. If no users file → create one and ask user to register
            //
            if (!File.Exists(UserDbPath))
            {
                Console.WriteLine("No account database found. Creating a new one...");
                File.WriteAllText(UserDbPath, "");   // avoid IL2CPU zero-byte bug
                RegisterNewUser();
            }

            //
            // 3. If file exists → go to login
            //
            return LoginUser();
        }

        private bool RegisterNewUser()
        {
            Console.WriteLine("=== Create New Account ===");
            Console.Write("Enter username: ");
            string user = Console.ReadLine();

            Console.Write("Enter password: ");
            string pass = Console.ReadLine();

            // Store as simple text: username:password
            File.AppendAllText(UserDbPath, $"{user}:{pass}\n");

            Console.WriteLine("Account created successfully!");

            return true;
        }

        private bool LoginUser()
        {
            int tries = 0;

            while (tries < 3)
            {
                Console.WriteLine("=== Login ===");

                Console.Write("Username: ");
                string userInput = Console.ReadLine();

                Console.Write("Password: ");
                string passInput = Console.ReadLine();


                // Read safely (Cosmos-friendly)
                string fileContent = File.ReadAllText(UserDbPath);
                var lines = fileContent.Split('\n');

                foreach (var line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    var parts = line.Split(':');
                    if (parts.Length != 2) continue;

                    string savedUser = parts[0];
                    string savedPass = parts[1];

                    if (savedUser == userInput && savedPass == passInput)
                    {
                        Console.WriteLine("Login successful! Welcome, " + savedUser);
                        return true;
                    }
                    else
                    {
                        tries++;
                        Console.WriteLine("InvaLid Username or password");
                    }
                }
            }

            Console.WriteLine("User log in failed. Shutting Down");
            return false;
        }

        public void ViewUsers()
        {
            if (!Directory.Exists(@"0:\"))
            {
                Console.WriteLine("Warning: 0:\\ not found. Cannot use account features.");
                return;
            }

            if (File.Exists(@"0:\users.txt"))
                Console.WriteLine(File.ReadAllText(@"0:\users.txt"));
            else
                Console.WriteLine("No users file found.");
        }

    }
}
