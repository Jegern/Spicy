using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Media;

namespace Spicy
{
    public partial class AddingSoundForm : Window
    {
        MainForm.Sound newSound;

        public AddingSoundForm()
        {
            InitializeComponent();
        }

        void AddingSoundWindow_Loaded(object sender, RoutedEventArgs e)
        {
            DirectoryInfo directory = new DirectoryInfo("sounds/");
            foreach (var fileName in directory.GetFiles("*.mp3"))
                ListOfSounds.Items.Add(fileName.Name.Replace(".mp3", ""));
            ListOfSounds.Items.SortDescriptions.Add(new SortDescription("", ListSortDirection.Ascending));
        }

        void ListOfSounds_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            string name = ListOfSounds.SelectedItem.ToString();
            double volume = SoundVolumeSlider.Value;
            int repetitionRate = SoundRepetitionRateTextbox.Text.Length > 0 ? Convert.ToInt32(SoundRepetitionRateTextbox.Text) : 0;
            newSound = new MainForm.Sound(name, volume, repetitionRate);
        }

        void AddSoundButton_Click(object sender, RoutedEventArgs e)
        {
            if (newSound != null)
            {
                MainForm main = Owner as MainForm;
                AddSoundInListAndSort(main);
                PlaySound(main);
                main.RewriteTemplateFile();
                Close();
            }
            else
                MessageBox.Show("Вы не выбрали звук", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        void AddSoundInListAndSort(MainForm main)
        {
            main.AddSoundInCollectionOfSoundsAndSort(newSound);
        }

        void PlaySound(MainForm main)
        {
            int index = main.players.Count;
            main.players.Add(new MediaPlayer());
            main.players[index].MediaEnded += main.MediaPlayerSoundEnded;
            main.players[index].Volume = newSound.Volume;
            string soundPath = "sounds/" + newSound.Name + ".mp3";
            main.players[index].Open(new Uri(AppDomain.CurrentDomain.BaseDirectory + soundPath));
            main.players[index].Play();
        }
    }
}
