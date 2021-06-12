using System.Windows.Media;

namespace Spicy
{
    public class MediaPlayerWithSound : MediaPlayer
    {
        public MediaPlayerWithSound(string Name, double Volume, double RepetitionRate)
        {
            this.Name = Name;
            this.SoundVolume = Volume;
            this.RepetitionRate = RepetitionRate;
        }

        public string Name { get; }

        public double SoundVolume { get; set; }

        public double RepetitionRate { get; set; }

        public bool IsPlaying { get; set; }
    }
}