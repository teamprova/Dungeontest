﻿using System;
using System.Net;
using System.Net.Sockets;

namespace DungeonTest
{
#if WINDOWS || LINUX
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {

        public static DungeonTest game;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            using (game = new DungeonTest())
            game.Run();
        }

        public static string getIPv4()
        {
            IPAddress[] localIPs = Dns.GetHostAddresses(Dns.GetHostName());
            foreach (IPAddress a in localIPs)
            {
                if (a.AddressFamily == AddressFamily.InterNetwork)
                    return a.ToString();
            }
            return "null";
        }

        public static void Close() {
            using (game = new DungeonTest())
            game.Exit();
        }

    }
#endif
}
