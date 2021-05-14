namespace Spicy
{
    public class Sound
    {
        public Sound(string Name, double Volume, double RepetitionRate)
        {
            this.Name = Name;
            this.Volume = Volume;
            this.RepetitionRate = RepetitionRate;
        }

        public string Name { get; }

        public double Volume { get; set; }

        public double RepetitionRate { get; set; }
    }
}