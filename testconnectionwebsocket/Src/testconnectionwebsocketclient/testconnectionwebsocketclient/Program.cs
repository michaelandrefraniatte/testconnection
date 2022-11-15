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
namespace testconnectionwebsocketclient
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
        public static string ip;
        public static string port;
        public static WebSocket wsc;
        public static void Connect()
        {
            ip = "192.168.1.13";
            port = "62000";
            String connectionString = "ws://" + ip + ":" + port + "/Audio";
            wsc = new WebSocket(connectionString);
            wsc.OnMessage += Ws_OnMessage;
            while (!wsc.IsAlive)
            {
                try
                {
                    wsc.Connect();
                    wsc.Send("Hello from client");
                }
                catch { }
                System.Threading.Thread.Sleep(1);
            }
            Console.WriteLine("ok");
        }
        private static void Ws_OnMessage(object sender, MessageEventArgs e)
        {
            byte[] rawdata = e.RawData;
        }
        public static void Disconnect()
        {
            wsc.Close();
        }
    }
}
