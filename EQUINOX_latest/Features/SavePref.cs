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
    public class SavePref
    {
        private const string PrefFile = @"0:\preferences.txt";
        public void SavePreference(string theme)
        {
            // Ensure drive exists
            if (!Directory.Exists(@"0:\"))
            {
                return;
            }

            File.WriteAllText(PrefFile, "");
            RegisterPref(theme);
        }

        private void RegisterPref(string theme)
        {
            // Store as simple text: username:password
            File.AppendAllText(PrefFile, theme);
            return;
        }

        public string ReadPref()
        {
            // Ensure drive exists
            if (!Directory.Exists(@"0:\"))
            {
                return "";
            }
            if (!File.Exists(PrefFile))
            {
                return "";
            }

            string fileContent = File.ReadAllText(PrefFile);
            return fileContent;
        }

        public void PrefInit()
        {
            // Ensure drive exists
            if (!Directory.Exists(@"0:\"))
            {
                return;
            }

            if (!File.Exists(PrefFile))
            {
                Console.WriteLine("No User Preferences database found. Creating a new one...");
                File.WriteAllText(PrefFile, "");
            }
            
            return;
        }
    }
}
