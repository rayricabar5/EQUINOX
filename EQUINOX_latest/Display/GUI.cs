using Cosmos.HAL;
using Cosmos.System;
using Cosmos.System.Graphics;
using EQUINOX.Features;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Console = System.Console;
using Sys = Cosmos.System;

namespace EQUINOX.Display
{
    public class GUI
    {
        private Canvas canvas;

        private MouseState prevMouseState;
        private List<Tuple<Sys.Graphics.Point, Color>> savedPixels;
        private UInt32 px, py;

        private Cancel cancel;
        private TabBar tabBar;
        private Pref pref;
        private ColorLib colorlib;
        private Redraw redraw;
        private OS_Details os_details;
        private SavePref pref_saved;

        private static Color bg;

        bool close = false;
        bool design = false;
        int active_tab = 0;

        private Pen pen;
        private Pen pref1 = new Pen(Color.Yellow);
        private Pen pref2 = new Pen(Color.Red);
        private Pen background;
        private Pen black;

        public GUI()
        {
            pref_saved.PrefInit();
            string theme = pref_saved.ReadPref();

            update();

            background = new Pen(bg);

            this.canvas = FullScreenCanvas.GetFullScreenCanvas();
            this.canvas.Clear(bg);

            this.pen = new Pen(Color.White);
            this.black = new Pen(Color.Black);
            this.prevMouseState = MouseState.None;

            this.px = 5;
            this.py = 5;
            this.savedPixels = new List<Tuple<Sys.Graphics.Point, Color>>();

            this.cancel = new Cancel(this.canvas);
            this.tabBar = new TabBar(this.canvas);

            MouseManager.ScreenHeight = (UInt32)this.canvas.Mode.Rows;
            MouseManager.ScreenWidth = (UInt32)this.canvas.Mode.Columns;

            //I
            this.canvas.DrawFilledRectangle(pref1, 20, 20, 110, 25);
            this.canvas.DrawFilledRectangle(pref1, 60, 45, 25, 80);
            this.canvas.DrawFilledRectangle(pref1, 20, 125, 110, 25);

            //N
            canvas.DrawFilledRectangle(pref1, 160, 20, 25, 130);
            canvas.DrawFilledRectangle(pref1, 245, 20, 25, 130);
            canvas.DrawFilledRectangle(pref1, 185, 46, 21, 26);
            canvas.DrawFilledRectangle(pref1, 206, 72, 21, 26);
            canvas.DrawFilledRectangle(pref1, 227, 98, 21, 26);
            canvas.DrawFilledRectangle(pref1, 248, 124, 22, 26);

            //D
            canvas.DrawFilledRectangle(pref1, 300, 20, 25, 130);
            canvas.DrawFilledRectangle(pref1, 385, 40, 25, 90);
            canvas.DrawFilledRectangle(pref1, 325, 20, 40, 13);
            canvas.DrawFilledRectangle(pref1, 355, 33, 40, 13);
            canvas.DrawFilledRectangle(pref1, 325, 137, 40, 13);
            canvas.DrawFilledRectangle(pref1, 355, 124, 40, 13);

            //I
            this.canvas.DrawFilledRectangle(pref1, 440, 20, 110, 25);
            this.canvas.DrawFilledRectangle(pref1, 480, 45, 25, 80);
            this.canvas.DrawFilledRectangle(pref1, 440, 125, 110, 25);

            //E
            this.canvas.DrawFilledRectangle(pref1, 580, 20, 25, 130);
            this.canvas.DrawFilledRectangle(pref1, 605, 20, 85, 26); ;
            this.canvas.DrawFilledRectangle(pref1, 605, 72, 85, 26);
            this.canvas.DrawFilledRectangle(pref1, 605, 124, 85, 26);

            //O
            this.canvas.DrawFilledRectangle(pref2, 505, 140, 25, 130);
            this.canvas.DrawFilledRectangle(pref2, 590, 140, 25, 130);
            this.canvas.DrawFilledRectangle(pref2, 530, 140, 60, 25);
            this.canvas.DrawFilledRectangle(pref2, 530, 245, 60, 25);

            //S
            this.canvas.DrawFilledRectangle(pref2, 670, 140, 85, 26);
            this.canvas.DrawFilledRectangle(pref2, 645, 140, 25, 78);
            this.canvas.DrawFilledRectangle(pref2, 670, 192, 60, 26);
            this.canvas.DrawFilledRectangle(pref2, 730, 192, 25, 78);
            this.canvas.DrawFilledRectangle(pref2, 645, 244, 85, 26);

            //Components Box
            this.canvas.DrawRectangle(black, 20, 300, 980, 320);

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
                    Kernel.gui = null;

                    // 6. Show CLI prompt again
                    Console.WriteLine("Exited GUI.");
                    Console.Write("> ");

                    return;
                }

                active_tab = this.tabBar.tryTabClick((Int32)MouseManager.X, (Int32)MouseManager.Y);


                if (active_tab == 1)
                {
                    redraw = new Redraw(bg, pref1.Color, pref2.Color);
                    redraw.Draw(canvas);
                    this.cancel = new Cancel(this.canvas);
                    this.tabBar = new TabBar(this.canvas);
                    this.pref = new Pref(this.canvas);
                    os_details = null;
                }

                else if (active_tab == 2)
                {
                    redraw = new Redraw(bg, pref1.Color, pref2.Color);
                    redraw.Draw(canvas);
                    this.cancel = new Cancel(this.canvas);
                    this.tabBar = new TabBar(this.canvas);
                    os_details = new OS_Details(this.canvas);
                    this.pref = null;
                }

                if (this.pref != null)
                {
                    design = this.pref.tryPrefClick((Int32)MouseManager.X, (Int32)MouseManager.Y);

                    if (design)
                    {
                        update();
                        // Redraw entire GUI with new preferences
                        redraw = new Redraw(bg, pref1.Color, pref2.Color);
                        redraw.Draw(canvas);

                        this.pref = null;
                        this.cancel = new Cancel(this.canvas);
                        this.tabBar = new TabBar(this.canvas);
                    }
                }

            }

            this.prevMouseState = MouseManager.MouseState;
            this.canvas.Display();
        }

        private void update()
        {
            string theme = pref_saved.ReadPref();

            if (theme == "Deck of Cards")
            {
                bg = Color.Red;
                pref1 = new Pen(Color.White);
                pref2 = new Pen(Color.Black);
            }
            else if (theme == "Skyline")
            {
                bg = Color.Blue;
                pref1 = new Pen(Color.LightBlue);
                pref2 = new Pen(Color.White);
            }
            else if (theme == "Veggie Salad")// Dark Green as default
            {
                bg = Color.DarkSlateGray;
                pref1 = new Pen(Color.Yellow);
                pref2 = new Pen(Color.Red);
            }
            else // Dark Green as default
            {
                bg = Color.DarkSlateGray;
                pref1 = new Pen(Color.Yellow);
                pref2 = new Pen(Color.Red);
            }
        }
    }   
}
