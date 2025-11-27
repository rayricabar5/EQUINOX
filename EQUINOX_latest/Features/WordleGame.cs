using System;
using System.Collections.Generic;
using System.Drawing;
using Cosmos.System;
using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;

namespace EQUINOX.Features
{
    public class WordleGame
    {
        private Canvas canvas;
        private int screenWidth;
        private int screenHeight;

        // --- Game Constants ---
        private const int MAX_ATTEMPTS = 6;
        private const int WORD_LENGTH = 5;

        // --- Layout Variables ---
        private int boxSize = 50;
        private int gap = 8;
        private int keyWidth = 30;
        private int keyHeight = 45;
        private int keyGap = 6;
        private int startY_Grid = 80;
        private int startY_Keyboard = 0;

        // --- Game State ---
        private string targetWord;
        private List<string> guesses;
        private string currentGuess;
        private bool gameOver;
        private string message;
        private Dictionary<char, int> keyStates; // 0: Unused, 1: Absent, 2: Present, 3: Correct

        // --- Theme Colors (Dark Mode Style) ---
        // Using Color.FromArgb for specific "Wordle" aesthetics
        private readonly Color Col_Background = Color.FromArgb(18, 18, 19);    // Dark Hex #121213
        private readonly Color Col_Correct = Color.FromArgb(83, 141, 78);   // Green Hex #538D4E
        private readonly Color Col_Present = Color.FromArgb(181, 159, 59);  // Yellow Hex #B59F3B
        private readonly Color Col_Absent = Color.FromArgb(58, 58, 60);    // Dark Gray Hex #3A3A3C
        private readonly Color Col_Border = Color.FromArgb(58, 58, 60);    // Border Gray
        private readonly Color Col_Text = Color.White;
        private readonly Color Col_KeyDefault = Color.FromArgb(129, 131, 132); // Light Gray for keys

        // Pens
        private Pen penCorrect;
        private Pen penPresent;
        private Pen penAbsent;
        private Pen penBorder;
        private Pen penText;
        private Pen penKeyDefault;
        private Pen penBackground;

        private string[] wordList = new string[] {
            "APPLE", "BERRY", "CRANE", "DRIVE", "EAGLE",
            "FLAME", "GRAPE", "HOUSE", "IMAGE", "JUMPS",
            "KITES", "LEMON", "MOUSE", "NIGHT", "OCEAN",
            "POWER", "QUEST", "RIVER", "SPACE", "TIGER",
            "WORLD", "PIZZA", "UNITY", "CODES", "SMART"
        };

        private string[] keyRows = new string[] {
            "QWERTYUIOP",
            "ASDFGHJKL",
            "ZXCVBNM"
        };

        public WordleGame(Canvas canvas)
        {
            this.canvas = canvas;
            this.screenWidth = (int)canvas.Mode.Columns;
            this.screenHeight = (int)canvas.Mode.Rows;
            this.keyStates = new Dictionary<char, int>();

            // Initialize Pens once
            penCorrect = new Pen(Col_Correct);
            penPresent = new Pen(Col_Present);
            penAbsent = new Pen(Col_Absent);
            penBorder = new Pen(Col_Border);
            penText = new Pen(Col_Text);
            penKeyDefault = new Pen(Col_KeyDefault);
            penBackground = new Pen(Col_Background);

            CalculateLayout();
        }

        private void CalculateLayout()
        {
            // Dynamic resizing based on screen resolution
            int totalAvailableHeight = screenHeight - 100; // Reserve header space
            int gridTotalHeight = (int)(totalAvailableHeight * 0.55);

            boxSize = (gridTotalHeight / MAX_ATTEMPTS) - gap;

            // Clamp box size for sanity
            if (boxSize > 70) boxSize = 70;
            if (boxSize < 30) boxSize = 30;

            int maxKeyRowWidth = (int)(screenWidth * 0.6); // Keyboard takes 60% width
            keyWidth = (maxKeyRowWidth / 10) - keyGap;

            if (keyWidth > 40) keyWidth = 40;
            keyHeight = (int)(keyWidth * 1.4); // Aspect ratio for keys

            startY_Grid = 70;
            int gridHeightPixels = (MAX_ATTEMPTS * (boxSize + gap));
            startY_Keyboard = startY_Grid + gridHeightPixels + 40; // Space between grid and keyboard
        }

        public void Run()
        {
            ResetGame();
            bool running = true;

            Draw();

            while (running)
            {
                KeyEvent k;
                if (KeyboardManager.TryReadKey(out k))
                {
                    bool needsRedraw = false;

                    if (k.Key == ConsoleKeyEx.Escape)
                    {
                        running = false;
                    }
                    else if (gameOver)
                    {
                        if (k.Key == ConsoleKeyEx.Enter)
                        {
                            ResetGame();
                            needsRedraw = true;
                        }
                    }
                    else
                    {
                        ProcessKey(k, ref needsRedraw);
                    }

                    if (needsRedraw)
                    {
                        Draw();
                    }
                }
            }
        }

        private void ResetGame()
        {
            Random rnd = new Random();
            targetWord = wordList[rnd.Next(wordList.Length)];
            guesses = new List<string>();
            currentGuess = "";
            gameOver = false;
            message = "Guess the 5-letter word";

            keyStates.Clear();
            string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            foreach (char c in alphabet)
            {
                keyStates[c] = 0;
            }
        }

