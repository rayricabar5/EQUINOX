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
    public class Calculator
    {
        public float Calculate(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: calc <operation> <num1> <num2> ...");
                Console.WriteLine("Operations: -a (add), -s (subtract), -m (multiply), -d (divide)");
                return 0;
            }

            float[] numbers = new float[args.Length - 1];
            string operation = args[0];
            float answer;

            for (int i = 0; i < args.Length - 1; i++)
            {
                try
                {
                    numbers[i] = float.Parse(args[i + 1]);
                }
                catch (Exception err)
                {
                    Console.WriteLine(err.Message);
                    Console.WriteLine("One of the input is unable to be added");
                    Console.WriteLine($"Invalid number: {args[i + 1]}");
                    return 0;
                }
            }

            answer = numbers[0];

            if (numbers.Length == 1)
            {
                if (operation != "-a" && operation != "-s" && operation != "-m" && operation != "-d")
                {
                    Console.WriteLine($"Operation {operation} is not recognized.");
                    return 0;
                }
            }

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
                            Console.WriteLine("Error: Division by zero.");
                            return 0;
                        }
                        answer /= numbers[i];
                        break;
                    default:
                        Console.WriteLine($"Operation {operation} is not recognized.");
                        return 0;
                }
            }

            Console.WriteLine(answer);
            return answer;
        }
    }
}
