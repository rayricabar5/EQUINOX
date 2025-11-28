using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using Cosmos.System;
using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;

namespace EQUINOX.Features
{
    public class Typeracing
    {
        private Canvas canvas;
        private int screenWidth;
        private int screenHeight;
        private Pen white = new Pen(Color.White);
        private Pen gray = new Pen(Color.Gray);
        private Pen green = new Pen(Color.FromArgb(120, 255, 120));
        private Pen red = new Pen(Color.FromArgb(255, 120, 120));
        private Pen darkBg = new Pen(Color.FromArgb(12, 12, 12));
        private string[] passages = new string[] {
            "Si Sir Michael Ramos ang pinaka magaling na Operating System professor!",
            "Palagi ako pumapasok sa Operating System Subject kasi gusto kong matuto.",
            "I genuinely feel fulfilled and enjoy myself every time I attend my Operating Systems class.",
            "Ako ay estudyante ni Sir Michael Ramos at makakakuha ako ng pinaka mataas na marka.",
            "IndieOS is one of the most outstanding, interactive, and gorgeous projects among all operating system projects.",
        };

        // Performance / UI tuning
        private const int TargetFps = 25; // cap redraws
        private readonly int frameMs = 1000 / TargetFps;

        // Precomputed target layout (static per sentence)
        private List<string> targetLines = new List<string>();
        private int targetPanelX, targetPanelY, targetPanelW, targetPanelH;
        private int typedAreaX, typedAreaY, typedAreaW, typedAreaH;

        public Typeracing(Canvas canvas)
        {
            this.canvas = canvas;
            this.screenWidth = (int)canvas.Mode.Columns;
            this.screenHeight = (int)canvas.Mode.Rows;
        }

        public void Run()
        {
            Random rnd = new Random();
            string target = passages[rnd.Next(passages.Length)];
            string typed = "";
            bool running = true;
            bool started = false;
            DateTime startTime = DateTime.MinValue;
            DateTime endTime = DateTime.MinValue;
            int mistakes = 0;
            bool dirty = true;
            DateTime lastDraw = DateTime.MinValue;
            bool caretOn = true;
            DateTime lastCaretToggle = DateTime.Now;

            // Initial screen
            DrawIntro(target, typed, "Press any key to start. ESC to exit.");

            while (running)
            {
                KeyEvent k;
                bool hadKey = false;

                if (KeyboardManager.TryReadKey(out k))
                {
                    hadKey = true;

                    if (k.Key == ConsoleKeyEx.Escape)
                    {
                        running = false;
                        break;
                    }

                    if (!started)
                    {
                        started = true;
                        startTime = DateTime.Now;
                    }

                    if (k.Key == ConsoleKeyEx.Backspace)
                    {
                        if (typed.Length > 0)
                        {
                            typed = typed.Substring(0, typed.Length - 1);
                            dirty = true;
                        }
                    }
                    else if (k.Key == ConsoleKeyEx.Enter)
                    {
                        // restart with a new passage
                        target = passages[rnd.Next(passages.Length)];
                        typed = "";
                        mistakes = 0;
                        started = false;
                        startTime = DateTime.MinValue;
                        DrawIntro(target, typed, "Press any key to start. ESC to exit.");
                        dirty = false;
                        lastDraw = DateTime.Now;
                        continue;
                    }
                    else
                    {
                        char ch = k.KeyChar;
                        if (ch != '\0')
                        {
                            typed += ch;
                            int idx = typed.Length - 1;
                            if (idx < target.Length)
                            {
                                if (typed[idx] != target[idx]) mistakes++;
                            }
                            else
                            {
                                mistakes++; // extra characters are mistakes
                            }
                            dirty = true;
                        }
                    }
                }

                // Finish check
                if (typed == target && started)
                {
                    endTime = DateTime.Now;
                    double seconds = (endTime - startTime).TotalSeconds;
                    double minutes = seconds / 60.0;
                    int chars = target.Length;
                    double wpm = (chars / 5.0) / (minutes > 0 ? minutes : 1.0);
                    double accuracy = Math.Max(0, ((double)(chars - mistakes) / chars) * 100.0);
                    DrawResult(target, typed, wpm, accuracy, seconds);

                    // wait for enter or esc to return/retry
                    bool waiting = true;
                    while (waiting)
                    {
                        KeyEvent kk;
                        if (KeyboardManager.TryReadKey(out kk))
                        {
                            if (kk.Key == ConsoleKeyEx.Escape)
                            {
                                running = false;
                                waiting = false;
                                break;
                            }
                            else if (kk.Key == ConsoleKeyEx.Enter)
                            {
                                target = passages[rnd.Next(passages.Length)];
                                typed = "";
                                mistakes = 0;
                                started = false;
                                startTime = DateTime.MinValue;
                                DrawIntro(target, typed, "Press any key to start. ESC to exit.");
                                waiting = false;
                                dirty = false;
                                lastDraw = DateTime.Now;
                                break;
                            }
                        }
                        Thread.Sleep(20);
                    }
                }

                // caret blink toggle
                if ((DateTime.Now - lastCaretToggle).TotalMilliseconds >= 500)
                {
                    caretOn = !caretOn;
                    lastCaretToggle = DateTime.Now;
                    dirty = true;
                }

                // redraw only at limited FPS or when dirty (input)
                if (dirty || (DateTime.Now - lastDraw).TotalMilliseconds >= frameMs)
                {
                    double elapsed = (startTime == DateTime.MinValue) ? 0 : (DateTime.Now - startTime).TotalSeconds;
                    DrawDynamic(target, typed, mistakes, elapsed, caretOn);
                    lastDraw = DateTime.Now;
                    dirty = false;
                }

                // small sleep to avoid busy-wait and reduce CPU usage
                Thread.Sleep(6);
            }
        }

