using Cosmos.HAL;
using Cosmos.System;
using Cosmos.System.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Console = System.Console;
using Sys = Cosmos.System;
using EQUINOX.Audio;

namespace EQUINOX.Display
{
    public class Piano
    {
        private Canvas canvas;
        private Cancel cancel;
        private Music key;
        private Color bg = Color.Brown;

        private MouseState prevMouseState;
        private List<Tuple<Sys.Graphics.Point, Color>> savedPixels;
        private UInt32 px, py;

        private Pen pen;
        private Pen gray = new Pen(Color.Gray);
        private Pen black = new Pen(Color.Black);

        bool close = false;

        public Piano()
        {
            this.canvas = FullScreenCanvas.GetFullScreenCanvas();
            this.canvas.Clear(bg);
            this.cancel = new Cancel(canvas);

            this.pen = new Pen(Color.White);
            this.prevMouseState = MouseState.None;

            this.px = 5;
            this.py = 5;
            this.savedPixels = new List<Tuple<Sys.Graphics.Point, Color>>();

            this.cancel = new Cancel(this.canvas);
            this.key = new Music();

            MouseManager.ScreenHeight = (UInt32)this.canvas.Mode.Rows;
            MouseManager.ScreenWidth = (UInt32)this.canvas.Mode.Columns;

            int startX = 212;
            int y = 100;
            int keyW = 75;
            int keyH = 375;

            // A3
            this.canvas.DrawFilledRectangle(gray, startX - keyW * 2, y, keyW, keyH);
            this.canvas.DrawRectangle(black, startX - keyW * 2, y, keyW, keyH);

            // B3
            this.canvas.DrawFilledRectangle(gray, startX - keyW, y, keyW, keyH);
            this.canvas.DrawRectangle(black, startX - keyW, y, keyW, keyH);

            // C
            this.canvas.DrawFilledRectangle(gray, startX, y, keyW, keyH);
            this.canvas.DrawRectangle(black, startX, y, keyW, keyH);

            // D
            this.canvas.DrawFilledRectangle(gray, startX + keyW, y, keyW, keyH);
            this.canvas.DrawRectangle(black, startX + keyW, y, keyW, keyH);

            // E
            this.canvas.DrawFilledRectangle(gray, startX + keyW * 2, y, keyW, keyH);
            this.canvas.DrawRectangle(black, startX + keyW * 2, y, keyW, keyH);

            // F
            this.canvas.DrawFilledRectangle(gray, startX + keyW * 3, y, keyW, keyH);
            this.canvas.DrawRectangle(black, startX + keyW * 3, y, keyW, keyH);

            // G
            this.canvas.DrawFilledRectangle(gray, startX + keyW * 4, y, keyW, keyH);
            this.canvas.DrawRectangle(black, startX + keyW * 4, y, keyW, keyH);

            // A
            this.canvas.DrawFilledRectangle(gray, startX + keyW * 5, y, keyW, keyH);
            this.canvas.DrawRectangle(black, startX + keyW * 5, y, keyW, keyH);

            // B
            this.canvas.DrawFilledRectangle(gray, startX + keyW * 6, y, keyW, keyH);
            this.canvas.DrawRectangle(black, startX + keyW * 6, y, keyW, keyH);

            // C5
            this.canvas.DrawFilledRectangle(gray, startX + keyW * 7, y, keyW, keyH);
            this.canvas.DrawRectangle(black, startX + keyW * 7, y, keyW, keyH);
        }

