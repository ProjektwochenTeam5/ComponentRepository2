using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public static class GUI
    {
        public static void Do()
        {
            Console.Clear();
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Clear();
            Console.CursorVisible = false;

            Console.SetWindowSize(Console.LargestWindowWidth - 50, Console.LargestWindowHeight - 25);


            for (int i = 0; i < Console.WindowHeight; i++)
            {
                Console.SetCursorPosition(Console.WindowWidth - 20, i);
                Console.Write("|");
            }

            for (int i = 0; i < Console.WindowHeight; i++)
            {
                Console.SetCursorPosition(Console.WindowWidth - 19, i);
                Console.Write("|");
            }

            for (int i = 0; i < Console.WindowWidth; i++)
            {
                Console.SetCursorPosition(i, Console.WindowHeight - 10);
                Console.Write("=");
            }

            for (int i = 0; i < Console.WindowWidth; i++)
            {
                Console.SetCursorPosition(i, Console.WindowHeight - 9);
                Console.Write("=");
            }

            Console.SetCursorPosition(0, 0);
        }
    }
}
