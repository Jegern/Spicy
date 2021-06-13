using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;

namespace Spicy
{
    public partial class AddingSoundForm : Window
    {
        bool repetitionRateTextBoxGotFocus = false;
        internal bool SoundIsReady = false;
        internal MediaPlayerWithSound NewSound;
        readonly MediaPlayer mediaPlayer = new MediaPlayer();
        string playingSoundName = string.Empty;
        bool soundIsPaused = false;

        public AddingSoundForm()
        {
            InitializeComponent();
        }

        private void InitializeOtherComponent()
        {
            if ((bool)(Owner as MainForm).SfxMenu.Tag || (bool)(Owner as MainForm).TemplateMenu.Tag)
                Width = Owner.Width / 3;
            else
                Width = Owner.Width / 2;
            Height = Owner.Height;
            Top = Owner.Top;
            mediaPlayer.MediaEnded += SoundEnded;
        }

        private void SoundEnded(object sender, EventArgs e)
        {
            Button button = ListBoxFunctions.FindButton(ListBoxOfSounds, playingSoundName, "Sound");
            button.Background = Resources["Play"] as ImageBrush;
            playingSoundName = string.Empty;
        }

        private void AddingSoundWindow_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeOtherComponent();
            ListBoxFunctions.LoadFileNamesFromFolderToList(ListBoxOfSounds, "sounds", "mp3");
            ListBoxFunctions.RemoveNamesOfFirstListBoxFromSecondListBox((Owner as MainForm).ListBoxOfSounds, ListBoxOfSounds);
            ListBoxFunctions.SortAscending(ListBoxOfSounds);
        }

        private void PlaySoundButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            string melodyName = button.DataContext.ToString();
            if (playingSoundName == string.Empty || melodyName != playingSoundName)
                StartMelody(button);
            else if (playingSoundName != string.Empty)
                ContinuePlayOrPauseMelody(button);
        }

        private void StartMelody(Button button)
        {
            string melodyName = button.DataContext.ToString();
            ClearMelody();
            PlayMelody(melodyName);
            button.Background = Resources["Pause"] as ImageBrush;
            playingSoundName = melodyName;
        }

        private void ClearMelody()
        {
            if (playingSoundName != string.Empty)
            { 
                ListBoxItem item = ListBoxOfSounds.ItemContainerGenerator.ContainerFromItem(playingSoundName) as ListBoxItem;
                Button button = item.Template.FindName("PlaySoundButton", item) as Button;
                button.Background = Resources["Play"] as ImageBrush;
                //Grid grid = (button.Parent as Grid).Parent as Grid;
                //grid.RowDefinitions[1].Height = new GridLength(0);
            }
        }

        private void PlayMelody(string name)
        {
            mediaPlayer.Open(new Uri("sounds/" + name + ".mp3", UriKind.Relative));
            mediaPlayer.Play();
            soundIsPaused = false;
        }

        private void ContinuePlayOrPauseMelody(Button button)
        {
            if (soundIsPaused)
                ContinuePlayMelody(button);
            else
                PauseMelody(button);
        }

        private void ContinuePlayMelody(Button button)
        {
            mediaPlayer.Play();
            soundIsPaused = false;
            button.Background = Resources["Pause"] as ImageBrush;

        }

        private void PauseMelody(Button button)
        {
            mediaPlayer.Pause();
            soundIsPaused = true;
            button.Background = Resources["Play"] as ImageBrush;

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
            AddSoundIfItIsReady();
        }

        private void AddSoundIfItIsReady()
        {
            CheckIfSoundIsReady();
            if (SoundIsReady)
            {
                string name = ListBoxOfSounds.SelectedItem.ToString();
                double volume = SoundVolumeSlider.Value;
                int repetitionRate = SoundRepetitionRateTextbox.Text.Length > 0 ? Convert.ToInt32(SoundRepetitionRateTextbox.Text) : 0;
                NewSound = new MediaPlayerWithSound(name, volume, repetitionRate);
                Close();
            }
        }

        private void ListBoxOfSounds_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            AddSoundIfItIsReady();
        }

        private void CheckIfSoundIsReady()
        {
            if (ListBoxOfSounds.SelectedItem == null)
                MessageBox.Show("Пожалуйста, выберите звук", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            else
                SoundIsReady = true;
        }

        private void AddingSoundWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            mediaPlayer.Close();
        }
    }
}