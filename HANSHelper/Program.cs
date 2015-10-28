using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HANSHelper
{
    class Program
    {
        enum Screen
        {
            MainMenu, Extract
        }

        static void Main(string[] args)
        {
            Screen screen = Screen.MainMenu;

            while (true)
            {
                switch(screen)
                {
                    case Screen.MainMenu:
                        Console.Clear();
                        Console.WriteLine("HANSHelper - by VegaRoXas");
                        Console.WriteLine("1 - Extract exe-/romfs");
                        Console.WriteLine("0 - Exit");
                        switch (Console.ReadKey().Key)
                        {
                            case ConsoleKey.D1:
                                screen = Screen.Extract;
                                break;
                            case ConsoleKey.D0:
                                if (File.Exists("ctrtool.exe"))
                                    File.Delete("ctrtool.exe");
                                return;
                        }
                        break;
                    case Screen.Extract:
                        Console.Clear();
                        if (!File.Exists("ctrtool.exe"))
                        {
                            File.WriteAllBytes("ctrtool.exe", Properties.Resources.ctrtool);
                        }
                        Console.WriteLine("What do you want to extract? [e]xefs, [r]omfs or [b]oth");
                        switch (Console.ReadKey().Key)
                        {
                            case ConsoleKey.E:
                                AskExeFS();
                                screen = Screen.MainMenu;
                                break;
                            case ConsoleKey.R:
                                AskRomFS();
                                screen = Screen.MainMenu;
                                break;
                            case ConsoleKey.B:
                                AskExeFS();
                                AskRomFS();
                                screen = Screen.MainMenu;
                                break;
                            default:
                                Console.WriteLine("\nThis is not an option!");
                                Console.ReadKey();
                                screen = Screen.MainMenu;
                                break;
                        }
                        break;
                }
            }
        }

        static void ExtractExeFS(string FileName, string OutputDirectory, bool DecompressCode)
        {
            using (Process p = Process.Start(new ProcessStartInfo("ctrtool.exe", "-t exefs --exefsdir=" + OutputDirectory + " " + (DecompressCode ? "--decompresscode " : "") + FileName) { UseShellExecute = false, RedirectStandardOutput = true }))
            using (StreamReader outputReader = p.StandardOutput)
            {
                string line;
                while ((line = outputReader.ReadLine()) != null)
                {
                    Console.WriteLine(line);
                }
                p.WaitForExit();
            }
            Console.WriteLine("Finished extracting exefs! Press any key to return to main menu.");
            Console.ReadKey();
        }
        static void ExtractRomFS(string FileName, string OutputDirectory)
        {
            using (Process p = Process.Start(new ProcessStartInfo("ctrtool.exe", "-t romfs --romfsdir=" + OutputDirectory + " " + FileName) { UseShellExecute = false, RedirectStandardOutput = true}))
            using (StreamReader outputReader = p.StandardOutput)
            {
                string line;
                while ((line = outputReader.ReadLine()) != null)
                {
                    Console.WriteLine(line);
                }
                p.WaitForExit();
                Console.WriteLine("Finished extracting romfs! Press any key to return to main menu.");
                Console.ReadKey();
            }
        }
        static void AskRomFS()
        {
            Console.WriteLine("\nEnter path of your romfs file (default: romfs.bin)");
            string path = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(path))
            {
                path = "romfs.bin";
            }
            if (!File.Exists(path))
            {
                Console.WriteLine("That file doesn't exist!");
                Console.ReadKey();
                return;
            }
            Console.WriteLine("Enter the output directory (default: romfs)");
            string outputDirectory = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(outputDirectory))
                outputDirectory = "romfs";

            ExtractRomFS(path, outputDirectory);
        }
        static void AskExeFS()
        {
            Console.WriteLine("\nEnter path of your exefs file (default: exefs.bin)");
            string path = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(path))
            {
                path = "exefs.bin";
            }
            else if (!File.Exists(path))
            {
                Console.WriteLine("That file doesn't exist!");
                Console.ReadKey();
                return;
            }
            Console.WriteLine("Enter the output directory (default: exefs)");
            string outputDirectory = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(outputDirectory))
                outputDirectory = "exefs";
            Console.WriteLine("Do you want to decompress the code.bin? [y]es [n]o (default: yes)");

            ExtractExeFS(path, outputDirectory, ParseBool(Console.ReadKey().KeyChar, true));
        }
        static bool ParseBool(string yesNo, bool defaultValue)
        {
            yesNo = yesNo.ToLower();
            if (yesNo.StartsWith("y"))
                return true;
            else if (yesNo.StartsWith("n"))
                return false;
            else
                return defaultValue;
        }
        static bool ParseBool(char yesNo, bool defaultValue)
        {
            yesNo = char.ToLower(yesNo);
            if (yesNo == 'y')
                return true;
            else if (yesNo == 'n')
                return false;
            else
                return defaultValue;
        }
    }
}
