using System;
using System.Windows;
using System.Windows.Controls;
using static Spicy.MainForm;

namespace Spicy
{
    public partial class AddingSoundForm : Window
    {
        MainForm owner;
        internal Sound NewSound;
        bool soundRepetitionRateTextboxGotFocus = false;

        public AddingSoundForm(Window owner)
        {
            InitializeComponent();
            InitializeOtherComponent(owner);
        }

        void InitializeOtherComponent(Window window)
        {
            Owner = window;
            owner = window as MainForm;
            ListBoxFunctions.LoadFileNamesFromFolderToList(ListBoxOfAllSounds, "sounds", "mp3");
            ListBoxFunctions.RemoveNamesOfFirstListBoxFromSecondListBox(owner.ListBoxOfSounds, ListBoxOfAllSounds);
            ListBoxFunctions.SortAscending(ListBoxOfAllSounds);
        }

        void ListOfSounds_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string name = ListBoxOfAllSounds.SelectedItem.ToString();
            double volume = SoundVolumeSlider.Value;
            int repetitionRate = SoundRepetitionRateTextbox.Text.Length > 0 ? Convert.ToInt32(SoundRepetitionRateTextbox.Text) : 0;
            NewSound = new Sound(name, volume, repetitionRate);
        }

        void SoundRepetitionRateTextbox_GotFocus(object sender, RoutedEventArgs e)
        {
            soundRepetitionRateTextboxGotFocus = true;
        }

        void SoundRepetitionRateTextbox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (soundRepetitionRateTextboxGotFocus)
            {
                soundRepetitionRateTextboxGotFocus = false;
                (sender as TextBox).SelectAll();
            }
        }

        void SoundVolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (NewSound != null)
                NewSound.Volume = (sender as Slider).Value;
        }

        void SoundRepetitionRateTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (NewSound != null)
                NewSound.RepetitionRate = Convert.ToInt32((sender as TextBox).Text);
        }

        void AddSoundButton_Click(object sender, RoutedEventArgs e)
        {
            if (SoundIsReadyToAdding())
            {
                ListBoxFunctions.AddSuitableObjectToSuitableListBox(this);
                //owner.PlaySound(sound);
                Close();
            }
        }

        bool SoundIsReadyToAdding()
        {
            bool isReady = true;
            if (NewSound == null)
            {
                MessageBox.Show("Пожалуйста, выберите звук", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                isReady = false;
            }
            return isReady;
        }
    }
}