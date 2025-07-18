using System;
using System.Diagnostics;
using System.Windows.Media;

namespace ArnoldVinkCode
{
    public partial class AVSoundPlayer
    {
        //Player Variables
        private static MediaPlayer windowsMediaPlayer = new MediaPlayer();

        //Play sound
        public static void PlaySound(string soundFilePath, double soundVolume)
        {
            try
            {
                Uri soundFileUri = new Uri(soundFilePath, UriKind.RelativeOrAbsolute);
                windowsMediaPlayer.Volume = soundVolume;
                windowsMediaPlayer.Open(soundFileUri);
                windowsMediaPlayer.Play();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to play sound: " + soundFilePath + " / " + ex.Message);
            }
        }
    }
}