namespace ClientAgent
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading.Tasks;
    using ConsoleGUI.Controls;
    using ConsoleGUI.IO;
    using System.Collections.ObjectModel;

    public static class Program
    {
        private static ConsoleInput input;

        private static Menu mainMenu;

        static void Main(string[] args)
        {
            /*ConsoleRenderer r = new ConsoleRenderer();
            input = new ConsoleInput();
            input.StartReading();*/

            /*mainMenu = new Menu(new Collection<IRenderer>(){ r }, input);
            MenuButton btn = new MenuButton() { Enabled = true, LinkedKey = ConsoleKey.F1, Text = "F1 Quit", Visible = true };
            btn.ButtonKeyPressed += btn_ButtonKeyPressed;
            mainMenu.Buttons.Add(btn);
            mainMenu.Visible = true;
            mainMenu.Focused = true;*/
            
            TcpClient c = new TcpClient(new IPEndPoint(IPAddress.Any, 12345));
            Client cl = new Client(c);
            ClientMessageManager mgr = new ClientMessageManager(cl);
            cl.Connected += cl_Connected;
            cl.ReceivedTCPMessage += cl_ReceivedTCPMessage;
            cl.StartConnectionSearch();
            /*Label l = new Label(mainMenu.Renderers);
            l.Text = "Started Connection Search!\nPress [ENTER] to stop...";
            l.Rectangle = new ConsoleGUI.Rectangle(0, 0, r.Width, 3);
            l.BackgroundColor = ConsoleColor.Blue;
            l.ForegroundColor = ConsoleColor.White;
            mainMenu.Controls.Add(l);

            mainMenu.Draw();

            mainMenu.WaitForClose();*/

            Console.ReadLine();
            cl.StopAll();


            Console.WriteLine("Stopped!");

            Console.ReadLine();
        }

        private static void btn_ButtonKeyPressed(object sender, EventArgs e)
        {
            mainMenu.Close();
        }

        static void cl_ReceivedTCPMessage(object sender, MessageReceivedEventArgs e)
        {
        }

        static void cl_Connected(object sender, EventArgs e)
        {
            Console.WriteLine("Connected to server!");
        }
    }
}