        private void DrawIntro(string target, string typed, string hint)
        {
            // backward-compatible alias to PrepareAndDrawStatic
            PrepareAndDrawStatic(target, hint);
        }

        // Draw static UI once per sentence: header, target panel and hint
        private void PrepareAndDrawStatic(string target, string hint)
        {
            canvas.Clear(Color.Black);
            int centerX = screenWidth / 2;
            canvas.DrawString("TYperace", PCScreenFont.Default, white, centerX - 72, 12);
            canvas.DrawString("Esc: Exit  |  Enter: New Sentence", PCScreenFont.Default, gray, 20, 36);

            // target panel
            targetPanelX = 16;
            targetPanelY = 72;
            targetPanelW = screenWidth - 32;
            targetPanelH = 80;
            canvas.DrawFilledRectangle(darkBg, targetPanelX, targetPanelY - 8, targetPanelW, targetPanelH + 16);

            // Compute wrapped lines once
            targetLines.Clear();
            int maxTextWidth = targetPanelW - 24;
            int charW = 8;
            int maxCharsPerLine = Math.Max(10, maxTextWidth / charW);
            string[] words = target.Split(' ');
            string line = "";
            foreach (var w in words)
            {
                if (line.Length + w.Length + 1 <= maxCharsPerLine)
                {
                    line = (line.Length == 0) ? w : (line + " " + w);
                }
                else
                {
                    targetLines.Add(line);
                    line = w;
                }
            }
            if (line.Length > 0) targetLines.Add(line);

            int ty = targetPanelY;
            foreach (var l in targetLines)
            {
                canvas.DrawString(l, PCScreenFont.Default, gray, targetPanelX + 12, ty);
                ty += 16;
            }

            // typed area
            typedAreaX = targetPanelX + 12;
            typedAreaY = targetPanelY + 40;
            typedAreaW = targetPanelW - 24;
            typedAreaH = 40;

            // hint and accent
            canvas.DrawString(hint, PCScreenFont.Default, gray, 20, screenHeight - 30);
            canvas.DrawFilledRectangle(new Pen(Color.FromArgb(40, 80, 160)), 0, 0, screenWidth, 6);

            canvas.Display();
        }

        // Draw only dynamic parts: typed text, caret, progress and stats
        private void DrawDynamic(string target, string typed, int mistakes, double elapsedSeconds, bool caretOn)
        {
            // clear typed area
            canvas.DrawFilledRectangle(darkBg, typedAreaX, typedAreaY - 6, typedAreaW, typedAreaH + 8);

            int x = typedAreaX;
            int y = typedAreaY;
            int charWidth = 8;
            int maxX = typedAreaX + typedAreaW;
            for (int i = 0; i < typed.Length; i++)
            {
                char tc = typed[i];
                char expected = i < target.Length ? target[i] : '\0';
                Pen p = white;
                if (expected != '\0') p = (tc == expected) ? green : red;
                canvas.DrawString(tc.ToString(), PCScreenFont.Default, p, x, y);
                x += charWidth;
                if (x + charWidth > maxX)
                {
                    x = typedAreaX;
                    y += 16;
                }
            }

            // caret
            if (caretOn)
            {
                canvas.DrawString("_", PCScreenFont.Default, white, x, y);
            }

            // progress bar
            int progressBarY = screenHeight - 84;
            int progressBarX = 20;
            int progressBarW = screenWidth - 40;
            canvas.DrawFilledRectangle(darkBg, progressBarX, progressBarY - 2, progressBarW, 18);
            canvas.DrawRectangle(gray, progressBarX, progressBarY, progressBarW, 14);
            double progress = Math.Min(1.0, typed.Length / (double)Math.Max(1, target.Length));
            int pW = (int)(progressBarW * progress);
            canvas.DrawFilledRectangle(new Pen(Color.FromArgb(30, 160, 30)), progressBarX + 1, progressBarY + 1, Math.Max(0, pW - 2), 12);

            // stats
            double minutes = elapsedSeconds / 60.0;
            double wpm = minutes > 0 ? ((typed.Length / 5.0) / minutes) : 0.0;
            int correctChars = Math.Max(0, typed.Length - mistakes);
            double accuracy = typed.Length > 0 ? (correctChars / (double)typed.Length) * 100.0 : 100.0;
            string stats = $"Time: {elapsedSeconds:0.0}s   WPM: {wpm:0.0}   Accuracy: {accuracy:0.0}%   Mistakes: {mistakes}";
            canvas.DrawFilledRectangle(darkBg, 16, screenHeight - 68, screenWidth - 32, 20);
            canvas.DrawString(stats, PCScreenFont.Default, white, 20, screenHeight - 60);

            canvas.Display();
        }

