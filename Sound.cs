namespace Spicy
{
    public partial class MainForm
    {
        public class Sound
        {
            public Sound(string Name, double Volume, int RepetitionRate)
            {
                this.Name = Name;
                this.Volume = Volume;
                this.RepetitionRate = RepetitionRate;
            }

            public string Name { get; }

            public double Volume { get; set; }

            public int RepetitionRate { get; set; }
        }
    }
}