using System;
using System.IO;
using System.Globalization;
using System.Collections.ObjectModel;

namespace Spicy
{
    public static class FileWork
    {
        public static void WriteSoundCollectionToFile(ObservableCollection<MainForm.Sound> collection, string fileName)
        {
            using (BinaryWriter writer = new BinaryWriter(File.Create("custom templates/" + fileName + ".bin")))
                foreach (var sound in collection)
                {
                    string name = sound.Name + ".mp3";
                    string volume = sound.Volume.ToString();
                    string repetitionRate = sound.RepetitionRate.ToString();
                    writer.Write(name + " " + volume + " " + repetitionRate);
                }
        }

        public static void ReadFileToSoundCollection(ref ObservableCollection<MainForm.Sound> collection, string fileName)
        {
            using (BinaryReader reader = new BinaryReader(File.OpenRead("custom templates/" + fileName + ".bin")))
                while (reader.PeekChar() != -1)
                {
                    string[] nameAndSettings = reader.ReadString().Split(new[] { ".mp3" }, StringSplitOptions.None);
                    string[] settings = nameAndSettings[1].Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    string name = nameAndSettings[0];
                    double volume = double.Parse(settings[0], CultureInfo.InvariantCulture);
                    int repetitionRate = Convert.ToInt32(settings[1]);
                    collection.Add(new MainForm.Sound(name, volume, repetitionRate));
                }
        }
    }
}
