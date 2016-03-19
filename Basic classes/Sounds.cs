using System.IO;
using Microsoft.Xna.Framework.Audio;

namespace Dungeontest
{
	public class Sounds
	{
        SoundEffect sound;

        public Sounds(string src)
        {
            using (FileStream stream = File.Open(src, FileMode.Open))
            {
                sound = SoundEffect.FromStream(stream);
                stream.Close();
            }
        }

		// Play a Sound
		public void Play(float volume, float pan)
		{
            sound.Play(volume, 1, pan);
		}
	}
}

