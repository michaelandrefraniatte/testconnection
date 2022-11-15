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
using WebSocketSharp;
using WebSocketSharp.Server;
namespace testconnectionwebsocketserver
{
    internal class Program
    {
        [DllImport("kernel32.dll")]
        private static extern bool SetConsoleCtrlHandler(ConsoleEventDelegate callback, bool add);
        private static ConsoleEventDelegate handler;
        private delegate bool ConsoleEventDelegate(int eventType);
        private static ThreadStart threadstart;
        private static Thread thread;
        public static bool closed = false;
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
            closed = true;
            Thread.Sleep(100);
            LSPControl.Disconnect();
        }
    }
    public class LSPControl
    {
        public static string localip;
        public static string port;
        public static WebSocketServer wss;
        public static byte[] rawdataavailable;
        public static void Connect()
        {
            try
            {
                localip = GetLocalIP();
                port = "62000";
                String connectionString = "ws://" + localip + ":" + port;
                wss = new WebSocketServer(connectionString);
                wss.AddWebSocketService<Audio>("/Audio");
                wss.Start();
                Console.WriteLine("ok");
            }
            catch { }
        }
        public static void Disconnect()
        {
            wss.RemoveWebSocketService("/Audio");
            wss.Stop();
        }
        public static string GetLocalIP()
        {
            string firstAddress = (from address in NetworkInterface.GetAllNetworkInterfaces().Select(x => x.GetIPProperties()).SelectMany(x => x.UnicastAddresses).Select(x => x.Address)
                                   where !IPAddress.IsLoopback(address) && address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork
                                   select address).FirstOrDefault().ToString();
            return firstAddress;
        }
    }
    public class Audio : WebSocketBehavior
    {
        protected override void OnMessage(MessageEventArgs e)
        {
            base.OnMessage(e);
            while (!Program.closed)
            {
                if (LSPControl.rawdataavailable != null)
                {
                    try
                    {
                        Send(LSPControl.rawdataavailable);
                        LSPControl.rawdataavailable = null;
                    }
                    catch { }
                }
                System.Threading.Thread.Sleep(10);
            }
        }
    }
}