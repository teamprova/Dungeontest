﻿#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Net;
using System.Net.Sockets;


#if MONOMAC
using MonoMac.AppKit;
using MonoMac.Foundation;

#elif __IOS__
using MonoTouch.Foundation;
using MonoTouch.UIKit;
#endif
#endregion

namespace DungeonTest
{
	#if __IOS__
	[Register("AppDelegate")]
	class Program : UIApplicationDelegate
	
#else
	static class Program
	#endif
    {
		private static DungeonTest game;

		internal static void RunGame ()
		{
			using (StreamWriter writer = new StreamWriter ("debug.txt")) {
				Console.SetOut(writer);
				game = new DungeonTest ();
				game.Run ();
			}
		}

		/// <summary>
		/// Grabs the player's IP
		/// </summary>
		/// <returns>Player's IP</returns>
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

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		#if !MONOMAC && !__IOS__		 
        [STAThread]
		#endif
		static void Main (string[] args)
		{
			#if MONOMAC
			NSApplication.Init ();

			using (var p = new NSAutoreleasePool ()) {
				NSApplication.SharedApplication.Delegate = new AppDelegate();
				NSApplication.Main(args);
			}
			#elif __IOS__
			UIApplication.Main(args, null, "AppDelegate");
			#else
			RunGame ();
			#endif
		}

		#if __IOS__
		public override void FinishedLaunching(UIApplication app)
		{
			RunGame();
		}
		#endif
	}

	#if MONOMAC
	class AppDelegate : NSApplicationDelegate
	{
		public override void FinishedLaunching (MonoMac.Foundation.NSObject notification)
		{
			AppDomain.CurrentDomain.AssemblyResolve += (object sender, ResolveEventArgs a) =>  {
				if (a.Name.StartsWith("MonoMac")) {
					return typeof(MonoMac.AppKit.AppKitFramework).Assembly;
				}
				return null;
			};
			Program.RunGame();
		}

		public override bool ApplicationShouldTerminateAfterLastWindowClosed (NSApplication sender)
		{
			return true;
		}
	}  
	#endif
}

