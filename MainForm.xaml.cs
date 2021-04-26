using System;
using System.IO;
using System.ComponentModel;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Spicy
{
    public partial class MainForm : Window
    {
        readonly ObservableCollection<Sound> collectionOfSounds;
        internal readonly List<MediaPlayer> players;
        readonly DoubleAnimation fadingAnimation;
        readonly DoubleAnimation appearanceAnimation;
        int musicVolume = 100;
        int ambientVolume = 100;
        int sfxVolume = 100;

        public MainForm()
        {
            InitializeComponent();
            InitializeOtherComponent();
            collectionOfSounds = new ObservableCollection<Sound>();
            players = new List<MediaPlayer>();
            fadingAnimation = new DoubleAnimation
            {
                From = 1,
                To = 0,
                Duration = new Duration(TimeSpan.FromMilliseconds(500))
            };
            appearanceAnimation = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = new Duration(TimeSpan.FromMilliseconds(500)),
            };
        }

        void InitializeOtherComponent()
        {
            MinimizeSoundSettings();
            MinimizeSoundVolumeAndRepetitionRate();
        }

        void MinimizeSoundSettings()
        {
            SoundSettings.Width = new GridLength(0, GridUnitType.Star);
            this.Width -= 287;
        }

        private void MinimizeSoundVolumeAndRepetitionRate()
        {
            SoundVolumeRow.Height = new GridLength(0, GridUnitType.Pixel);
            SoundRepetitionRateRow.Height = new GridLength(0, GridUnitType.Pixel);
        }

        void ListOfReadyMadeTemplates_Loaded(object sender, RoutedEventArgs e)
        {
            DirectoryInfo directory = new DirectoryInfo("bin/templates/");
            foreach (var fileName in directory.GetFiles("*.bin"))
                ListOfTemplates.Items.Add(fileName.Name.Replace(".bin", ""));
            ListOfTemplates.Items.SortDescriptions.Add(new SortDescription("", ListSortDirection.Ascending));
        }

        #region Sound control

        void MusicVolumeButton_Click(object sender, RoutedEventArgs e)
        {
            RemoveOrReturnSound(MusicVolumeSlider, ref musicVolume);
        }

        void RemoveOrReturnSound(Slider slider, ref int volume)
        {
            if (volume == 0)
            {
                volume = (int)slider.Value;
                slider.IsEnabled = true;
            }
            else
            {
                volume = 0;
                slider.IsEnabled = false;
            }
        }

        void MusicVolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            musicVolume = (int)(sender as Slider).Value;
        }

        void AmbientVolumeButton_Click(object sender, RoutedEventArgs e)
        {
            RemoveOrReturnSound(AmbientVolumeSlider, ref ambientVolume);
        }

        void AmbientVolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ambientVolume = (int)(sender as Slider).Value;
        }

        void SfxVolumeButton_Click(object sender, RoutedEventArgs e)
        {
            RemoveOrReturnSound(SfxVolumeSlider, ref sfxVolume);
        }

        void SfxVolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            sfxVolume = (int)(sender as Slider).Value;
        }

        #endregion

        #region Button animation

        void sfxButton_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            AnimationForButtonIconAndTextBlock(sender, appearanceAnimation);
        }

        void AnimationForButtonIconAndTextBlock(object sender, DoubleAnimation animation)
        {
            Button button = sender as Button;
            string buttonNumber = Regex.Match(button.Name, @"[0-9]{1,2}").ToString();
            Ellipse ellipse = button.Template.FindName("sfxButtonIcon" + buttonNumber, button) as Ellipse;
            TextBlock textBlock = sfxGrid.FindName("sfxTextBlock" + buttonNumber) as TextBlock;
            ellipse.BeginAnimation(OpacityProperty, animation);
            textBlock.BeginAnimation(OpacityProperty, animation);
        }

        void sfxButton_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            AnimationForButtonIconAndTextBlock(sender, fadingAnimation);
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
            AddIconToSfxButton(button, "sfxButtonIcon" + buttonNumber);
            AddTextToSfxTextBlock("sfxTextBlock" + buttonNumber, "SFX звук");
        }

        void AddIconToSfxButton(Button button, string buttonIconName)
        {
            Ellipse ellipse = button.Template.FindName(buttonIconName, button) as Ellipse;
            ellipse.Fill = new ImageBrush(new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "images/Sound icon.png")));
        }

        void AddTextToSfxTextBlock(string textBlockName, string text)
        {
            TextBlock textBlock = sfxGrid.FindName(textBlockName) as TextBlock;
            textBlock.Text = text;
        }

        #endregion

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
            StopSounds();
            MinimizeSoundSettings();
            File.Delete("bin/templates/" + ListOfTemplates.SelectedItem.ToString() + ".bin");
            ListOfTemplates.Items.Remove(ListOfTemplates.SelectedItem);
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
            int soundIndex = collectionOfSounds.IndexOf(ListOfSounds.SelectedItem as Sound);
            players[soundIndex].Stop();
            players.RemoveAt(soundIndex);
            collectionOfSounds.Remove(ListOfSounds.SelectedItem as Sound);
            RewriteTemplateFile();
        }

        #region List of templates selection

        void ListOfTemplates_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListOfTemplates.SelectedItem != null)
            {
                DeleteTemplateButton.IsEnabled = true;
                ExpandSoundSettings();
                ChangeTemplateNameTextBlock();
                LoadTemplateSoundsInCollection();
                AddSoundsInListAndSort();
                PlayNewSounds();
            }
            else
                DeleteTemplateButton.IsEnabled = false;

        }

        void ExpandSoundSettings()
        {
            if (SoundSettings.Width != new GridLength(1, GridUnitType.Star))
            {
                SoundSettings.Width = new GridLength(1, GridUnitType.Star);
                this.Width += 287;
                if (SystemParameters.PrimaryScreenWidth - this.Left < this.Width)
                    this.Left = SystemParameters.PrimaryScreenWidth - this.Width;
            }
        }

        void ChangeTemplateNameTextBlock()
        {
            TemplateName.Text = ListOfTemplates.SelectedItem.ToString();
        }

        void LoadTemplateSoundsInCollection()
        {
            collectionOfSounds.Clear();
            using (BinaryReader reader = new BinaryReader(File.Open("bin/templates/" + TemplateName.Text + ".bin", FileMode.Open)))
                while (reader.PeekChar() != -1)
                {
                    string[] soundNameAndSettings = reader.ReadString().Split(new[] { ".mp3" }, StringSplitOptions.None);
                    string[] soundSettings = soundNameAndSettings[1].Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    string soundName = soundNameAndSettings[0];
                    double soundVolume = double.Parse(soundSettings[0], System.Globalization.CultureInfo.InvariantCulture);
                    int soundRepetitionRate = Convert.ToInt32(soundSettings[1]);
                    collectionOfSounds.Add(new Sound(soundName, soundVolume, soundRepetitionRate));
                }
        }

        void AddSoundsInListAndSort()
        {
            ListOfSounds.ItemsSource = collectionOfSounds;
            ListOfSounds.DisplayMemberPath = "Name";
            ListOfSounds.Items.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
        }

        void PlayNewSounds()
        {
            StopSounds();
            for (int i = 0; i < collectionOfSounds.Count; i++)
            {
                players.Add(new MediaPlayer());
                players[i].MediaEnded += MediaPlayerSoundEnded;
                players[i].Volume = collectionOfSounds[i].Volume;
                string soundPath = "sounds/" + collectionOfSounds[i].Name + ".mp3";
                players[i].Open(new Uri(AppDomain.CurrentDomain.BaseDirectory + soundPath));
                players[i].Play();
            }
        }

        void StopSounds()
        {
            if (players.Count > 0)
                for (int i = 0; i < players.Count; i++)
                    players[i].Stop();
            players.Clear();
        }

        internal async void MediaPlayerSoundEnded(object sender, EventArgs e)
        {
            MediaPlayer player = sender as MediaPlayer;
            int soundIndex = players.IndexOf(player);
            int delay = collectionOfSounds[soundIndex].RepetitionRate;
            await Task.Delay(delay * 1000);
            if (players.Contains(player))
            {
                player.Position = new TimeSpan(0);
                player.Play();
            }
        }

        #endregion

        #region List of sounds selection

        void ListOfSounds_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListOfTemplates.SelectedItem != null && ListOfSounds.SelectedItem != null)
            {
                ExpandSoundVolumeAndRepetitionRate();
                DeleteSoundButton.IsEnabled = true;

                SoundVolumeSlider.Value = (ListOfSounds.SelectedItem as Sound).Volume;
                SoundRepetitionRateTextbox.Text = (ListOfSounds.SelectedItem as Sound).RepetitionRate.ToString();
            }
            else
            {
                MinimizeSoundVolumeAndRepetitionRate();
                DeleteSoundButton.IsEnabled = false;
            }
        }

        private void ExpandSoundVolumeAndRepetitionRate()
        {
            SoundVolumeRow.Height = new GridLength(60, GridUnitType.Pixel);
            SoundRepetitionRateRow.Height = new GridLength(60, GridUnitType.Pixel);
        }

        void SoundVolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (ListOfSounds.SelectedItem != null)
            {
                (ListOfSounds.SelectedItem as Sound).Volume = SoundVolumeSlider.Value;
                ChangeSoundVolume();
                RewriteTemplateFile();
            }
        }

        void ChangeSoundVolume()
        {
            int soundIndex = collectionOfSounds.IndexOf(ListOfSounds.SelectedItem as Sound);
            players[soundIndex].Volume = (ListOfSounds.SelectedItem as Sound).Volume;
        }

        internal void RewriteTemplateFile()
        {
            using (BinaryWriter writer = new BinaryWriter(File.Open("bin/templates/" + TemplateName.Text + ".bin", FileMode.Create)))
                foreach (var sound in collectionOfSounds)
                {
                    string soundFileName = sound.Name + ".mp3";
                    string soundVolume = sound.Volume.ToString();
                    string soundRepetitionRate = sound.RepetitionRate.ToString();
                    writer.Write(soundFileName + " " + soundVolume + " " + soundRepetitionRate);
                }
        }

        void SoundRepetitionRateTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ListOfSounds.SelectedItem != null && SoundRepetitionRateTextbox.Text.Length != 0)
            {
                (ListOfSounds.SelectedItem as Sound).RepetitionRate = Convert.ToInt32(SoundRepetitionRateTextbox.Text);
                RewriteTemplateFile();
            }
        }

        #endregion

        internal void AddSoundInCollectionOfSoundsAndSort(Sound sound)
        {
            collectionOfSounds.Add(sound);
            ListOfSounds.Items.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
        }
    }
}