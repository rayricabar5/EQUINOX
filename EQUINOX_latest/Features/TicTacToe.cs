using System;

namespace EQUINOX.Features
{
    public class TicTacToe
    {
        private static char[] board;
        private static int turn;

        // defined colors for easy changing later
        private const ConsoleColor ColorX = ConsoleColor.Red;
        private const ConsoleColor ColorO = ConsoleColor.Cyan;
        private const ConsoleColor ColorGrid = ConsoleColor.DarkGray;
        private const ConsoleColor ColorText = ConsoleColor.White;

        public void Run()
        {
            char player = 'X';
            turn = 0;
            bool running = true;

            board = new char[] { '1', '2', '3', '4', '5', '6', '7', '8', '9' };

            // Intro Screen
            Console.Clear();
            DrawHeader("TIC TAC TOE");

            Console.WriteLine("\n\n");
            CenterText("Player 1 controls X", ColorX);
            CenterText("Player 2 controls O", ColorO);
            Console.WriteLine("\n\n");
            CenterText("Press any key to start battle...", ConsoleColor.Gray);
            Console.ReadKey();

            while (running)
            {
                Console.Clear();
                DrawHeader("TIC TAC TOE");

                // Show whose turn it is
                Console.WriteLine();
                if (player == 'X')
                    CenterText($">>> PLAYER X TURN <<<", ColorX);
                else
                    CenterText($">>> PLAYER O TURN <<<", ColorO);

                DrawBoard();

                Console.WriteLine();
                CenterText("Choose a number (1-9) or 'q' to quit:", ConsoleColor.Gray);

                // Custom input position (centered)
                int consoleCenter = Console.WindowWidth / 2;
                Console.SetCursorPosition(consoleCenter - 1, Console.CursorTop);

                string input = Console.ReadLine();

                if (input.ToLower() == "q")
                {
                    running = false;
                    break;
                }

                int choice;
                if (int.TryParse(input, out choice) && choice >= 1 && choice <= 9)
                {
                    int index = choice - 1;

                    if (board[index] != 'X' && board[index] != 'O')
                    {
                        board[index] = player;
                        turn++;

                        if (CheckWin())
                        {
                            Console.Clear();
                            DrawHeader("GAME OVER");
                            DrawBoard();
                            Console.WriteLine("\n");

                            if (player == 'X')
                                CenterText("!!! PLAYER X WINS !!!", ColorX);
                            else
                                CenterText("!!! PLAYER O WINS !!!", ColorO);

                            Console.WriteLine("\n");
                            CenterText("Press Enter to return to OS.", ConsoleColor.Gray);
                            Console.ReadLine();
                            running = false;
                        }
                        else if (turn == 9)
                        {
                            Console.Clear();
                            DrawHeader("GAME OVER");
                            DrawBoard();
                            Console.WriteLine("\n");
                            CenterText("!!! IT'S A DRAW !!!", ConsoleColor.Yellow);
                            Console.WriteLine("\n");
                            CenterText("Press Enter to return to OS.", ConsoleColor.Gray);
                            Console.ReadLine();
                            running = false;
                        }
                        else
                        {
                            player = (player == 'X') ? 'O' : 'X';
                        }
                    }
                    else
                    {
                        // Feedback for invalid move
                        Console.Beep();
                    }
                }
            }
        }

        private void DrawBoard()
        {
            Console.WriteLine();
            // We use helper methods to print rows so we can color X and O individually
            PrintRow(0, 1, 2);
            PrintDivider();
            PrintRow(3, 4, 5);
            PrintDivider();
            PrintRow(6, 7, 8);
            Console.WriteLine();
        }

        private void PrintRow(int a, int b, int c)
        {
            // Calculate padding to center the board
            // Assumes a board width of roughly 13 characters
            // "  X  |  O  |  X  "
            int pad = (Console.WindowWidth - 13) / 2;
            Console.Write(new string(' ', pad));

            PrintCell(board[a]);
            PrintGrid(" | ");
            PrintCell(board[b]);
            PrintGrid(" | ");
            PrintCell(board[c]);
            Console.WriteLine();
        }

        private void PrintDivider()
        {
            int pad = (Console.WindowWidth - 13) / 2;
            Console.Write(new string(' ', pad));
            Console.ForegroundColor = ColorGrid;
            Console.WriteLine("_____|_____|_____");
            Console.ResetColor();
        }

        private void PrintCell(char val)
        {
            Console.Write("  "); // spacing
            if (val == 'X')
            {
                Console.ForegroundColor = ColorX;
                Console.Write(val);
            }
            else if (val == 'O')
            {
                Console.ForegroundColor = ColorO;
                Console.Write(val);
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkGray; // Dim numbers
                Console.Write(val);
            }
            Console.Write("  "); // spacing
            Console.ResetColor();
        }

        private void PrintGrid(string str)
        {
            Console.ForegroundColor = ColorGrid;
            Console.Write(str);
            Console.ResetColor();
        }

        // --- VISUAL HELPERS ---

        private void DrawHeader(string title)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(new string('=', Console.WindowWidth));

            CenterText(title, ConsoleColor.Yellow);

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(new string('=', Console.WindowWidth));
            Console.ResetColor();
        }

        private void CenterText(string text, ConsoleColor color)
        {
            int spaces = (Console.WindowWidth - text.Length) / 2;
            Console.Write(new string(' ', spaces));
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ResetColor();
        }

        private bool CheckWin()
        {
            // Horizontal
            if (board[0] == board[1] && board[1] == board[2]) return true;
            if (board[3] == board[4] && board[4] == board[5]) return true;
            if (board[6] == board[7] && board[7] == board[8]) return true;

            // Vertical
            if (board[0] == board[3] && board[3] == board[6]) return true;
            if (board[1] == board[4] && board[4] == board[7]) return true;
            if (board[2] == board[5] && board[5] == board[8]) return true;

            // Diagonal
            if (board[0] == board[4] && board[4] == board[8]) return true;
            if (board[2] == board[4] && board[4] == board[6]) return true;

            return false;
        }
    }
}