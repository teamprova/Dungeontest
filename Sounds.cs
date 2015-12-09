using System;
using System.IO;
using MoonSharp.Interpreter;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

namespace DungeonTest
{
	public class Sounds
	{
		// Create a new sound effect

		public static int CreateSound(string name, string src)
		{
			int id = CoreGame.soundEffect.Count;
			Console.WriteLine("\n[dungeontest] creating sound '{0}' under id '{1}'\n", name, id);

			try
			{
				// add name to sound list
				CoreGame.soundNames.Add(name);

				// add sound to sound list
				using (FileStream stream = File.Open(src, FileMode.Open)) {
					SoundEffect myNewSoundEffect = SoundEffect.FromStream(stream);
					CoreGame.soundEffect.Add(myNewSoundEffect);
				}

				Console.WriteLine("\n[dungeontest] created sound '{0}' under id '{1}'\n", name, id);

			}
			catch (Exception ex)
			{
				Console.WriteLine("\n[dungeontest] error occured when creating sound '{0}' under id '{1}'. error: {2}\n", name, id, ex.Message);
			}

			return id;
		}

		// Play a Sound
		public static void PlaySound(string sound)
		{
			Console.WriteLine ("\n[dungeontest] playing sound '{0}'\n", sound);
			CoreGame.soundEffect [CoreGame.soundNames.IndexOf (sound)].Play ();
		}
	}
}

