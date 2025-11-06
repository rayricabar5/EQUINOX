using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Sys = Cosmos.System;

namespace EQUINOX
{
    public class Kernel : Sys.Kernel
    {

        //protected override void Start()
        //{
        //    CanvasCreate();
        //}

        protected override void BeforeRun()
        {

            Console.WriteLine();
            Console.WriteLine("Equinox Booted.");
            Console.WriteLine();
            Console.WriteLine("For the list of function, type \" help \"");
            Console.WriteLine();
        }


        protected override void Run()
        {
            string choice;

            Console.WriteLine();
            Console.Write("> ");

            choice = Console.ReadLine();

            commandHandler(choice);
        }

        protected override void AfterRun()
        {
            Console.WriteLine("Farewell");
        }

        void commandHandler(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return;
            }

            void main(string command, string[] args)
            {
                switch (command)
                {
                    case "help":
                        Help();
                        break;
                    case "echo":
                        Console.WriteLine(echo(args));
                        break;
                    case "calc":
                        Calculate(args);
                        break;
                    case "reboot":
                        Cosmos.System.Power.Reboot();
                        break;
                    case "shutdown":
                        Cosmos.System.Power.Shutdown();
                        break;
                    case "canvas":
                        CanvasCreate();
                        break;
                    default:
                        Console.WriteLine("Invalid command");
                        break;
                }
            }

            string[] sections = input.Split(' ');
            string command = sections[0];
            string[] args;

            if (sections.Length > 1)
            {
                args = new string[sections.Length - 1];
                for (int i = 1; i < sections.Length; i++)
                {
                    args[i - 1] = sections[i];
                }
            }
            else
            {
                args = new string[0];
            }

            main(command, args);
        }

        string echo(string[] words)
        {
            string output = string.Join(" ", words);

            return output;
        }

        void Help()
        {
            Console.WriteLine("- help");
            Console.WriteLine("- echo");
            Console.WriteLine("- calc");
            Console.WriteLine("- reboot");
            Console.WriteLine("- shutdown");
        }

        float Calculate(string[] args)
        {

            if (args.Length < 2)
            {
                Console.WriteLine("Usage: Calc <operation> <num1> <num2> ...");
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

        void CanvasCreate()
        {
            Canvas canvas;
            Pen yellow = new Pen(Color.Yellow);
            var font = PCScreenFont.Default;

            canvas = FullScreenCanvas.GetFullScreenCanvas(new Mode(1024, 768, ColorDepth.ColorDepth32));

            canvas.Clear(Color.DarkSlateGray);

            // E
            canvas.DrawFilledRectangle(yellow, 50, 50, 100, 30);
            canvas.DrawFilledRectangle(yellow, 50, 135, 100, 30);
            canvas.DrawFilledRectangle(yellow, 50, 220, 100, 30);
            canvas.DrawFilledRectangle(yellow, 50, 50, 30, 200);

            // Q
            canvas.DrawFilledRectangle(yellow, 170, 50, 95, 30);
            canvas.DrawFilledRectangle(yellow, 170, 50, 30, 200);
            canvas.DrawFilledRectangle(yellow, 265, 50, 30, 200);
            canvas.DrawFilledRectangle(yellow, 170, 220, 95, 30);
            canvas.DrawFilledRectangle(yellow, 230, 200, 20, 70);

            // U
            canvas.DrawFilledRectangle(yellow, 325, 50, 30, 200);
            canvas.DrawFilledRectangle(yellow, 420, 50, 30, 200);
            canvas.DrawFilledRectangle(yellow, 325, 220, 95, 30);

            // I
            canvas.DrawFilledRectangle(yellow, 475, 50, 30, 200);

            // N
            canvas.DrawFilledRectangle(yellow, 530, 50, 30, 200);
            canvas.DrawFilledRectangle(yellow, 620, 50, 30, 200);
            canvas.DrawFilledRectangle(yellow, 560, 80, 20, 42);
            canvas.DrawFilledRectangle(yellow, 575, 120, 22, 47);
            canvas.DrawFilledRectangle(yellow, 595, 160, 22, 47);
            canvas.DrawFilledRectangle(yellow, 615, 200, 28, 50);

            // O
            canvas.DrawFilledRectangle(yellow, 670, 50, 100, 30);
            canvas.DrawFilledRectangle(yellow, 670, 50, 30, 200);
            canvas.DrawFilledRectangle(yellow, 770, 50, 30, 200);
            canvas.DrawFilledRectangle(yellow, 670, 220, 100, 30);

            //X
            canvas.DrawFilledRectangle(yellow, 830, 50, 25, 46);
            canvas.DrawFilledRectangle(yellow, 850, 90, 25, 46);
            canvas.DrawFilledRectangle(yellow, 870, 130, 25, 46);
            canvas.DrawFilledRectangle(yellow, 890, 170, 25, 46);
            canvas.DrawFilledRectangle(yellow, 910, 200, 25, 50);

            canvas.DrawFilledRectangle(yellow, 830, 200, 22, 50);
            canvas.DrawFilledRectangle(yellow, 850, 170, 22, 46);
            canvas.DrawFilledRectangle(yellow, 890, 90, 22, 46);
            canvas.DrawFilledRectangle(yellow, 910, 50, 22, 50);


            canvas.Display();
            Console.ReadKey();
            canvas.Disable();
        }
    }
}
