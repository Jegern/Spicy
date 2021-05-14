using System;
using System.Windows;
using System.Windows.Controls;

namespace Spicy
{
    public partial class AddingSoundForm : Window
    {
        bool repetitionRateTextBoxGotFocus = false;
        internal bool SoundIsReady = false;
        internal Sound NewSound;

        public AddingSoundForm()
        {
            InitializeComponent();
        }

        private void AddingSoundWindow_Loaded(object sender, RoutedEventArgs e)
        {
            ListBoxFunctions.LoadFileNamesFromFolderToList(ListBoxOfSounds, "sounds", "mp3");
            ListBoxFunctions.RemoveNamesOfFirstListBoxFromSecondListBox((Owner as MainForm).ListBoxOfSounds, ListBoxOfSounds);
            ListBoxFunctions.SortAscending(ListBoxOfSounds);
        }

        void SoundRepetitionRateTextbox_GotFocus(object sender, RoutedEventArgs e)
        {
            repetitionRateTextBoxGotFocus = true;
        }

        void SoundRepetitionRateTextbox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (repetitionRateTextBoxGotFocus)
            {
                repetitionRateTextBoxGotFocus = false;
                (sender as TextBox).SelectAll();
            }
        }

        void AddSoundButton_Click(object sender, RoutedEventArgs e)
        {
            CheckIfSoundIsReady();
            if (SoundIsReady)
            {
                string name = ListBoxOfSounds.SelectedItem.ToString();
                double volume = SoundVolumeSlider.Value;
                int repetitionRate = SoundRepetitionRateTextbox.Text.Length > 0 ? Convert.ToInt32(SoundRepetitionRateTextbox.Text) : 0;
                NewSound = new Sound(name, volume, repetitionRate);
                Close();
            }
        }

        private void CheckIfSoundIsReady()
        {
            if (ListBoxOfSounds.SelectedItem == null)
                MessageBox.Show("Пожалуйста, выберите звук", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            else
                SoundIsReady = true;
        }
    }
}