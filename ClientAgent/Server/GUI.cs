using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleGUI.Controls;
using ConsoleGUI.IO;

namespace Server
{
    public static class GUI
    {
        public static void Do()
        {
            Random r = new Random();

            int first = r.Next(0, 16);
            int second = r.Next(0, 16);
            while (first == second)
            {
                second = r.Next(0, 16);
            }

            Console.Clear();
            Console.BackgroundColor = (ConsoleColor)first;
            Console.ForegroundColor = (ConsoleColor)second;
            Console.Clear();
            Console.CursorVisible = false;

            Console.SetWindowSize(Console.LargestWindowWidth - 90, Console.LargestWindowHeight - 35);
            Console.SetCursorPosition(0, 0);
        }
    }
}
