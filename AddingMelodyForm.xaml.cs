using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;

namespace Spicy
{
    public partial class AddingMelodyForm : Window
    {
        internal bool MelodyIsReady = false;
        internal string NewMelody = string.Empty;
        readonly MediaPlayer mediaPlayer = new MediaPlayer();
        string playingMelodyName = string.Empty;
        bool melodyIsPaused = false;

        public AddingMelodyForm()
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
            mediaPlayer.MediaEnded += MelodyEnded;
        }

        private void MelodyEnded(object sender, EventArgs e)
        {
            Button button = ListBoxFunctions.FindButton(ListBoxOfMelodies, playingMelodyName, "Melody");
            button.Background = Resources["Play"] as ImageBrush;
            playingMelodyName = string.Empty;
        }

        private void AddingMelodyWindow_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeOtherComponent();
            ListBoxFunctions.LoadFileNamesFromFolderToList(ListBoxOfMelodies, "music", "mp3");
            ListBoxFunctions.RemoveNamesOfFirstListBoxFromSecondListBox((Owner as MainForm).ListBoxOfMelodies, ListBoxOfMelodies);
            ListBoxFunctions.SortAscending(ListBoxOfMelodies);
        }

        private void PlayMelodyButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            string melodyName = button.DataContext.ToString();
            if (playingMelodyName == string.Empty || melodyName != playingMelodyName)
                StartMelody(button);
            else if (playingMelodyName != string.Empty)
                ContinuePlayOrPauseMelody(button);
        }

        private void StartMelody(Button button)
        {
            string melodyName = button.DataContext.ToString();
            ClearMelody();
            PlayMelody(melodyName);
            button.Background = Resources["Pause"] as ImageBrush;
            playingMelodyName = melodyName;
        }

        private void ClearMelody()
        {
            if (playingMelodyName != string.Empty)
            { 
                ListBoxItem item = ListBoxOfMelodies.ItemContainerGenerator.ContainerFromItem(playingMelodyName) as ListBoxItem;
                Button button = item.Template.FindName("PlayMelodyButton", item) as Button;
                button.Background = Resources["Play"] as ImageBrush;
                //Grid grid = (button.Parent as Grid).Parent as Grid;
                //grid.RowDefinitions[1].Height = new GridLength(0);
            }
        }

        private void PlayMelody(string name)
        {
            mediaPlayer.Open(new Uri("music/" + name + ".mp3", UriKind.Relative));
            mediaPlayer.Play();
            melodyIsPaused = false;
        }

        private void ContinuePlayOrPauseMelody(Button button)
        {
            if (melodyIsPaused)
                ContinuePlayMelody(button);
            else
                PauseMelody(button);
        }

        private void ContinuePlayMelody(Button button)
        {
            mediaPlayer.Play();
            melodyIsPaused = false;
            button.Background = Resources["Pause"] as ImageBrush;

        }

        private void PauseMelody(Button button)
        {
            mediaPlayer.Pause();
            melodyIsPaused = true;
            button.Background = Resources["Play"] as ImageBrush;

        }

        private void AddMelodyButton_Click(object sender, RoutedEventArgs e)
        {
            AddMelodyIfItIsReady();
        }

        private void AddMelodyIfItIsReady()
        {
            CheckIfMelodyIsReady();
            if (MelodyIsReady)
            {
                NewMelody = ListBoxOfMelodies.SelectedItem.ToString();
                Close();
            }
        }

        private void ListBoxOfMelodies_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            AddMelodyIfItIsReady();
        }

        private void CheckIfMelodyIsReady()
        {
            if (ListBoxOfMelodies.SelectedItem == null)
                MessageBox.Show("Пожалуйста, выберите мелодию", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            else
                MelodyIsReady = true;
        }

        private void AddingMelodyWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            mediaPlayer.Close();
        }
    }
}