        private void DrawTypingScreen(string target, string typed, int mistakes, double elapsedSeconds, bool caretOn)
        {
            canvas.Clear(Color.Black);

            // Header
            int centerX = screenWidth / 2;
            canvas.DrawString("TYperace", PCScreenFont.Default, white, centerX - 72, 12);
            canvas.DrawString("Esc: Exit  |  Enter: New Sentence", PCScreenFont.Default, gray, 20, 36);

            // Target box
            int boxX = 16;
            int boxY = 72;
            int boxW = screenWidth - 32;
            int boxH = 80;
            canvas.DrawFilledRectangle(darkBg, boxX, boxY - 8, boxW, boxH + 16);
            DrawWrappedText(target, boxX + 12, boxY, boxW - 24, gray);

            // Typed text - draw with per-char coloring
            int startY = boxY + 40;
            int x = boxX + 12;
            int y = startY;
            int charWidth = 8;
            for (int i = 0; i < typed.Length; i++)
            {
                char tc = typed[i];
                char expected = i < target.Length ? target[i] : '\0';
                Pen p = white;
                if (expected != '\0')
                {
                    if (tc == expected) p = green;
                    else p = red;
                }
                canvas.DrawString(tc.ToString(), PCScreenFont.Default, p, x, y);
                x += charWidth;
                if (x > screenWidth - 20)
                {
                    x = boxX + 12;
                    y += 16;
                }
            }

            // caret
            if (caretOn)
            {
                int caretX = x;
                int caretY = y + 12;
                canvas.DrawString("_", PCScreenFont.Default, white, caretX, y);
            }

            // progress bar
            int progressBarY = screenHeight - 84;
            int progressBarX = 20;
            int progressBarW = screenWidth - 40;
            canvas.DrawRectangle(gray, progressBarX, progressBarY, progressBarW, 14);
            double progress = Math.Min(1.0, typed.Length / (double)Math.Max(1, target.Length));
            int pW = (int)(progressBarW * progress);
            canvas.DrawFilledRectangle(new Pen(Color.FromArgb(30, 160, 30)), progressBarX + 1, progressBarY + 1, Math.Max(0, pW - 2), 12);

            // Stats live
            double minutes = elapsedSeconds / 60.0;
            double wpm = minutes > 0 ? ((typed.Length / 5.0) / minutes) : 0.0;
            int correctChars = Math.Max(0, typed.Length - mistakes);
            double accuracy = typed.Length > 0 ? (correctChars / (double)typed.Length) * 100.0 : 100.0;

            string stats = $"Time: {elapsedSeconds:0.0}s   WPM: {wpm:0.0}   Accuracy: {accuracy:0.0}%   Mistakes: {mistakes}";
            canvas.DrawString(stats, PCScreenFont.Default, white, 20, screenHeight - 60);

            canvas.Display();
        }

        private void DrawResult(string target, string typed, double wpm, double accuracy, double seconds)
        {
            canvas.Clear(Color.Black);
            canvas.DrawString("TYperace - Finished", PCScreenFont.Default, white, 20, 10);
            DrawWrappedText(target, 20, 50, screenWidth - 40, gray);
            canvas.DrawString($"Time: {seconds:0.0}s", PCScreenFont.Default, white, 20, 140);
            canvas.DrawString($"WPM: {wpm:0.0}", PCScreenFont.Default, white, 20, 160);
            canvas.DrawString($"Accuracy: {accuracy:0.0}%", PCScreenFont.Default, white, 20, 180);
            canvas.DrawString("Press Enter to retry, Esc to return.", PCScreenFont.Default, gray, 20, screenHeight - 40);
            canvas.Display();
        }

        private void DrawWrappedText(string text, int xStart, int yStart, int maxWidth, Pen pen)
        {
            // Simple wrap by words (minimal allocations)
            string[] words = text.Split(' ');
            int x = xStart;
            int y = yStart;
            int charW = 8;
            foreach (var w in words)
            {
                string tw = w + " ";
                int twLen = tw.Length * charW;
                if (x + twLen > xStart + maxWidth)
                {
                    x = xStart;
                    y += 16;
                }
                canvas.DrawString(tw, PCScreenFont.Default, pen, x, y);
                x += twLen;
            }
        }
    }
}
