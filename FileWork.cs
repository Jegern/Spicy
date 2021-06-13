using System;
using System.IO;
using System.Windows;
using System.Globalization;
using System.Windows.Controls;
using System.Collections.ObjectModel;

namespace Spicy
{
    public static class FileWork
    {
        public static void WriteSoundCollectionToFile(ObservableCollection<MediaPlayerWithSound> collection, string fileName)
        {
            using (BinaryWriter writer = new BinaryWriter(File.Create("sound templates/" + fileName + ".bin")))
                foreach (var sound in collection)
                {
                    string name = sound.Name + ".mp3";
                    string volume = sound.SoundVolume.ToString();
                    string repetitionRate = sound.RepetitionRate.ToString();
                    writer.Write(name + " " + volume + " " + repetitionRate);
                }
        }

        public static void ReadFileToSoundCollection(ObservableCollection<MediaPlayerWithSound> collection, string fileName)
        {
            using (BinaryReader reader = new BinaryReader(File.OpenRead("sound templates/" + fileName + ".bin")))
                while (reader.PeekChar() != -1)
                {
                    string[] nameAndSettings = reader.ReadString().Split(new[] { ".mp3" }, StringSplitOptions.None);
                    string[] settings = nameAndSettings[1].Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    string name = nameAndSettings[0];
                    double volume = double.Parse(settings[0], CultureInfo.InvariantCulture);
                    int repetitionRate = Convert.ToInt32(settings[1]);
                    collection.Add(new MediaPlayerWithSound(name, volume, repetitionRate));
                }
        }

        public static void WriteMelodiesToFile(ItemCollection collection, string fileName)
        {
            using (BinaryWriter writer = new BinaryWriter(File.Create("music templates/" + fileName + ".bin")))
                foreach (var melody in collection)
                    writer.Write((string)melody + ".mp3");
        }

        public static void ReadFileToListBox(ListBox listBox, string fileName)
        {
            if (File.Exists("music templates/" + fileName + ".bin"))
                using (BinaryReader reader = new BinaryReader(File.OpenRead("music templates/" + fileName + ".bin")))
                    while (reader.PeekChar() != -1)
                    {
                        string name = reader.ReadString().Replace(".mp3", "");
                        listBox.Items.Add(name);
                    }
        }

        public static void WriteSfxGridToFile(Grid grid, string fileName)
        {
            using (BinaryWriter writer = new BinaryWriter(File.Create("sfx templates/" + fileName + ".bin")))
                for (int i = 1; i < grid.Children.Count - 1; i++)
                {
                    string name = (string)((grid.Children[i] as Grid).Children[1] as Button).Content;
                    writer.Write(name + ".mp3");
                }
        }

        public static void ReadFileToSfxGrid(Window window, Grid grid, string fileName)
        {
            if (File.Exists("sfx templates/" + fileName + ".bin"))
                using (BinaryReader reader = new BinaryReader(File.OpenRead("sfx templates/" + fileName + ".bin")))
                    for (int i = 1; i < grid.Children.Count - 1; i++)
                    {
                        string name = reader.ReadString().Replace(".mp3", "");
                        if (name != string.Empty)
                        {
                            Button button = (grid.Children[i] as Grid).Children[1] as Button;
                            MediaPlayerWithSound sound = new MediaPlayerWithSound(name, 1, 0);
                            Extensions.SetSound(button, sound);
                            (window as MainForm).ChangeSfxLayout(button);
                        }
                    }
        }

        public static void WriteSettingsToFile(Window window)
        {
            MainForm main = window as MainForm;
            using (BinaryWriter writer = new BinaryWriter(File.Create("settings.bin")))
            {
                for (int i = 0; i < main.volume.Length; i++)
                {
                    writer.Write(main.volume[i].ToString());
                    writer.Write(main.pastVolume[i].ToString());
                    writer.Write(main.volumeIsMute[i].ToString());
                }
                writer.Write(main.Width.ToString());
                writer.Write(main.Height.ToString());
                writer.Write(main.Left.ToString());
                writer.Write(main.Top.ToString());
            }
        }

        public static void ReadFileToSettings(Window window)
        {
            MainForm main = window as MainForm;
            using (BinaryReader reader = new BinaryReader(File.OpenRead("settings.bin")))
            {
                for (int i = 0; i < main.volume.Length; i++)
                {
                    main.volumeSliders[i].Value = main.volume[i] = Convert.ToDouble(reader.ReadString());
                    main.pastVolume[i] = Convert.ToDouble(reader.ReadString());
                    main.volumeIsMute[i] = Convert.ToBoolean(reader.ReadString());
                }
                main.Width = Convert.ToDouble(reader.ReadString());
                main.Height = Convert.ToDouble(reader.ReadString());
                main.Left = Convert.ToDouble(reader.ReadString());
                main.Top = Convert.ToDouble(reader.ReadString());
            }
        }
    }
}