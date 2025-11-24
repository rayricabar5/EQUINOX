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
    public class LaunchGUI
    {

        public void CanvasCreate()
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

        public string LaunchDisplay(string name, string[] args)
        {
            if (Kernel.gui != null)
            {
                return "Already have GUI";
            }

            Kernel.gui = new GUI();

            return "Launched GUI";
        }

        public string Piano(string name, string[] args)
        {
            if (Kernel.piano != null)
            {
                return "Already have GUI";
            }

            Kernel.piano = new Piano();

            return "Launched GUI";
        }


    }
}
