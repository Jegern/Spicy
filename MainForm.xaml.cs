using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Spicy
{
    public partial class MainForm : Window
    {
        #region Инициализация
        public MainForm()
        {
            InitializeComponent();
            InitializeOtherComponent();
        }

        private void InitializeOtherComponent()
        {
            HideTemplateAndSfxMenu();
            InitializeListBoxOfTemplateSounds();
            LoadTemplatesInListBox();
        }

        private void HideTemplateAndSfxMenu()
        {
            TemplateMenu.Width = new GridLength(0);
            SfxMenu.Width = new GridLength(0);
            Width -= Width / 2;
        }

        private void InitializeListBoxOfTemplateSounds()
        {
            ListBoxOfSounds.ItemsSource = collectionOfSounds;
            ListBoxOfSounds.DisplayMemberPath = "Name";
        }

        private void LoadTemplatesInListBox()
        {
            ListBoxFunctions.LoadFileNamesFromFolderToList(ListBoxOfTemplates, "sound templates", "bin");
            ListBoxFunctions.SortAscending(ListBoxOfTemplates);
        }

        private void TemplateMenu_Click(object sender, RoutedEventArgs e)
        {
            MinimizeMenu(SfxMenu);
            ExpandMenu(TemplateMenu);
        }

        private void SfxMenu_Click(object sender, RoutedEventArgs e)
        {
            MinimizeMenu(TemplateMenu);
            ExpandMenu(SfxMenu);
        }

        private void MinimizeMenu(ColumnDefinition grid)
        {
            if (grid.Width != new GridLength(0))
            {
                grid.Width = new GridLength(0);
                Width -= Width / 3;
            }
        }

        private void ExpandMenu(ColumnDefinition grid)
        {
            if (grid.Width == new GridLength(0))
            {
                grid.Width = new GridLength(1, GridUnitType.Star);
                Width += Width / 2;
            }
        }
        #endregion


        #region Управление звуками
        readonly ObservableCollection<Sound> collectionOfSounds = new ObservableCollection<Sound>();

        private void AddSoundButton_Click(object sender, RoutedEventArgs e)
        {
            AddingSoundForm addingSoundForm = new AddingSoundForm { Owner = this };
            addingSoundForm.ShowDialog();
            if (addingSoundForm.SoundIsReady)
                AddSoundToListBoxOfSounds(addingSoundForm.NewSound);
        }

        private void AddSoundToListBoxOfSounds(Sound sound)
        {
            collectionOfSounds.Add(sound);
            ListBoxFunctions.SortAscending(ListBoxOfSounds);
        }

        private void DeleteSoundButton_Click(object sender, RoutedEventArgs e)
        {
            collectionOfSounds.Remove(((sender as Button).TemplatedParent as ListBoxItem).Content as Sound);
        }
        #endregion


        #region Управление мелодиями
        private void AddMelodyButton_Click(object sender, RoutedEventArgs e)
        {
            AddingMelodyForm addingMelodyForm = new AddingMelodyForm { Owner = this };
            addingMelodyForm.ShowDialog();
            if (addingMelodyForm.MelodyIsReady)
                ListBoxOfMelodies.Items.Add(addingMelodyForm.NewMelody);
        }

        private void DeleteMelodyButton_Click(object sender, RoutedEventArgs e)
        {
            ListBoxOfMelodies.Items.Remove(((sender as Button).TemplatedParent as ListBoxItem).Content);
        }
        #endregion


        #region Управление SFX
        readonly List<MediaPlayer> listOfSfxMediaPlayers = new List<MediaPlayer>();

        private void SfxButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (Extensions.GetSound(button) == null)
                AddSoundToSfxButton(button);
            else
                PlaySfxSound(button);
        }

        private void AddSoundToSfxButton(Button button)
        {
            AttachSoundToButton(button);
            if (Extensions.GetSound(button) != null)
            {
                ChangeSfxIcon(button);
                ChangeSfxText(button);
            }
        }

        private void AttachSoundToButton(Button button)
        {
            AddingSoundForm addingSoundForm = new AddingSoundForm();
            addingSoundForm.ShowDialog();
            if (addingSoundForm.SoundIsReady)
                Extensions.SetSound(button, addingSoundForm.NewSound);
        }

        private void ChangeSfxIcon(Button button)
        {
            button.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/images/Note in circle.png")));
            button.Style = Resources["SfxButtonWithoutAnimation"] as Style;
            button.BeginAnimation(OpacityProperty, new DoubleAnimation(0, 1, new TimeSpan(0)));
        }

        private void ChangeSfxText(Button button)
        {
            button.Content = Extensions.GetSound(button).Name;
        }

        private void PlaySfxSound(object sender)
        {
            Sound sound = Extensions.GetSound(sender as Button);
            MediaPlayer mediaPlayer = new MediaPlayer();
            mediaPlayer.MediaEnded += RemoveSoundAfterPlay;
            mediaPlayer.Open(new Uri("sounds/" + sound.Name + ".mp3", UriKind.Relative));
            mediaPlayer.Volume = sound.Volume;
            listOfSfxMediaPlayers.Add(mediaPlayer);
            mediaPlayer.Play();
        }

        private void RemoveSoundAfterPlay(object sender, EventArgs e)
        {
            listOfSfxMediaPlayers.Remove(sender as MediaPlayer);
        }
        #endregion


        #region Управление шаблонами
        readonly List<MediaPlayer> mediaPlayers = new List<MediaPlayer>();
        readonly MediaPlayer melodyMediaPlayer = new MediaPlayer();

        private void AddTemplateButton_Click(object sender, RoutedEventArgs e)
        {
            TemplateCreationForm templateCreationForm = new TemplateCreationForm { Owner = this };
            templateCreationForm.ShowDialog();
            if (templateCreationForm.TemplateIsReady)
                AddTemplateToListBoxOfTemplates(templateCreationForm.TemplateName);
        }

        private void AddTemplateToListBoxOfTemplates(string templateName)
        {
            ListBoxOfTemplates.Items.Add(templateName);
            ListBoxFunctions.SortAscending(ListBoxOfTemplates);
        }

        private void ListBoxOfTemplates_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedTemplate = (string)ListBoxOfTemplates.SelectedItem;
            if (selectedTemplate != null)
            {
                StopPlayingTemplate();
                LoadSelectedTemplateSounds(selectedTemplate);
                LoadSelectedTemplateMelodies(selectedTemplate);
                PlaySelectedTemplate(selectedTemplate);
            }
        }

        private void StopPlayingTemplate()
        {
            for (int i = 0; i < mediaPlayers.Count; i++)
                mediaPlayers[i].Stop();
            mediaPlayers.Clear();
            melodyMediaPlayer.Stop();
        }

        private void LoadSelectedTemplateSounds(string selectedTemplate)
        {
            TemplateNameTextBox.Text = selectedTemplate;
            collectionOfSounds.Clear();
            FileWork.ReadFileToSoundCollection(collectionOfSounds, selectedTemplate);
            ListBoxFunctions.SortAscending(ListBoxOfSounds);
        }

        private void LoadSelectedTemplateMelodies(string selectedTemplate)
        {
            ListBoxOfMelodies.Items.Clear();
            FileWork.ReadFileToListBox(ListBoxOfMelodies, selectedTemplate);
        }

        private void PlaySelectedTemplate(string selectedTemplate)
        {
            for (int i = 0; i < collectionOfSounds.Count; i++)
                PlaySound(collectionOfSounds[i]);
        }

        void PlaySound(Sound sound)
        {
            MediaPlayer mediaPlayer = ConfiguredMediaPlayer(sound);
            mediaPlayer.Play();
            mediaPlayers.Add(mediaPlayer);
        }

        MediaPlayer ConfiguredMediaPlayer(Sound sound)
        {
            MediaPlayer mediaPlayer = new MediaPlayer();
            mediaPlayer.MediaEnded += MediaPlayerSoundEnded;
            mediaPlayer.Volume = sound.Volume;
            string soundPath = "sounds/" + sound.Name + ".mp3";
            mediaPlayer.Open(new Uri(soundPath, UriKind.Relative));

            return mediaPlayer;
        }

        async void MediaPlayerSoundEnded(object sender, EventArgs e)
        {
            MediaPlayer mediaPlayer = sender as MediaPlayer;
            await Task.Delay(GetDelay(mediaPlayer));
            ReplayMediaPlayer(mediaPlayer);
        }

        int GetDelay(MediaPlayer mediaPlayer)
        {
            int soundIndex = mediaPlayers.IndexOf(mediaPlayer);
            double repetitionRate = collectionOfSounds[soundIndex].RepetitionRate;

            return (int)(repetitionRate * 1000);
        }

        void ReplayMediaPlayer(MediaPlayer mediaPlayer)
        {
            if (mediaPlayers.Contains(mediaPlayer))
            {
                mediaPlayer.Position = new TimeSpan(0);
                mediaPlayer.Play();
            }
        }
        #endregion
    }
}