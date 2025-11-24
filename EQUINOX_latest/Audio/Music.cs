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
using EQUINOX.Features;
using IL2CPU.API.Attribs;

namespace EQUINOX.Audio
{
    public class Music
    {
            public static Dictionary<string, uint> frequencies = new Dictionary<string, uint>
        {
            {"A3", 220 },
            {"B3", 247 },
            {"C", 261},
            {"D", 294},
            {"E", 329},
            {"F", 349},
            {"G", 392},
            {"A", 440},
            {"B", 493},
            {"C5", 523},
        };

        public void LaLow() { PCSpeaker.Beep(Music.frequencies["A3"], 150); }
        public void TiLow() { PCSpeaker.Beep(Music.frequencies["B3"], 150); }
        public void Do() { PCSpeaker.Beep(Music.frequencies["C"], 150); }
        public void Re() { PCSpeaker.Beep(Music.frequencies["D"], 150); }
        public void Mi() { PCSpeaker.Beep(Music.frequencies["E"], 150); }
        public void Fa() { PCSpeaker.Beep(Music.frequencies["F"], 150); }
        public void So() { PCSpeaker.Beep(Music.frequencies["G"], 150); }
        public void La() { PCSpeaker.Beep(Music.frequencies["A"], 150); }
        public void Ti() { PCSpeaker.Beep(Music.frequencies["B"], 150); }
        public void DoHigh() { PCSpeaker.Beep(Music.frequencies["C5"], 150); }

        public void DoReMi()
        {
            PCSpeaker.Beep(Music.frequencies["C"], 150); // Do
            PCSpeaker.Beep(Music.frequencies["D"], 150); // Re
            PCSpeaker.Beep(Music.frequencies["E"], 150); // Mi
            PCSpeaker.Beep(Music.frequencies["F"], 150); // Fa
            PCSpeaker.Beep(Music.frequencies["G"], 150); // So
            PCSpeaker.Beep(Music.frequencies["A"], 150); // La
            PCSpeaker.Beep(Music.frequencies["B"], 150); // Ti
            PCSpeaker.Beep(Music.frequencies["C5"], 150); // Do
        }
    
        public void DoTiLa()
        {
            PCSpeaker.Beep(Music.frequencies["C5"], 150); // Do
            PCSpeaker.Beep(Music.frequencies["B"], 150); // Ti
            PCSpeaker.Beep(Music.frequencies["A"], 150); // La
            PCSpeaker.Beep(Music.frequencies["G"], 150); // So
            PCSpeaker.Beep(Music.frequencies["F"], 150); // Fa
            PCSpeaker.Beep(Music.frequencies["E"], 150); // Mi
            PCSpeaker.Beep(Music.frequencies["D"], 150); // Re
            PCSpeaker.Beep(Music.frequencies["C"], 150); // Do
        }

        public void Tagline()
        {
            
        }


    }
}
