using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace Pandell
{
    class Program
    {
        private const int
            DefaultNumberMin = 1,
            DefaultNumberMax = 10000;
        private const string filename = "shuffledNumbers.txt";
        private static string filepath = Path.Combine(Environment.CurrentDirectory, filename);

        private static Random random = new Random();

        static void Main(string[] args)
        {
            string quitResult = null;
            do
            {
                // Clear the output in case we're running again
                Console.Clear();

                // Print title
                Print("Welcome to the Pandell Shuffle Sort Program!\r\n", ConsoleColor.Cyan);

                // Get the values (not required)
                var min = Prompt("Please enter the minimum value to shuffle (default 1): ");
                var max = Prompt("Please enter the minimum value to shuffle (default 10, 000): ");

                //Parse into int's
                if (!int.TryParse(min, out int minValue))
                    minValue = DefaultNumberMin;
                if (!int.TryParse(max, out int maxValue))
                    maxValue = DefaultNumberMax;

                // Generate an array from min to max
                var arr = Enumerable.Range(minValue, maxValue - minValue + 1).ToArray();

                // Fisher-Yates shuffle
                for (int i = 0; i < arr.Length; i++)
                {
                    var j = random.Next(i, arr.Length);
                    int iValue = arr[i];
                    arr[i] = arr[j];
                    arr[j] = iValue;
                }

                // Figure out if we should print to the console or to a file, if the output is large
                bool shouldPrintToFile = true;
                if (arr.Length > 100)
                    shouldPrintToFile = Prompt("There are more than 100 numbers. Print shuffle to a file? //(Y)es or //(N)o: ") == "y";

                string strArray = string.Join(", ", arr);
                if (shouldPrintToFile)
                {
                    Console.WriteLine($"\r\nNumbers shuffled, and saved to file at {filepath}");
                    PrintFile(strArray);

                    if (Prompt("//(O)pen file, or any other key to continue: ") == "o")
                        OpenFile();
                }
                else
                    Console.WriteLine($"\r\nShuffled numbers: {strArray}");

                // Prompt to exit
                quitResult = Prompt("\r\n\r\nDone! //(Q)uit, or any key to go again: ");
            }
            while (quitResult != "q");
        }

        static string Prompt(string message)
        {
            // Do some fanciness to highlight the options in a different color
            string[] prompt = message.Split("//");

            // If we don't find any 'escaped' input options, just print the prompt
            if (prompt.Length == 1)
                Print(message);
            else
            {
                // Loop through, find the 'escaped' input options, and color them yellow
                for (int i = 0; i < prompt.Length; i++)
                {
                    if (prompt[i].Contains("("))
                    {
                        // The option will always be the first 3 characters
                        // Grab that portion, and print it yellow
                        var option = prompt[i].Substring(0, 3);
                        Print(option, ConsoleColor.Yellow);

                        // If there's anything after the option, print that in normal white
                        if (prompt[i].Length > 3)
                            Print(prompt[i].Substring(3));
                    }
                    else
                        Print(prompt[i]);
                }

            }
            return Console.ReadLine().ToLowerInvariant();
        }

        static void Print(string message, ConsoleColor consoleColor = ConsoleColor.White)
        {
            // Save the original color
            var oldColor = Console.ForegroundColor;

            // Change the color to print the message
            Console.ForegroundColor = consoleColor;
            Console.Write(message);

            // Reset the color so other WriteLines aren't affected
            Console.ForegroundColor = oldColor;
        }

        static void PrintFile(string array)
        {
            File.WriteAllLines(filepath, new string[]
            {
                "Pandell Number Shuffle!\r\n\r\n",
                array
            });
        }

        static void OpenFile()
        {
            // Need some fanciness to ensure we can open the file on multiple OS's
            // as per MS github: https://github.com/dotnet/runtime/issues/17938

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                Process.Start(new ProcessStartInfo("cmd", $"/c start {filepath}"));
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                Process.Start("xdg-open", filepath);
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                Process.Start("open", filepath);
        }
    }
}