        public void HandleGUIinputs()
        {
            if (this.px != MouseManager.X && this.py != MouseManager.Y)
            {
            // This code moves mouse pointer based on compared prev and current positions

                //Avoid out of bounds
                if (MouseManager.X <= 2 || MouseManager.Y <= 2 || MouseManager.X >= 1000 || MouseManager.Y >= 750)
                    return;

                this.px = MouseManager.X;
                this.py = MouseManager.Y;

                Sys.Graphics.Point[] points = new Sys.Graphics.Point[]
                {
                    new Sys.Graphics.Point((Int32) MouseManager.X, (Int32)MouseManager.Y),
                    new Sys.Graphics.Point((Int32) MouseManager.X, (Int32)MouseManager.Y-1),
                    new Sys.Graphics.Point((Int32) MouseManager.X, (Int32)MouseManager.Y-2),
                    new Sys.Graphics.Point((Int32) MouseManager.X, (Int32)MouseManager.Y-3),
                    new Sys.Graphics.Point((Int32) MouseManager.X, (Int32)MouseManager.Y-4),

                    new Sys.Graphics.Point((Int32) MouseManager.X+1, (Int32)MouseManager.Y-1),
                    new Sys.Graphics.Point((Int32) MouseManager.X+2, (Int32)MouseManager.Y-2),
                    new Sys.Graphics.Point((Int32) MouseManager.X+3, (Int32)MouseManager.Y-3),
                    new Sys.Graphics.Point((Int32) MouseManager.X+4, (Int32)MouseManager.Y-4),

                    new Sys.Graphics.Point((Int32) MouseManager.X+1, (Int32)MouseManager.Y-4),
                    new Sys.Graphics.Point((Int32) MouseManager.X+2, (Int32)MouseManager.Y-4),
                    new Sys.Graphics.Point((Int32) MouseManager.X+3, (Int32)MouseManager.Y-4),
                };

                foreach (Tuple<Sys.Graphics.Point, Color> pixelData in this.savedPixels)
                {
                    this.canvas.DrawPoint(new Pen(pixelData.Item2), pixelData.Item1);
                }

                this.savedPixels.Clear();

                foreach (Sys.Graphics.Point p in points)
                {
                    this.savedPixels.Add(new Tuple<Sys.Graphics.Point, Color>(p, this.canvas.GetPointColor(p.X, p.Y)));
                    this.canvas.DrawPoint(this.pen, p);
                }


            }

            if (MouseManager.MouseState == MouseState.Left && this.prevMouseState != MouseState.Left)
            {
                close = this.cancel.tryCancelClick((Int32)MouseManager.X, (Int32)MouseManager.Y);

                if (close)
                {
                    // 1. Disable graphics canvas
                    this.canvas.Disable();

                    // 3. Clear the screen
                    Console.Clear();

                    // 4. Drain any pending keyboard input
                    while (Cosmos.System.KeyboardManager.TryReadKey(out var _)) { }

                    // 5. Reset GUI reference in Kernel
                    Kernel.piano = null;

                    // 6. Show CLI prompt again
                    Console.WriteLine("Exited GUI.");
                    Console.Write("> ");

                    return;
                }

                this.tryKeyClick((Int32)MouseManager.X, (Int32)MouseManager.Y);
            }
            this.canvas.Display();
        }

        public void tryKeyClick(Int32 mouseX, Int32 mouseY)
        {
            Rectangle p = new Rectangle(mouseX, mouseY, 1, 1);

            int startX = 212;
            int y = 100;
            int keyW = 75;
            int keyH = 375;

                // A3
                if (p.IntersectsWith(new Rectangle(startX - keyW * 2, y, keyW, keyH)))
                    key.LaLow();

                // B3
                if (p.IntersectsWith(new Rectangle(startX - keyW, y, keyW, keyH)))
                    key.TiLow();

                // C4
                if (p.IntersectsWith(new Rectangle(startX, y, keyW, keyH)))
                    key.Do();

                // D4
                if (p.IntersectsWith(new Rectangle(startX + keyW, y, keyW, keyH)))
                    key.Re();

                // E4
                if (p.IntersectsWith(new Rectangle(startX + keyW * 2, y, keyW, keyH)))
                    key.Mi();

                // F4
                if (p.IntersectsWith(new Rectangle(startX + keyW * 3, y, keyW, keyH)))
                    key.Fa();

                // G4
                if (p.IntersectsWith(new Rectangle(startX + keyW * 4, y, keyW, keyH)))
                    key.So();

                // A4
                if (p.IntersectsWith(new Rectangle(startX + keyW * 5, y, keyW, keyH)))
                    key.La();

                // B4
                if (p.IntersectsWith(new Rectangle(startX + keyW * 6, y, keyW, keyH)))
                    key.Ti();

                // C5
                if (p.IntersectsWith(new Rectangle(startX + keyW * 7, y, keyW, keyH)))
                    key.DoHigh();
        }


    }
}
