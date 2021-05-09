using System;
using System.IO;
using System.Globalization;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using static Spicy.MainForm;

namespace Spicy
{
    public static class FileWork
    {
        public static void WriteSoundCollectionToFile(ObservableCollection<Sound> collection, string fileName)
        {
            using (BinaryWriter writer = new BinaryWriter(File.Create("sound templates/" + fileName + ".bin")))
                foreach (var sound in collection)
                {
                    string name = sound.Name + ".mp3";
                    string volume = sound.Volume.ToString();
                    string repetitionRate = sound.RepetitionRate.ToString();
                    writer.Write(name + " " + volume + " " + repetitionRate);
                }
        }

        public static void WriteMelodiesToFile(ItemCollection collection, string fileName)
        {
            using (BinaryWriter writer = new BinaryWriter(File.Create("music templates/" + fileName + ".bin")))
                foreach (var melody in collection)
                    writer.Write((string)melody + ".mp3");
        }

        public static void ReadFileToSoundCollection(ObservableCollection<Sound> collection, string fileName)
        {
            using (BinaryReader reader = new BinaryReader(File.OpenRead("sound templates/" + fileName + ".bin")))
                while (reader.PeekChar() != -1)
                {
                    string[] nameAndSettings = reader.ReadString().Split(new[] { ".mp3" }, StringSplitOptions.None);
                    string[] settings = nameAndSettings[1].Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    string name = nameAndSettings[0];
                    double volume = double.Parse(settings[0], CultureInfo.InvariantCulture);
                    int repetitionRate = Convert.ToInt32(settings[1]);
                    collection.Add(new Sound(name, volume, repetitionRate));
                }
        }

        public static void ReadFileToListBox(ListBox listBox, string fileName)
        {
            using (BinaryReader reader = new BinaryReader(File.OpenRead("music templates/" + fileName + ".bin")))
                while (reader.PeekChar() != -1)
                {
                    string name = reader.ReadString().Replace(".mp3", "");
                    listBox.Items.Add(name);
                }
        }
    }
}