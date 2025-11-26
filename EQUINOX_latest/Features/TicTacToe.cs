using System;

namespace EQUINOX.Features
{
    public class TicTacToe
    {
        private static char[] board;
        private static int turn;

        public void Run()
        {
            char player = 'X';
            turn = 0;
            bool running = true;

            board = new char[] { '1', '2', '3', '4', '5', '6', '7', '8', '9' };

            Console.Clear();
            Console.WriteLine("--- TIC TAC TOE (Text Mode) ---");
            Console.WriteLine("Player 1: X  |  Player 2: O");
            Console.WriteLine("Press any key to start...");
            Console.ReadKey();

            while (running)
            {
                Console.Clear();
                Console.WriteLine("----------------");
                Console.WriteLine($" Player {player}'s Turn");
                Console.WriteLine("----------------");
                DrawBoard();
                Console.WriteLine();
                Console.Write("Choose a number (1-9) or 'q' to quit: ");

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
                            DrawBoard();
                            Console.WriteLine();
                            Console.WriteLine($"!!! PLAYER {player} WINS !!!");
                            Console.WriteLine("Press Enter to return to OS.");
                            Console.ReadLine();
                            running = false;
                        }
                        else if (turn == 9)
                        {
                            Console.Clear();
                            DrawBoard();
                            Console.WriteLine();
                            Console.WriteLine("!!! DRAW !!!");
                            Console.WriteLine("Press Enter to return to OS.");
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
                        Console.WriteLine("Spot already taken! Press Enter.");
                        Console.ReadLine();
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input. Press Enter.");
                    Console.ReadLine();
                }
            }
        }

        private void DrawBoard()
        {
            Console.WriteLine();
            Console.WriteLine($"  {board[0]}  |  {board[1]}  |  {board[2]}");
            Console.WriteLine("_____|_____|_____");
            Console.WriteLine($"  {board[3]}  |  {board[4]}  |  {board[5]}");
            Console.WriteLine("_____|_____|_____");
            Console.WriteLine($"  {board[6]}  |  {board[7]}  |  {board[8]}");
            Console.WriteLine("     |     |     ");
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