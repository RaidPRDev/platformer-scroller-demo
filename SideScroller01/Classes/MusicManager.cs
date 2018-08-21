using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;

namespace SideScroller01
{
    enum MusicState
    {
        Fading,
        Rising,
        Holding
    }

    static class MusicManager
    {
        public static MusicState State = MusicState.Holding;
        public static float CurrentVolume = 0.04f;
        public static float VolumeTarget = 0.02f;
        public static float RateOfChange = 0.0008f;

        public static void Update()
        {
            switch (State)
            {
                case MusicState.Rising:
                    if (MediaPlayer.Volume < VolumeTarget)
                    {
                        MediaPlayer.Volume += RateOfChange;
                        if (MediaPlayer.Volume > VolumeTarget)
                        {
                            MediaPlayer.Volume = VolumeTarget;
                            State = MusicState.Holding;
                        }
                    }
                    break;

                case MusicState.Fading:
                    if (MediaPlayer.Volume > VolumeTarget)
                    {
                        MediaPlayer.Volume -= RateOfChange;
                        if (MediaPlayer.Volume < VolumeTarget)
                        {
                            MediaPlayer.Volume = VolumeTarget;
                            State = MusicState.Holding;
                        }
                    }
                    break;
            }

        }

        public static void ChangeToVolume(float setTo)
        {
            VolumeTarget = setTo;

            if (VolumeTarget < MediaPlayer.Volume)
                State = MusicState.Fading;
            else
                State = MusicState.Rising;
        }

        public static void PlaySong(Song title)
        {
         
            MediaPlayer.Play(title);
            MediaPlayer.Volume = CurrentVolume;
        }

        public static void SetRepeating(bool IsRepeating)
        {
            MediaPlayer.IsRepeating = IsRepeating;
        }
       
        public static void StopSong()
        {
            MediaPlayer.Stop();

        }
    }
}
