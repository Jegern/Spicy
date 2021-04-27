using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace Spicy
{
    public partial class AddingSoundForm : Window
    {
        MainForm.Sound sound;
        bool soundRepetitionRateTextboxGotFocus = false;

        public AddingSoundForm()
        {
            InitializeComponent();
            InitializeOtherComponent();
        }

        void InitializeOtherComponent()
        {
            ListBoxFunctions.LoadFileNamesFromFolderToList(ListBoxOfAllSounds, "sounds", "mp3");
            ListBoxFunctions.SortAscending(ListBoxOfAllSounds);
        }

        void ListOfSounds_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            string name = ListBoxOfAllSounds.SelectedItem.ToString();
            double volume = SoundVolumeSlider.Value;
            int repetitionRate = SoundRepetitionRateTextbox.Text.Length > 0 ? Convert.ToInt32(SoundRepetitionRateTextbox.Text) : 0;
            sound = new MainForm.Sound(name, volume, repetitionRate);
        }

        private void SoundRepetitionRateTextbox_GotFocus(object sender, RoutedEventArgs e)
        {
            soundRepetitionRateTextboxGotFocus = true;
        }

        private void SoundRepetitionRateTextbox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (soundRepetitionRateTextboxGotFocus)
            {
                soundRepetitionRateTextboxGotFocus = false;
                (sender as TextBox).SelectAll();
            }
        }

        void AddSoundButton_Click(object sender, RoutedEventArgs e)
        {
            if (sound != null)
            {
                (Owner as MainForm).PlaySoundAndAddToListBoxOfSounds(sound);
                Close();
            }
            else
                MessageBox.Show("Пожалуйста, выберите звук", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
}
