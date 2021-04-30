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
using System.Text.RegularExpressions;

namespace Spicy
{
    public partial class MainForm : Window
    {
        #region Initialization
        public MainForm()
        {
            InitializeComponent();
            InitializeOtherComponent();
            volumeSliders = new[] { SfxVolumeSlider, MusicVolumeSlider, AmbientVolumeSlider };
            volumeButtons = new[] { SfxVolumeButton, MusicVolumeButton, AmbientVolumeButton };
        }

        void InitializeOtherComponent()
        {
            InitializeListBoxOfTemplateSounds();
            MinimizeSoundListAndSettings();
            MinimizeSoundSettings();
        }

        void InitializeListBoxOfTemplateSounds()
        {
            ListBoxOfTemplateSounds.ItemsSource = collectionOfTemplateSounds;
            ListBoxOfTemplateSounds.DisplayMemberPath = "Name";
        }

        void MinimizeSoundListAndSettings()
        {
            SoundListAndSettings.Width = new GridLength(0, GridUnitType.Star);
            Width -= Width / 4;
        }

        void MinimizeSoundSettings()
        {
            SoundVolumeRow.Height = new GridLength(0, GridUnitType.Pixel);
            SoundRepetitionRateRow.Height = new GridLength(0, GridUnitType.Pixel);
        }

        void ListOfReadyMadeTemplates_Loaded(object sender, RoutedEventArgs e)
        {
            ListBoxFunctions.LoadFileNamesFromFolderToList(ListBoxOfTemplates, "sound templates", "bin");
            ListBoxFunctions.SortAscending(ListBoxOfTemplates);
        }
        #endregion


        #region General sound control
        readonly Slider[] volumeSliders;
        readonly Button[] volumeButtons;
        readonly double[] volume = new[] { 1.0, 1.0, 1.0 };
        readonly double[] pastVolume = new[] { 1.0, 1.0, 1.0 };
        readonly bool[] volumeIsMute = new[] { false, false, false };
        bool soundRepetitionRateTextBoxGotFocus = false;

        void VolumeButton_Click(object sender, RoutedEventArgs e)
        {
            MuteOrUnmuteGeneralSound(sender);
        }

        void MuteOrUnmuteGeneralSound(object sender)
        {
            int volumeIndex = Array.IndexOf(volumeButtons, sender as Button);
            volumeIsMute[volumeIndex] = !volumeIsMute[volumeIndex];
            if (volumeIsMute[volumeIndex])
                volumeSliders[volumeIndex].Value = 0;
            else
                volumeSliders[volumeIndex].Value = pastVolume[volumeIndex];
        }

        void SfxVolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (volumeSliders != null)
                ChangeGeneralVolume(sender);
        }

        void ChangeGeneralVolume(object sender)
        {
            int volumeIndex = Array.IndexOf(volumeSliders, sender as Slider);
            volume[volumeIndex] = volumeSliders[volumeIndex].Value;
            if (volume[volumeIndex] != 0)
            {
                pastVolume[volumeIndex] = volume[volumeIndex];
                volumeIsMute[volumeIndex] = false;
            }
        }

