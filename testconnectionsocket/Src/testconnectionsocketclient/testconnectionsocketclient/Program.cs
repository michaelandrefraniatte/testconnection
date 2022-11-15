using System;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Drawing;
using System.Net.NetworkInformation;
using System.Diagnostics;
using System.Text;
using System.Data;
using System.Globalization;
using System.Collections.Generic;
namespace testconnectionsocketclient
{
    internal class Program
    {
        [DllImport("kernel32.dll")]
        private static extern bool SetConsoleCtrlHandler(ConsoleEventDelegate callback, bool add);
        private static ConsoleEventDelegate handler;
        private delegate bool ConsoleEventDelegate(int eventType);
        private static ThreadStart threadstart;
        private static Thread thread;
        static void Main(string[] args)
        {
            handler = new ConsoleEventDelegate(ConsoleEventCallback);
            SetConsoleCtrlHandler(handler, true);
            Task.Run(() => LSPControl.Connect());
            Console.ReadLine();
        }
        static bool ConsoleEventCallback(int eventType)
        {
            if (eventType == 2)
            {
                threadstart = new ThreadStart(FormClose);
                thread = new Thread(threadstart);
                thread.Start();
                Thread.Sleep(10000);
            }
            return false;
        }
        private static void FormClose()
        {
            LSPControl.Disconnect();
        }
    }
    public class LSPControl
    {
        private static IPEndPoint ipEnd;
        private static Socket client;
        public static string ip;
        public static int port;
        public static string control;
        public static void Connect()
        {
            Console.WriteLine("start");
            ip = "10.0.0.13";
            port = Convert.ToInt32(62000);
            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            while (!client.Connected)
            {
                try
                {
                    ipEnd = new IPEndPoint(IPAddress.Parse(ip), port);
                    client.Blocking = true;
                    client.Connect(ipEnd);
                }
                catch { }
                System.Threading.Thread.Sleep(1);
            }
            Console.WriteLine("ok");
        }
        public static void Disconnect()
        {
            client.Close();
        }
        public static byte[] controlToByteArray(string control)
        {
            byte[] data = Encoding.ASCII.GetBytes(control);
            return data;
        }
    }
}
