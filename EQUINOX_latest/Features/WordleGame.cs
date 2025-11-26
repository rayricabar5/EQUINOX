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
        private const int MAX_ATTEMPTS = 6;
        private const int WORD_LENGTH = 5;
        private int boxSize = 50; 
        private int gap = 10;
        private int keyWidth = 30;
        private int keyHeight = 40;
        private int keyGap = 5;
        private int startY_Grid = 80;
        private int startY_Keyboard = 0;
        private int fontSizeOffset = 0;
        private string targetWord;
        private List<string> guesses;
        private string currentGuess;
        private bool gameOver;
        private string message;
        private Dictionary<char, int> keyStates;
        private Pen whitePen = new Pen(Color.White);
        private Pen grayPen = new Pen(Color.Gray);
        private Pen darkGrayPen = new Pen(Color.FromArgb(50, 50, 50));
        private Pen blackPen = new Pen(Color.Black);
        private string[] wordList = new string[] {
            "APPLE", "BERRY", "CRANE", "DRIVE", "EAGLE",
            "FLAME", "GRAPE", "HOUSE", "IMAGE", "JUMPS",
            "KITES", "LEMON", "MOUSE", "NIGHT", "OCEAN",
            "POWER", "QUEST", "RIVER", "SPACE", "TIGER"
        };
        private string[] keyRows = new string[] {
            "ABCDEFGHIJKLM",
            "NOPQRSTUVWXYZ"
        };
        public WordleGame(Canvas canvas)
        {
            this.canvas = canvas;
            this.screenWidth = (int)canvas.Mode.Columns;
            this.screenHeight = (int)canvas.Mode.Rows;
            this.keyStates = new Dictionary<char, int>();
            CalculateLayout();
        }
        private void CalculateLayout()
        {
            int totalAvailableHeight = screenHeight - 80; 
            int gridTotalHeight = (int)(totalAvailableHeight * 0.60);
            boxSize = (gridTotalHeight / MAX_ATTEMPTS) - gap;
            
            if (boxSize > 60) boxSize = 60;
            if (boxSize < 20) boxSize = 20;

            int maxKeyRowWidth = screenWidth - 20; 
            keyWidth = (maxKeyRowWidth / 13) - keyGap;            
            
            if (keyWidth > 50) keyWidth = 50;
            keyHeight = (int)(boxSize * 0.8);
            startY_Grid = 60;
            int gridHeightPixels = (MAX_ATTEMPTS * (boxSize + gap));
            startY_Keyboard = startY_Grid + gridHeightPixels + 20;
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
            message = "Type 5 letters. ENTER to guess.";

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
                int status = 1;

                if (targetWord[i] == c)
                {
                    status = 3;
                }
                else if (targetWord.Contains(c.ToString()))
                {
                    status = 2;
                }

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
                message = "WINNER! Press Enter.";
            }
            else if (guesses.Count >= MAX_ATTEMPTS)
            {
                gameOver = true;
                message = "GAME OVER: " + targetWord;
            }
        }

        private void Draw()
        {
            canvas.Clear(Color.Black);
            int gridTotalWidth = (boxSize + gap) * WORD_LENGTH; 
            int startX = (screenWidth - gridTotalWidth) / 2;
            canvas.DrawString("EQUINOX WORDLE", PCScreenFont.Default, whitePen, startX, 20);
            canvas.DrawString(message, PCScreenFont.Default, grayPen, startX, 40);
            for (int row = 0; row < MAX_ATTEMPTS; row++)
            {
                for (int col = 0; col < WORD_LENGTH; col++)
                {
                    int x = startX + col * (boxSize + gap);
                    int y = startY_Grid + row * (boxSize + gap);

                    string letter = "";
                    bool isFilled = false;
                    Color fillColor = Color.Black;
                    Pen textPen = whitePen;
                    Pen outlinePen = grayPen;

                    if (row < guesses.Count)
                    {
                        string g = guesses[row];
                        letter = g[col].ToString();

                        if (letter[0] == targetWord[col])
                        {
                            isFilled = true;
                            fillColor = Color.White;
                            textPen = blackPen;
                        }
                        else if (targetWord.Contains(letter))
                        {
                            isFilled = true;
                            fillColor = Color.Gray;
                            textPen = whitePen;
                        }
                        else
                        {
                            isFilled = false;
                            outlinePen = darkGrayPen;
                            textPen = grayPen;
                        }
                    }
                    else if (row == guesses.Count && col < currentGuess.Length)
                    {
                        letter = currentGuess[col].ToString();
                        isFilled = false;
                        outlinePen = whitePen;
                        textPen = whitePen;
                    }
                    else
                    {
                        isFilled = false;
                        outlinePen = darkGrayPen;
                    }

                    if (isFilled)
                        canvas.DrawFilledRectangle(new Pen(fillColor), x, y, boxSize, boxSize);
                    else
                        canvas.DrawRectangle(outlinePen, x, y, boxSize, boxSize);
                    if (letter != "")
                    {
                        int textX = x + (boxSize / 2) - 4; 
                        int textY = y + (boxSize / 2) - 8;
                        canvas.DrawString(letter, PCScreenFont.Default, textPen, textX, textY);
                    }
                }
            }
            DrawKeyboard(startY_Keyboard);
            canvas.Display();
        }

        private void DrawKeyboard(int yStart)
        {
            int keysPerRow = 13; 
            int totalRowWidth = (keysPerRow * keyWidth) + ((keysPerRow - 1) * keyGap);
            int kStartX = (screenWidth - totalRowWidth) / 2;

            for (int r = 0; r < keyRows.Length; r++)
            {
                string rowKeys = keyRows[r];
                int currentX = kStartX;

                foreach (char c in rowKeys)
                {
                    int state = keyStates.ContainsKey(c) ? keyStates[c] : 0;

                    bool filled = false;
                    Color kFill = Color.Black;
                    Pen kText = whitePen;
                    Pen kOutline = grayPen;

                    switch (state)
                    {
                        case 0: 
                            kOutline = grayPen;
                            kText = whitePen;
                            break;
                        case 1: 
                            kOutline = darkGrayPen;
                            kText = darkGrayPen;
                            break;
                        case 2: 
                            filled = true;
                            kFill = Color.Gray;
                            kText = whitePen;
                            break;
                        case 3:
                            filled = true;
                            kFill = Color.White;
                            kText = blackPen;
                            break;
                    }

                    if (filled)
                        canvas.DrawFilledRectangle(new Pen(kFill), currentX, yStart, keyWidth, keyHeight);
                    else
                        canvas.DrawRectangle(kOutline, currentX, yStart, keyWidth, keyHeight);
                    int kTextX = currentX + (keyWidth / 2) - 4;
                    int kTextY = yStart + (keyHeight / 2) - 8;

                    canvas.DrawString(c.ToString(), PCScreenFont.Default, kText, kTextX, kTextY);

                    currentX += keyWidth + keyGap;
                }
                yStart += keyHeight + keyGap;
            }
        }
    }
}