        private void ProcessKey(KeyEvent k, ref bool changed)
        {
            if (k.Key == ConsoleKeyEx.Enter)
            {
                if (currentGuess.Length == WORD_LENGTH)
                {
                    guesses.Add(currentGuess);
                    UpdateKeyStates(currentGuess);
                    CheckWinCondition();
                    currentGuess = "";
                    changed = true;
                }
            }
            else if (k.Key == ConsoleKeyEx.Backspace)
            {
                if (currentGuess.Length > 0)
                {
                    currentGuess = currentGuess.Substring(0, currentGuess.Length - 1);
                    changed = true;
                }
            }
            else if (char.IsLetter(k.KeyChar) && currentGuess.Length < WORD_LENGTH)
            {
                currentGuess += k.KeyChar.ToString().ToUpper();
                changed = true;
            }
        }

        private void UpdateKeyStates(string guess)
        {
            for (int i = 0; i < WORD_LENGTH; i++)
            {
                char c = guess[i];
                int status = 1; // Absent by default

                if (targetWord[i] == c)
                {
                    status = 3; // Correct
                }
                else if (targetWord.Contains(c.ToString()))
                {
                    status = 2; // Present
                }

                // Only upgrade status (don't downgrade a Green key to Yellow)
                if (keyStates[c] < status)
                {
                    keyStates[c] = status;
                }
            }
        }

        private void CheckWinCondition()
        {
            string lastGuess = guesses[guesses.Count - 1];
            if (lastGuess == targetWord)
            {
                gameOver = true;
                message = "SPLENDID! Enter to play again.";
            }
            else if (guesses.Count >= MAX_ATTEMPTS)
            {
                gameOver = true;
                message = "GAME OVER: " + targetWord;
            }
        }

        private void Draw()
        {
            // 1. Clear Background
            canvas.Clear(Col_Background);

            // 2. Draw Header
            int centerX = screenWidth / 2;
            int gridTotalWidth = (boxSize + gap) * WORD_LENGTH;
            int startX = (screenWidth - gridTotalWidth) / 2;

            DrawCenteredText("EQUINOX WORDLE", 20, Col_Text, 2); // Larger faux-bold
            DrawCenteredText(message, 50, Color.LightGray, 1);

            // 3. Draw Grid
            for (int row = 0; row < MAX_ATTEMPTS; row++)
            {
                for (int col = 0; col < WORD_LENGTH; col++)
                {
                    int x = startX + col * (boxSize + gap);
                    int y = startY_Grid + row * (boxSize + gap);

                    string letter = "";
                    Pen fillPen = null;       // If null, don't fill (transparent/black)
                    Pen outlinePen = penBorder;
                    Pen charPen = penText;

                    // Logic for colored boxes
                    if (row < guesses.Count)
                    {
                        // Completed rows
                        string g = guesses[row];
                        letter = g[col].ToString();

                        if (letter[0] == targetWord[col])
                        {
                            fillPen = penCorrect;
                            outlinePen = penCorrect;
                        }
                        else if (targetWord.Contains(letter))
                        {
                            fillPen = penPresent;
                            outlinePen = penPresent;
                        }
                        else
                        {
                            fillPen = penAbsent;
                            outlinePen = penAbsent;
                        }
                    }
                    else if (row == guesses.Count)
                    {
                        // Current typing row
                        if (col < currentGuess.Length)
                        {
                            letter = currentGuess[col].ToString();
                            outlinePen = new Pen(Color.LightGray); // Highlight active letters
                        }
                    }

                    // Render Box
                    if (fillPen != null)
                    {
                        canvas.DrawFilledRectangle(fillPen, x, y, boxSize, boxSize);
                    }
                    else
                    {
                        // Draw outline for empty/typing boxes
                        canvas.DrawRectangle(outlinePen, x, y, boxSize, boxSize);
                    }

                    // Render Letter
                    if (letter != "")
                    {
                        // Simple centering math for monospaced font
                        // Assumes approx 8x16 font size
                        int textX = x + (boxSize / 2) - 4;
                        int textY = y + (boxSize / 2) - 8;
                        canvas.DrawString(letter, PCScreenFont.Default, charPen, textX, textY);
                    }
                }
            }

            // 4. Draw Keyboard
            DrawKeyboard(startY_Keyboard);

            canvas.Display();
        }

        private void DrawKeyboard(int yStart)
        {
            int kStartX = 0;

            for (int r = 0; r < keyRows.Length; r++)
            {
                string rowKeys = keyRows[r];

                // Calculate center alignment for this specific row
                int rowWidth = (rowKeys.Length * keyWidth) + ((rowKeys.Length - 1) * keyGap);
                kStartX = (screenWidth - rowWidth) / 2;

                int currentX = kStartX;

                foreach (char c in rowKeys)
                {
                    int state = keyStates.ContainsKey(c) ? keyStates[c] : 0;
                    Pen kFill = penKeyDefault;
                    Pen kText = penText;

                    switch (state)
                    {
                        case 0: kFill = penKeyDefault; break; // Unused
                        case 1: kFill = penAbsent; break;     // Wrong
                        case 2: kFill = penPresent; break;    // Wrong Spot
                        case 3: kFill = penCorrect; break;    // Correct
                    }

                    // Draw Key
                    canvas.DrawFilledRectangle(kFill, currentX, yStart, keyWidth, keyHeight);

                    // Draw Key Letter
                    int kTextX = currentX + (keyWidth / 2) - 4;
                    int kTextY = yStart + (keyHeight / 2) - 8;
                    canvas.DrawString(c.ToString(), PCScreenFont.Default, kText, kTextX, kTextY);

                    currentX += keyWidth + keyGap;
                }
                yStart += keyHeight + keyGap;
            }
        }

        // Helper to center text easily
        private void DrawCenteredText(string text, int y, Color color, int scale = 1)
        {
            // Note: PCScreenFont is usually 8px wide per char
            int strWidth = text.Length * 8;
            int x = (screenWidth - strWidth) / 2;

            // Simple DrawString wrapper
            canvas.DrawString(text, PCScreenFont.Default, new Pen(color), x, y);
        }
    }
}