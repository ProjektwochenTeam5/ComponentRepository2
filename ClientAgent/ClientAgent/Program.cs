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
        static void Main(string[] args)
        {
            ConsoleRenderer r = new ConsoleRenderer();
            ConsoleInput i = new ConsoleInput();
            i.StartReading();

            Menu m = new Menu(new Collection<IRenderer>(){ r }, i);
            MenuButton btn = new MenuButton() { Enabled = true, LinkedKey = ConsoleKey.F1, Text = "F1 Quit", Visible = true };
            btn.ButtonKeyPressed += btn_ButtonKeyPressed;
            m.Buttons.Add(btn);
            m.Visible = true;
            m.Focused = true;
            m.Draw();
            /*
            TcpClient c = new TcpClient(new IPEndPoint(IPAddress.Any, 12345));
            Client cl = new Client(c);
            ClientMessageManager mgr = new ClientMessageManager(cl);
            cl.Connected += cl_Connected;
            cl.ReceivedTCPMessage += cl_ReceivedTCPMessage;
            cl.StartConnectionSearch();
            Console.WriteLine("Started Connection Search!\nPress [ENTER] to stop...");



            Console.ReadLine();

            cl.StopAll();

            Console.WriteLine("Stopped!");*/
        }

        static void btn_ButtonKeyPressed(object sender, EventArgs e)
        {
            Environment.Exit(0);
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
