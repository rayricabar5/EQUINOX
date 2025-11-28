using Cosmos.HAL;
using Cosmos.System;
using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;
using EQUINOX.Audio;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Console = System.Console;
using Sys = Cosmos.System;

namespace EQUINOX.Display
{
    public class Piano
    {
        private Canvas canvas;
        private Cancel cancel;
        private Music key;

        // THEME COLORS
        private Color colBackground = Color.FromArgb(30, 30, 30);
        private Color colKeyWhite = Color.White;
        private Color colKeyShadow = Color.Silver;
        private Color colKeyBorder = Color.Black;
        private Color colText = Color.Black;
        private Color colAccent = Color.Cyan;

        private MouseState prevMouseState;
        private List<Tuple<Sys.Graphics.Point, Color>> savedPixels;
        private UInt32 px, py;
        private Pen penCursor;
        private bool close = false;

        // Key Configuration
        private int startX;
        private int startY;
        private int keyW = 75;
        private int keyH = 350;

        // Key Labels
        private string[] keyLabels = { "A3", "B3", "C4", "D4", "E4", "F4", "G4", "A4", "B4", "C5" };

        public Piano()
        {
            this.canvas = FullScreenCanvas.GetFullScreenCanvas();
            this.cancel = new Cancel(canvas);
            this.key = new Music();
            this.penCursor = new Pen(Color.Cyan);
            this.savedPixels = new List<Tuple<Sys.Graphics.Point, Color>>();

            MouseManager.ScreenHeight = (UInt32)this.canvas.Mode.Rows;
            MouseManager.ScreenWidth = (UInt32)this.canvas.Mode.Columns;

            // --- CALCULATE CENTERING ---
            int totalWidth = keyLabels.Length * keyW;
            this.startX = ((int)this.canvas.Mode.Columns - totalWidth) / 2;
            this.startY = ((int)this.canvas.Mode.Rows - keyH) / 2;

            // Initial Draw
            DrawInterface();
        }

        private void DrawInterface()
        {
            this.canvas.Clear(colBackground);

            // Header Line
            this.canvas.DrawFilledRectangle(new Pen(colAccent), 0, 40, (int)this.canvas.Mode.Columns, 5);
            this.canvas.DrawString("IndieOS Synth", PCScreenFont.Default, new Pen(Color.White), 25, 15);

            // Draw Keys
            for (int i = 0; i < keyLabels.Length; i++)
            {
                DrawKey(i, keyLabels[i]);
            }

            // Instructions
            string instructions = "[ Mouse Click ] or [ Keys A-L + P ] to Play  |  [ ESC ] to Exit";
            int instrX = ((int)this.canvas.Mode.Columns - (instructions.Length * 8)) / 2;
            this.canvas.DrawString(instructions, PCScreenFont.Default, new Pen(Color.Gray), instrX, (int)this.canvas.Mode.Rows - 30);
        }

        private void DrawKey(int index, string label)
        {
            int x = startX + (index * keyW);

            this.canvas.DrawFilledRectangle(new Pen(colKeyWhite), x, startY, keyW, keyH);
            this.canvas.DrawFilledRectangle(new Pen(colKeyShadow), x, startY + keyH - 30, keyW, 30);
            this.canvas.DrawRectangle(new Pen(colKeyBorder), x, startY, keyW, keyH);

            int textX = x + (keyW / 2) - ((label.Length * 8) / 2);
            int textY = startY + keyH - 20;
            this.canvas.DrawString(label, PCScreenFont.Default, new Pen(colText), textX, textY);
        }