        void MusicVolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (volumeSliders != null)
                ChangeGeneralVolume(sender);
            ApplyMusicVolumeToMelodyMediaPlayer();
        }

        void ApplyMusicVolumeToMelodyMediaPlayer()
        {
            melodyMediaPlayer.Volume = volume[1];
        }

        void AmbientVolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (volumeSliders != null)
                ChangeGeneralVolume(sender);
            ApplyAmbientVolumeToTemplateSounds();
        }

        void ApplyAmbientVolumeToTemplateSounds()
        {
            for (int i = 0; i < collectionOfTemplateSounds.Count; i++)
                mediaPlayers[i].Volume = collectionOfTemplateSounds[i].Volume * volume[2];
        }
        #endregion


        #region Button animation
        readonly DoubleAnimation fadingAnimation = new DoubleAnimation
        {
            From = 1,
            To = 0,
            Duration = new Duration(TimeSpan.FromMilliseconds(500))
        };
        readonly DoubleAnimation appearanceAnimation = new DoubleAnimation
        {
            From = 0,
            To = 1,
            Duration = new Duration(TimeSpan.FromMilliseconds(500)),
        };

        void SfxButton_MouseEnter(object sender, MouseEventArgs e)
        {
            AnimateButtonIconAndTextBlock(sender, appearanceAnimation);
        }

        void SfxButton_MouseLeave(object sender, MouseEventArgs e)
        {
            AnimateButtonIconAndTextBlock(sender, fadingAnimation);
        }

        void AnimateButtonIconAndTextBlock(object sender, DoubleAnimation animation)
        {
            Button button = sender as Button;
            string buttonNumber = Regex.Match(button.Name, @"[0-9]{1,2}").ToString();
            AnimateButtonIcon(animation, button, buttonNumber);
            AnimateButtonTextBlock(animation, buttonNumber);
        }

        void AnimateButtonIcon(DoubleAnimation animation, Button button, string buttonNumber)
        {
            Ellipse ellipse = button.Template.FindName("SfxButtonIcon" + buttonNumber, button) as Ellipse;
            ellipse.BeginAnimation(OpacityProperty, animation);
        }

        void AnimateButtonTextBlock(DoubleAnimation animation, string buttonNumber)
        {
            TextBlock textBlock = sfxGrid.FindName("SfxTextBlock" + buttonNumber) as TextBlock;
            textBlock.BeginAnimation(OpacityProperty, animation);
        }
        #endregion


        #region SFX button click
        void SfxButton_Click(object sender, RoutedEventArgs e)
        {
            AddSfxToButton(sender);
        }

        void AddSfxToButton(object sender)
        {
            Button button = sender as Button;
            string buttonNumber = Regex.Match(button.Name, @"[0-9]{1,2}").ToString();
            AddIconToSfxButton(button, "SfxButtonIcon" + buttonNumber);
            AddTextToSfxTextBlock("SfxTextBlock" + buttonNumber, "SFX звук");
        }

        void AddIconToSfxButton(Button button, string buttonIconName)
        {
            Ellipse ellipse = button.Template.FindName(buttonIconName, button) as Ellipse;
            ellipse.Fill = new ImageBrush(new BitmapImage(new Uri("images/Sound icon.png", UriKind.Relative)));
        }

        void AddTextToSfxTextBlock(string textBlockName, string text)
        {
            TextBlock textBlock = sfxGrid.FindName(textBlockName) as TextBlock;
            textBlock.Text = text;
        }
        #endregion


        #region Control components of templates
        readonly List<MediaPlayer> mediaPlayers = new List<MediaPlayer>();

        void ListOfTemplates_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListBoxOfTemplates.SelectedItem != null)
            {
                DeleteTemplateButton.IsEnabled = true;
                ExpandSoundListAndSettings();
                LoadAndPlaySelectedTemplate();
            }
            else
            {
                DeleteTemplateButton.IsEnabled = false;
                TemplateName.Text = string.Empty;
            }
        }

        void ExpandSoundListAndSettings()
        {
            GridLength oneStarLength = new GridLength(1, GridUnitType.Star);
            if (SoundListAndSettings.Width != oneStarLength)
            {
                Width += Width / 3;
                if (SystemParameters.PrimaryScreenWidth - Left < Width)
                    Left = SystemParameters.PrimaryScreenWidth - Width;
            }
            SoundListAndSettings.Width = oneStarLength;
        }

        void LoadAndPlaySelectedTemplate()
        {
            ChangeTextInTemplateName();
            LoadNewTemplateSoundsInCollection();
            StopCurrentTemplateSounds();
            PlaySoundsInSelectedTemplate();
        }

        void ChangeTextInTemplateName()
        {
            TemplateName.Text = ListBoxOfTemplates.SelectedItem.ToString();
        }

        void LoadNewTemplateSoundsInCollection()
        {
            collectionOfTemplateSounds.Clear();
            FileWork.ReadFileToSoundCollection(collectionOfTemplateSounds, TemplateName.Text);
            ListBoxFunctions.SortAscending(ListBoxOfTemplateSounds, "Name");
        }

        void StopCurrentTemplateSounds()
        {
            for (int i = 0; i < mediaPlayers.Count; i++)
                mediaPlayers[i].Stop();
            mediaPlayers.Clear();
        }

        void PlaySoundsInSelectedTemplate()
        {
            for (int i = 0; i < collectionOfTemplateSounds.Count; i++)
                PlaySound(collectionOfTemplateSounds[i]);
        }

        internal void PlaySound(Sound sound)
        {
            MediaPlayer mediaPlayer = ConfiguredMediaPlayer(sound);
            mediaPlayer.Play();
            mediaPlayers.Add(mediaPlayer);
        }

        MediaPlayer ConfiguredMediaPlayer(Sound sound)
        {
            MediaPlayer mediaPlayer = new MediaPlayer();
            mediaPlayer.MediaEnded += MediaPlayerSoundEnded;
            mediaPlayer.Volume = 2 * sound.Volume * volume[2];
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
            int repetitionRate = collectionOfTemplateSounds[soundIndex].RepetitionRate;

            return repetitionRate * 1000;
        }

        void ReplayMediaPlayer(MediaPlayer mediaPlayer)
        {
            if (mediaPlayers.Contains(mediaPlayer))
            {
                mediaPlayer.Position = new TimeSpan(0);
                mediaPlayer.Play();
            }
        }

        void CreateTemplateButton_Click(object sender, RoutedEventArgs e)
        {
            TemplateCreationForm templateCreationForm = new TemplateCreationForm
            {
                Owner = this
            };
            templateCreationForm.Show();
        }

        void DeleteTemplateButton_Click(object sender, RoutedEventArgs e)
        {
            StopCurrentTemplateSounds();
            MinimizeSoundListAndSettings();
            DeleteTemplate((string)ListBoxOfTemplates.SelectedItem);
        }

        void DeleteTemplate(string name)
        {
            File.Delete("sound templates/" + name + ".bin");
            ListBoxOfTemplates.Items.Remove(name);
        }
        #endregion


        #region Control components of template sounds
        readonly ObservableCollection<Sound> collectionOfTemplateSounds = new ObservableCollection<Sound>();

        void ListOfSounds_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListBoxOfTemplateSounds.SelectedItem != null)
            {
                ExpandSoundSettings();
                SetSoundSettings();
                DeleteSoundButton.IsEnabled = true;
            }
            else
            {
                MinimizeSoundSettings();
                DeleteSoundButton.IsEnabled = false;
            }
        }

        void ExpandSoundSettings()
        {
            SoundVolumeRow.Height = new GridLength(60, GridUnitType.Pixel);
            SoundRepetitionRateRow.Height = new GridLength(60, GridUnitType.Pixel);
        }

        void SetSoundSettings()
        {
            Sound selectedSound = ListBoxOfTemplateSounds.SelectedItem as Sound;
            SoundVolumeSlider.Value = selectedSound.Volume;
            SoundRepetitionRateTextbox.Text = selectedSound.RepetitionRate.ToString();
        }

        void SoundRepetitionRateTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ListBoxOfTemplateSounds.SelectedItem != null && SoundRepetitionRateTextbox.Text.Length != 0)
            {
                (ListBoxOfTemplateSounds.SelectedItem as Sound).RepetitionRate = Convert.ToInt32(SoundRepetitionRateTextbox.Text);
                RewriteTemplateFile();
            }
        }

        internal void RewriteTemplateFile()
        {
            FileWork.WriteSoundCollectionToFile(collectionOfTemplateSounds, TemplateName.Text);
        }

        void SoundVolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (ListBoxOfTemplateSounds.SelectedItem != null)
            {
                (ListBoxOfTemplateSounds.SelectedItem as Sound).Volume = SoundVolumeSlider.Value;
                ChangeSoundVolume();
                RewriteTemplateFile();
            }
        }

        void ChangeSoundVolume()
        {
            int soundIndex = collectionOfTemplateSounds.IndexOf(ListBoxOfTemplateSounds.SelectedItem as Sound);
            mediaPlayers[soundIndex].Volume = (ListBoxOfTemplateSounds.SelectedItem as Sound).Volume * volume[2];
        }

        void SoundRepetitionRateTextbox_GotFocus(object sender, RoutedEventArgs e)
        {
            soundRepetitionRateTextBoxGotFocus = true;
        }

        void SoundRepetitionRateTextbox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (soundRepetitionRateTextBoxGotFocus)
            {
                soundRepetitionRateTextBoxGotFocus = false;
                (sender as TextBox).SelectAll();
            }
        }

        void AddSoundButton_Click(object sender, RoutedEventArgs e)
        {
            AddingSoundForm addingSoundForm = new AddingSoundForm
            {
                Owner = this
            };
            addingSoundForm.Show();
        }

        void DeleteSoundButton_Click(object sender, RoutedEventArgs e)
        {
            Sound selectedSound = ListBoxOfTemplateSounds.SelectedItem as Sound;
            StopDeletableSound(selectedSound);
            collectionOfTemplateSounds.Remove(selectedSound);
            RewriteTemplateFile();
        }

        void StopDeletableSound(Sound sound)
        {
            int soundIndex = collectionOfTemplateSounds.IndexOf(ListBoxOfTemplateSounds.SelectedItem as Sound);
            mediaPlayers[soundIndex].Stop();
            mediaPlayers.RemoveAt(soundIndex);
        }
        #endregion


        #region Control components of melodies
        readonly MediaPlayer melodyMediaPlayer = new MediaPlayer();

        void AddMelodyButton_Click(object sender, RoutedEventArgs e)
        {
            AddingMelodyForm addingMelodyForm = new AddingMelodyForm
            {
                Owner = this
            };
            addingMelodyForm.Show();
        }

        void DeleteMelodyButton_Click(object sender, RoutedEventArgs e)
        {
            string selectedMelody = (string)ListBoxOfMelodies.SelectedItem;
            StopIfItPlayingMelody(selectedMelody);
            ListBoxOfMelodies.Items.Remove(selectedMelody);
            RewriteMelodiesFile();
        }

        void StopIfItPlayingMelody(string melody)
        {
            if (melodyMediaPlayer.Source != null)
            {
                string playingMelody = System.IO.Path.GetFileNameWithoutExtension(melodyMediaPlayer.Source.OriginalString);
                if (melody == playingMelody)
                {
                    melodyMediaPlayer.Stop();
                    PlayNextMelody(GetNextMelodyIndex());
                }
            }
        }

        int GetNextMelodyIndex()
        {
            string playingMelody = System.IO.Path.GetFileNameWithoutExtension(melodyMediaPlayer.Source.OriginalString);
            return ListBoxOfMelodies.Items.IndexOf(playingMelody) + 1;
        }

        void PlayNextMelody(int index)
        {
            if (ListBoxOfMelodies.Items.Count > 0)
            {
                if (index == ListBoxOfMelodies.Items.Count)
                    melodyMediaPlayer.Open(new Uri("music/" + ListBoxOfMelodies.Items[0] + ".mp3", UriKind.Relative));
                else
                    melodyMediaPlayer.Open(new Uri("music/" + ListBoxOfMelodies.Items[index] + ".mp3", UriKind.Relative));
                melodyMediaPlayer.Play();
            }
        }

        internal void RewriteMelodiesFile()
        {
            if (TemplateName.Text.Length > 0)
                FileWork.WriteMelodiesToFile(ListBoxOfMelodies.Items, TemplateName.Text);
        }

        void ListBoxOfMelodies_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                melodyMediaPlayer.MediaEnded += MediaPlayerMelodyEnded;
                melodyMediaPlayer.Volume = volume[1];
                PlayNextMelody(0);
            }
        }

        void MediaPlayerMelodyEnded(object sender, EventArgs e)
        {
            PlayNextMelody(GetNextMelodyIndex());
        }
        #endregion
    }
}