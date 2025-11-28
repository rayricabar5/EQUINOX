using System;
using Console = System.Console;

namespace EQUINOX.Features
{
    public class Calculator
    {
        public float Calculate(string[] args)
        {
            // Visual Improvement: Clean Usage Guide
            if (args.Length < 2)
            {
                PrintUsage();
                return 0;
            }

            float[] numbers = new float[args.Length - 1];
            string operation = args[0];
            float answer;

            // Parse numbers with visual error handling
            for (int i = 0; i < args.Length - 1; i++)
            {
                try
                {
                    numbers[i] = float.Parse(args[i + 1]);
                }
                catch
                {
                    PrintError($"Invalid number format: '{args[i + 1]}'");
                    return 0;
                }
            }

            answer = numbers[0];

            // Validation for single number input
            if (numbers.Length == 1)
            {
                if (!isValidOp(operation))
                {
                    PrintError($"Unknown operation: '{operation}'");
                    return 0;
                }
            }

            // Calculation Loop
            for (int i = 1; i < numbers.Length; i++)
            {
                switch (operation)
                {
                    case "-a":
                        answer += numbers[i];
                        break;
                    case "-s":
                        answer -= numbers[i];
                        break;
                    case "-m":
                        answer *= numbers[i];
                        break;
                    case "-d":
                        if (numbers[i] == 0)
                        {
                            PrintError("Cannot divide by zero.");
                            return 0;
                        }
                        answer /= numbers[i];
                        break;
                    default:
                        PrintError($"Unknown operation: '{operation}'");
                        return 0;
                }
            }

            // Print Final Result nicely
            PrintResult(answer);
            return answer;
        }

        // --- Helper Methods for Clean Code ---

        private bool isValidOp(string op)
        {
            return op == "-a" || op == "-s" || op == "-m" || op == "-d";
        }

        private void PrintUsage()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("  [?] Usage: ");
            Console.ResetColor();
            Console.WriteLine("calc <operation> <number1> <number2> ...");

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("      Operations: -a (add) | -s (sub) | -m (mul) | -d (div)");
            Console.ResetColor();
        }

        private void PrintError(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("  [!] Error: ");
            Console.ResetColor();
            Console.WriteLine(msg);
        }

        private void PrintResult(float result)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("  [=] Result: ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(result);
            Console.ResetColor();
        }
    }
}