        public void HandleGUIinputs()
        {
            bool actionTaken = false;

            // --- KEYBOARD INPUT ---
            KeyEvent k;
            if (KeyboardManager.TryReadKey(out k))
            {
                if (k.Key == ConsoleKeyEx.Escape)
                {
                    ExitPiano();
                    return;
                }

                int noteIndex = -1;
                switch (k.Key)
                {
                    case ConsoleKeyEx.A: noteIndex = 0; break;
                    case ConsoleKeyEx.S: noteIndex = 1; break;
                    case ConsoleKeyEx.D: noteIndex = 2; break;
                    case ConsoleKeyEx.F: noteIndex = 3; break;
                    case ConsoleKeyEx.G: noteIndex = 4; break;
                    case ConsoleKeyEx.H: noteIndex = 5; break;
                    case ConsoleKeyEx.J: noteIndex = 6; break;
                    case ConsoleKeyEx.K: noteIndex = 7; break;
                    case ConsoleKeyEx.L: noteIndex = 8; break;
                    case ConsoleKeyEx.P: noteIndex = 9; break;
                }

                if (noteIndex != -1)
                {
                    TriggerNote(noteIndex);
                    actionTaken = true;
                }
            }

            // --- MOUSE CURSOR ---
            if (this.px != MouseManager.X && this.py != MouseManager.Y)
            {
                if (MouseManager.X > 2 && MouseManager.Y > 2 && MouseManager.X < canvas.Mode.Columns - 2 && MouseManager.Y < canvas.Mode.Rows - 2)
                {
                    this.px = MouseManager.X;
                    this.py = MouseManager.Y;

                    foreach (Tuple<Sys.Graphics.Point, Color> pixelData in this.savedPixels)
                        this.canvas.DrawPoint(new Pen(pixelData.Item2), pixelData.Item1);
                    this.savedPixels.Clear();

                    Sys.Graphics.Point[] points = new Sys.Graphics.Point[]
                    {
                        new Sys.Graphics.Point((Int32)MouseManager.X, (Int32)MouseManager.Y),
                        new Sys.Graphics.Point((Int32)MouseManager.X - 1, (Int32)MouseManager.Y),
                        new Sys.Graphics.Point((Int32)MouseManager.X + 1, (Int32)MouseManager.Y),
                        new Sys.Graphics.Point((Int32)MouseManager.X, (Int32)MouseManager.Y - 1),
                        new Sys.Graphics.Point((Int32)MouseManager.X, (Int32)MouseManager.Y + 1)
                    };

                    foreach (Sys.Graphics.Point p in points)
                    {
                        if (p.X >= 0 && p.X < canvas.Mode.Columns && p.Y >= 0 && p.Y < canvas.Mode.Rows)
                        {
                            this.savedPixels.Add(new Tuple<Sys.Graphics.Point, Color>(p, this.canvas.GetPointColor(p.X, p.Y)));
                            this.canvas.DrawPoint(this.penCursor, p);
                        }
                    }
                    actionTaken = true;
                }
            }

            // --- MOUSE CLICK ---
            if (MouseManager.MouseState == MouseState.Left && this.prevMouseState != MouseState.Left)
            {
                close = this.cancel.tryCancelClick((Int32)MouseManager.X, (Int32)MouseManager.Y);
                if (close)
                {
                    ExitPiano();
                    return;
                }

                this.tryMouseClick((Int32)MouseManager.X, (Int32)MouseManager.Y);
                actionTaken = true;
            }

            this.prevMouseState = MouseManager.MouseState;

            if (actionTaken) this.canvas.Display();
        }

        private void TriggerNote(int index)
        {
            HighlightKey(index);
            this.canvas.Display();
            PlayNoteByIndex(index);
            DrawKey(index, keyLabels[index]);
            this.canvas.Display();
        }

        private void ExitPiano()
        {
            this.canvas.Disable();
            Console.Clear();
            while (Cosmos.System.KeyboardManager.TryReadKey(out var _)) { }

            // Reset Global Reference
            Kernel.piano = null;

            // DRAW THE LOGO ON EXIT
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(@"
  _____           _  _        ____   _____ 
 |_   _|         | |(_)      / __ \ / ____|
   | |  _ __   __| | _  ___ | |  | | (___  
   | | | '_ \ / _` || |/ _ \| |  | |\___ \ 
  _| |_| | | | (_| || |  __/| |__| |____) |
 |_____|_| |_|\__,_||_|\___| \____/|_____/ 
            ");
            Console.ResetColor();
            Console.WriteLine("\n       Welcome to IndieOS v1.0");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("       Type 'help' for commands.");
            Console.ResetColor();
            Console.WriteLine();
            Console.Write("IndieOS> ");
        }

        public void tryMouseClick(Int32 mouseX, Int32 mouseY)
        {
            if (mouseY >= startY && mouseY <= startY + keyH)
            {
                if (mouseX >= startX)
                {
                    int relativeX = mouseX - startX;
                    int keyIndex = relativeX / keyW;

                    if (keyIndex >= 0 && keyIndex < keyLabels.Length)
                    {
                        TriggerNote(keyIndex);
                    }
                }
            }
        }

        private void HighlightKey(int index)
        {
            int x = startX + (index * keyW);
            this.canvas.DrawFilledRectangle(new Pen(Color.Cyan), x + 5, startY + 5, keyW - 10, keyH - 40);
        }

        private void PlayNoteByIndex(int index)
        {
            switch (index)
            {
                case 0: key.LaLow(); break;
                case 1: key.TiLow(); break;
                case 2: key.Do(); break;
                case 3: key.Re(); break;
                case 4: key.Mi(); break;
                case 5: key.Fa(); break;
                case 6: key.So(); break;
                case 7: key.La(); break;
                case 8: key.Ti(); break;
                case 9: key.DoHigh(); break;
            }
        }
    }
}