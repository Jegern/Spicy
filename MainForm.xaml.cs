using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Spicy
{
    public partial class MainForm : Window
    {
        #region Инициализация
        public MainForm()
        {
            InitializeComponent();
            InitializeOtherComponent();
            volumeSliders = new[] { MasterVolumeSlider, MusicVolumeSlider, SoundVolumeSlider, SfxVolumeSlider };
            volumeButtons = new[] { MasterVolumeButton, MusicVolumeButton, SoundVolumeButton, SfxVolumeButton };
        }

        private void InitializeOtherComponent()
        {
            InitializeAppWidthAndHeight();
            HideTemplateAndSfxMenu();
            InitializeListBoxOfTemplateSounds();
            LoadTemplatesInListBox();
            InitializeMelodyMediaPlayer();
        }

        private void InitializeAppWidthAndHeight()
        {
            Width = SystemParameters.PrimaryScreenWidth * 0.9;
            Height = SystemParameters.PrimaryScreenHeight;
        }

        private void HideTemplateAndSfxMenu()
        {
            MinimizeMenu(TemplateMenu);
            MinimizeMenu(SfxMenu);
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

        private void InitializeMelodyMediaPlayer()
        {
            musicMediaPlayer.Volume = volume[1];
            musicMediaPlayer.MediaEnded += MediaPlayerMelodyEnded;
        }

        void MediaPlayerMelodyEnded(object sender, EventArgs e)
        {
            string melodyName = Path.GetFileNameWithoutExtension(musicMediaPlayer.Source.OriginalString);
            PlayNextMelody(melodyName);
        }

        private void PlayNextMelody(string currentMelodyName)
        {
            int currentMelodyIndex = ListBoxOfMelodies.Items.IndexOf(currentMelodyName);
            int nextMelodyIndex = currentMelodyIndex + 1 == ListBoxOfMelodies.Items.Count ? 0 : currentMelodyIndex + 1;
            PlayMelody(ListBoxOfMelodies.Items[nextMelodyIndex].ToString());
        }
        #endregion


        #region Управление меню

        private void TemplateMenu_Click(object sender, RoutedEventArgs e)
        {
            ExpandOrMinimizeMenu(TemplateMenu, SfxMenu);
        }

        private void SfxMenu_Click(object sender, RoutedEventArgs e)
        {
            ExpandOrMinimizeMenu(SfxMenu, TemplateMenu);
        }

        private void ExpandOrMinimizeMenu(ColumnDefinition mainMenu, ColumnDefinition secondaryMenu)
        {
            if ((bool)mainMenu.Tag)
            {
                MinimizeMenu(mainMenu);
                ReduceAppWidth();
            }
            else if ((bool)secondaryMenu.Tag)
            {
                MinimizeMenu(secondaryMenu);
                ExpandMenu(mainMenu);
            }
            else
            {
                IncreaseAppWidth();
                ExpandMenu(mainMenu);
            }
        }

        private void ReduceAppWidth()
        {
            MinWidth = 500;
            Width -= Width / 3 - 5;
        }

        private void IncreaseAppWidth()
        {
            Width += Width / 2 - 7.5;
            MinWidth = 742.5;
            if (SystemParameters.PrimaryScreenWidth - Left - Width < 0)
                Left = SystemParameters.PrimaryScreenWidth - Width;
        }

        private void MinimizeMenu(ColumnDefinition menu)
        {
            menu.Width = new GridLength(0);
            menu.Tag = false;
        }

        private void ExpandMenu(ColumnDefinition menu)
        {
            menu.Width = new GridLength(1, GridUnitType.Star);
            menu.Tag = true;
        }
        #endregion


        #region Управление звуками
        readonly ObservableCollection<MediaPlayerWithSound> collectionOfSounds = new ObservableCollection<MediaPlayerWithSound>();
        bool soundTemplateChanged = false;

        private void SoundTemplateHasBeenChanged()
        {
            soundTemplateChanged = true;
            SaveButton.Visibility = Visibility.Visible;
        }

        private void SoundTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            MediaPlayerWithSound sound = (sender as TextBox).DataContext as MediaPlayerWithSound;
            sound.RepetitionRate = Convert.ToDouble((sender as TextBox).Text);
            SoundTemplateHasBeenChanged();
        }

        private void SoundSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            MediaPlayerWithSound sound = (sender as Slider).DataContext as MediaPlayerWithSound;
            sound.SoundVolume = (sender as Slider).Value;
            sound.Volume = sound.SoundVolume * volume[2];
            SoundTemplateHasBeenChanged();
        }

        private void AddSoundButton_Click(object sender, RoutedEventArgs e)
        {
            AddingSoundForm addingSoundForm = new AddingSoundForm { Owner = this };
            addingSoundForm.ShowDialog();
            if (addingSoundForm.SoundIsReady)
                AddSoundToListBoxOfSounds(addingSoundForm.NewSound);
            SoundTemplateHasBeenChanged();
        }

        private void AddSoundToListBoxOfSounds(MediaPlayerWithSound sound)
        {
            collectionOfSounds.Add(sound);
            ListBoxFunctions.SortAscending(ListBoxOfSounds);
            ConfiguredMediaPlayer(sound);
            sound.Play();
        }

        private void DeleteSoundButton_Click(object sender, RoutedEventArgs e)
        {
            MediaPlayerWithSound sound = ((sender as Button).TemplatedParent as ListBoxItem).Content as MediaPlayerWithSound;
            sound.Stop();
            collectionOfSounds.Remove(sound);
            SoundTemplateHasBeenChanged();
        }
        #endregion


        #region Управление мелодиями
        readonly MediaPlayer musicMediaPlayer = new MediaPlayer();
        string playingMelodyName = string.Empty;
        bool melodyIsPaused = false;
        bool melodyTemplateChanged = false;

        private void MelodyTemplateHasBeenChanged()
        {
            melodyTemplateChanged = true;
            SaveButton.Visibility = Visibility.Visible;
        }

        private void AddMelodyButton_Click(object sender, RoutedEventArgs e)
        {
            AddingMelodyForm melodyForm = new AddingMelodyForm { Owner = this };
            melodyForm.ShowDialog();
            if (melodyForm.MelodyIsReady)
                ListBoxOfMelodies.Items.Add(melodyForm.NewMelody);
            MelodyTemplateHasBeenChanged();
        }

        private void DeleteMelodyButton_Click(object sender, RoutedEventArgs e)
        {
            string deletingMelodyName = (sender as Button).DataContext.ToString();
            if (deletingMelodyName == playingMelodyName)
                StopMelody();
            ListBoxOfMelodies.Items.Remove(deletingMelodyName);
            MelodyTemplateHasBeenChanged();
        }

        private void StopMelody()
        {
            musicMediaPlayer.Stop();
            playingMelodyName = string.Empty;
            MelodyNameLabel.Content = string.Empty;
            PlayPauseMelodyButton.Background = Resources["Play"] as ImageBrush;
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
            ChangeMelodyButtonIcons(button, "Play");
            ExpandMelodySlider(button);
            MelodyNameLabel.Content = playingMelodyName = melodyName;
        }

        private void ClearMelody()
        {
            for (int i = 0; i < ListBoxOfMelodies.Items.Count; i++)
            {
                ListBoxItem item = ListBoxOfMelodies.ItemContainerGenerator.ContainerFromIndex(i) as ListBoxItem;
                Button button = item.Template.FindName("PlayMelodyButton", item) as Button;
                button.Background = Resources["Play"] as ImageBrush;
                Grid grid = (button.Parent as Grid).Parent as Grid;
                grid.RowDefinitions[1].Height = new GridLength(0);
            }
        }

        private void PlayMelody(string name)
        {
            musicMediaPlayer.Open(new Uri("music/" + name + ".mp3", UriKind.Relative));
            musicMediaPlayer.Play();
            melodyIsPaused = false;
        }

        private void ChangeMelodyButtonIcons(Button button, string action)
        {
            string iconName = action == "Play" ? "Pause" : "Play";
            button.Background = Resources[iconName] as ImageBrush;
            PlayPauseMelodyButton.Background = Resources[iconName] as ImageBrush;
        }

        private void ExpandMelodySlider(Button button)
        {
            Grid listBoxItemGrid = (button.Parent as Grid).Parent as Grid;
            listBoxItemGrid.RowDefinitions[1].Height = new GridLength(30);
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
            musicMediaPlayer.Play();
            melodyIsPaused = false;
            ChangeMelodyButtonIcons(button, "Play");
        }

        private void PauseMelody(Button button)
        {
            musicMediaPlayer.Pause();
            melodyIsPaused = true;
            ChangeMelodyButtonIcons(button, "Pause");
        }

        private Button FindMelodyButton(string melodyName)
        {
            ListBoxOfMelodies.UpdateLayout();
            Button button = null;
            for (int i = 0; i < ListBoxOfMelodies.Items.Count; i++)
            {
                ListBoxItem item = ListBoxOfMelodies.ItemContainerGenerator.ContainerFromIndex(i) as ListBoxItem;
                button = item.Template.FindName("PlayMelodyButton", item) as Button;
                string buttonMelodyName = (button.Parent as Grid).DataContext.ToString();
                if (buttonMelodyName == melodyName)
                    break;
            }
            return button;
        }

        private void UpMelodyButton_Click(object sender, RoutedEventArgs e)
        {
            string melodyName = (sender as Button).DataContext.ToString();
            int melodyIndex = ListBoxOfMelodies.Items.IndexOf(melodyName);
            if (melodyIndex != 0)
            {
                ListBoxOfMelodies.Items.Insert(melodyIndex - 1, melodyName);
                ListBoxOfMelodies.Items.RemoveAt(melodyIndex + 1);
                UpdateMelodyLayout(melodyName);
                MelodyTemplateHasBeenChanged();
            }
        }

        private void UpdateMelodyLayout(string melodyName)
        {
            if (melodyName == playingMelodyName)
            {
                FindMelodyButton(playingMelodyName).Background = PlayPauseMelodyButton.Background;
                ExpandMelodySlider(FindMelodyButton(playingMelodyName));
            }
        }

        private void PlayPauseMelodyButton_Click(object sender, RoutedEventArgs e)
        {
            if (playingMelodyName != string.Empty)
                ContinuePlayOrPauseMelody(FindMelodyButton(playingMelodyName));
        }

        private void RewindMelodyButton_Click(object sender, RoutedEventArgs e)
        {
            StartMelodyWithShift(-1);
        }

        private void ForwardMelodyButton_Click(object sender, RoutedEventArgs e)
        {
            StartMelodyWithShift(+1);
        }

        private void StartMelodyWithShift(int shift)
        {
            if (playingMelodyName != string.Empty)
            {
                int melodyIndex = ListBoxOfMelodies.Items.IndexOf(playingMelodyName);
                if (0 <= melodyIndex + shift && melodyIndex + shift < ListBoxOfMelodies.Items.Count)
                {
                    string melodyName = ListBoxOfMelodies.Items[melodyIndex + shift].ToString();
                    StartMelody(FindMelodyButton(melodyName));
                }
            }
        }
        #endregion


        #region Управление SFX
        bool sfxTemplateChanged = false;

        private void SfxTemplateHasBeenChanged()
        {
            sfxTemplateChanged = true;
            SaveButton.Visibility = Visibility.Visible;
        }

        private void SfxButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (Extensions.GetSound(button) == null)
                AddSoundToSfxButton(button);
            else
                PlayOrStopSfxSound(button);
        }

        private void AddSoundToSfxButton(Button button)
        {
            AttachSoundToButton(button);
            ChangeSfxLayout(button);
        }

        internal void ChangeSfxLayout(Button button)
        {
            if (Extensions.GetSound(button) != null)
            {
                ChangeSfxIcon(button);
                ChangeSfxText(button);
                ConfigureCrossButton(button);
            }
        }

        private void AttachSoundToButton(Button button)
        {
            AddingSoundForm addingSoundForm = new AddingSoundForm { Owner = this };
            addingSoundForm.ShowDialog();
            if (addingSoundForm.SoundIsReady)
                Extensions.SetSound(button, addingSoundForm.NewSound);
            SfxTemplateHasBeenChanged();
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

        private void ConfigureCrossButton(Button button)
        {
            button.UpdateLayout();
            Button crossButton = button.Template.FindName("SfxCrossButton", button) as Button;
            crossButton.Click += SfxCrossButton_Click;
        }

        private void SfxCrossButton_Click(object sender, RoutedEventArgs e)
        {
            Button parentButton = (sender as Button).TemplatedParent as Button;
            parentButton.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/images/Plus in circle.png")));
            parentButton.Style = Resources["SfxButtonWithAnimation"] as Style;
            parentButton.BeginAnimation(OpacityProperty, new DoubleAnimation(1, 0, new TimeSpan(0)));
            parentButton.Content = string.Empty;
            Extensions.ClearSound(parentButton);
            SfxTemplateHasBeenChanged();
            e.Handled = true;
        }

        private void PlayOrStopSfxSound(Button button)
        {
            MediaPlayerWithSound sound = Extensions.GetSound(button);
            if (sound.IsPlaying)
                StopSfxSound(sound);
            else
                PlaySfxSound(sound);
            
        }

        private void StopSfxSound(MediaPlayerWithSound sound)
        {
            sound.Stop();
            sound.IsPlaying = false;
        }

        private void PlaySfxSound(MediaPlayerWithSound sound)
        {
            sound.Open(new Uri("sounds/" + sound.Name + ".mp3", UriKind.Relative));
            //Добавить инициализацию уровня громкости звука
            sound.Play();
            sound.IsPlaying = true;
        }
        #endregion


        #region Управление шаблонами
        string selectedTemplate = string.Empty;
        bool templateIsReady = false;

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(selectedTemplate))
                RewriteTemplate();
            else
                WriteNewTemplate();
        }

        private void RewriteTemplate()
        {
            if (soundTemplateChanged)
                RewriteSoundFile();
            if (melodyTemplateChanged)
                RewriteMelodyFile();
            if (sfxTemplateChanged)
                RewriteSfxFile();
            SaveButton.Visibility = Visibility.Hidden;
        }

        private void RewriteSoundFile()
        {
            FileWork.WriteSoundCollectionToFile(collectionOfSounds, TemplateNameTextBox.Text);
            soundTemplateChanged = false;
        }

        private void RewriteMelodyFile()
        {
            FileWork.WriteMelodiesToFile(ListBoxOfMelodies.Items, TemplateNameTextBox.Text);
            melodyTemplateChanged = false;
        }

        private void RewriteSfxFile()
        {
            FileWork.WriteSfxGridToFile(SfxGrid, TemplateNameTextBox.Text);
            sfxTemplateChanged = false;
        }

        private void WriteNewTemplate()
        {
            CheckIfTemplateIsReady();
            if (templateIsReady)
            {
                RewriteSoundFile();
                RewriteMelodyFile();
                ListBoxOfTemplates.Items.Add(TemplateNameTextBox.Text);
                SaveButton.Visibility = Visibility.Hidden;
            }
        }

        void CheckIfTemplateIsReady()
        {
            if (TemplateNameTextBox.Text.Length == 0)
                MessageBox.Show("Пожалуйста, введите имя шаблона", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            else if (ListBoxOfTemplates.Items.Contains(TemplateNameTextBox.Text))
                MessageBox.Show("Такое имя шаблона уже есть", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            else
                templateIsReady = true;
        }

        private void DeleteTemplateButton_Click(object sender, RoutedEventArgs e)
        {
            string templateName = (sender as Button).DataContext.ToString();
            if (MessageBox.Show("Вы уверены, что хотите удалить " + templateName + "?",
                                "Удаление шаблона",
                                MessageBoxButton.YesNo,
                                MessageBoxImage.Question) == MessageBoxResult.Yes)
                DeleteTemplate(templateName);
        }

        private void DeleteTemplate(string name)
        {
            ListBoxOfTemplates.Items.Remove(name);
            ClearTemplate();
            File.Delete("sound templates/" + name + ".bin");
            File.Delete("music templates/" + name + ".bin");
            File.Delete("sfx templates/" + name + ".bin");
        }

        private void ClearTemplate()
        {
            CloseAllSoundsAndMelodies();
            PlayPauseMelodyButton.Background = Resources["Play"] as ImageBrush;
            playingMelodyName = string.Empty;
            MelodyNameLabel.Content = string.Empty;
            ListBoxOfMelodies.Items.Clear();
            collectionOfSounds.Clear();
            ClearSfxGrid();
        }

        private void ClearSfxGrid()
        {
            for (int i = 1; i < 16; i++)
            {
                if ((SfxGrid.Children[i] as Grid).Children.Count > 2)
                {
                    Button button = (SfxGrid.Children[i] as Grid).Children[4] as Button;
                    SfxCrossButton_Click(button, null);
                }
            }
        }

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
            selectedTemplate = (string)ListBoxOfTemplates.SelectedItem;
            if (selectedTemplate != null)
            {
                StopPlayingTemplate();
                LoadSelectedTemplateSounds();
                LoadSelectedTemplateMelodies();
                LoadSelectedTemplateSfx();
                PlaySelectedTemplate();
            }
            else
                TemplateNameTextBox.Clear();
        }

        private void StopPlayingTemplate()
        {
            for (int i = 0; i < collectionOfSounds.Count; i++)
                collectionOfSounds[i].Stop();
            collectionOfSounds.Clear();
            musicMediaPlayer.Stop();
        }

        private void LoadSelectedTemplateSounds()
        {
            TemplateNameTextBox.Text = selectedTemplate;
            collectionOfSounds.Clear();
            FileWork.ReadFileToSoundCollection(collectionOfSounds, selectedTemplate);
            ListBoxFunctions.SortAscending(ListBoxOfSounds);
        }

        private void LoadSelectedTemplateMelodies()
        {
            ListBoxOfMelodies.Items.Clear();
            FileWork.ReadFileToListBox(ListBoxOfMelodies, selectedTemplate);
        }

        private void LoadSelectedTemplateSfx()
        {
            ClearSfxGrid();
            FileWork.ReadFileToSfxGrid(this, SfxGrid, selectedTemplate);
        }

        private void PlaySelectedTemplate()
        {
            for (int i = 0; i < collectionOfSounds.Count; i++)
                PlaySound(collectionOfSounds[i]);
            if (ListBoxOfMelodies.Items.Count > 0)
                StartMelody(FindMelodyButton(ListBoxOfMelodies.Items[0].ToString()));
        }

        private async void PlaySound(MediaPlayerWithSound sound)
        {
            ConfiguredMediaPlayer(sound);
            Random random = new Random();
            await Task.Delay(random.Next((int)(sound.RepetitionRate * 1000)));
            if (collectionOfSounds.Contains(sound))
                sound.Play();
        }

        private void ConfiguredMediaPlayer(MediaPlayerWithSound sound)
        {
            string soundPath = "sounds/" + sound.Name + ".mp3";
            sound.Open(new Uri(soundPath, UriKind.Relative));
            sound.Volume = sound.SoundVolume * volume[2];
            sound.MediaEnded += MediaPlayerSoundEnded;
        }

        private async void MediaPlayerSoundEnded(object sender, EventArgs e)
        {
            MediaPlayerWithSound sound = sender as MediaPlayerWithSound;
            await Task.Delay(GetDelay(sound));
            ReplayMediaPlayer(sound);
        }

        private int GetDelay(MediaPlayerWithSound sound)
        {
            return (int)(sound.RepetitionRate * 1000);
        }

        private void ReplayMediaPlayer(MediaPlayerWithSound sound)
        {
            if (collectionOfSounds.Contains(sound))
            {
                sound.Position = new TimeSpan(0);
                sound.Play();
            }
        }

        private bool CollectionOfSoundsContains(string name)
        {
            bool contains = false;
            foreach (var sound in collectionOfSounds)
                if (sound.Name == name)
                    contains = true;
            return contains;
        }
        #endregion


        #region Управление громкостью
        readonly Slider[] volumeSliders;
        readonly Button[] volumeButtons;
        readonly double[] volume = new[] { 1.0, 1.0, 1.0, 1.0 };
        readonly double[] pastVolume = new[] { 1.0, 1.0, 1.0, 1.0 };
        readonly bool[] volumeIsMute = new[] { false, false, false, false };

        private void VolumeButton_Click(object sender, RoutedEventArgs e)
        {
            if (VolumeGrid.Visibility == Visibility.Visible)
                VolumeGrid.Visibility = Visibility.Hidden;
            else
                VolumeGrid.Visibility = Visibility.Visible;
        }

        private void GeneralVolumeButton_Click(object sender, RoutedEventArgs e)
        {
            MuteOrUnmuteGeneralVolume(sender);
        }

        private void MuteOrUnmuteGeneralVolume(object sender)
        {
            int volumeIndex = Array.IndexOf(volumeButtons, sender as Button);
            volumeIsMute[volumeIndex] = !volumeIsMute[volumeIndex];
            if (volumeIsMute[volumeIndex])
            {
                volumeButtons[volumeIndex].Background = Resources["Speaker Mute"] as ImageBrush;
                volumeSliders[volumeIndex].Value = 0;
            }
            else
            {
                volumeButtons[volumeIndex].Background = Resources["Speaker"] as ImageBrush;
                volumeSliders[volumeIndex].Value = pastVolume[volumeIndex];
            }
        }

        void MusicVolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (volumeSliders != null)
                ChangeGeneralVolume(sender);
            ApplyMusicVolumeToMelodyMediaPlayer();
        }

        void ChangeGeneralVolume(object sender)
        {
            int volumeIndex = Array.IndexOf(volumeSliders, sender as Slider);
            volume[volumeIndex] = volumeSliders[volumeIndex].Value;
            if (volume[volumeIndex] != 0)
            {
                pastVolume[volumeIndex] = volume[volumeIndex];
                volumeIsMute[volumeIndex] = false;
                volumeButtons[volumeIndex].Background = Resources["Speaker"] as ImageBrush;
            }
            else
            {
                volumeIsMute[volumeIndex] = true;
                volumeButtons[volumeIndex].Background = Resources["Speaker Mute"] as ImageBrush;
            }
        }

        void ApplyMusicVolumeToMelodyMediaPlayer()
        {
            musicMediaPlayer.Volume = volume[1];
        }

        void SoundVolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (volumeSliders != null)
                ChangeGeneralVolume(sender);
            ApplySoundVolumeToTemplateSounds();
        }

        void ApplySoundVolumeToTemplateSounds()
        {
            foreach (var sound in collectionOfSounds)
                sound.Volume = sound.SoundVolume * volume[2];
        }

        void SfxVolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (volumeSliders != null)
                ChangeGeneralVolume(sender);
        }
        #endregion


        #region Закрытие приложения
        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            CloseAllSoundsAndMelodies();
        }

        private void CloseAllSoundsAndMelodies()
        {
            musicMediaPlayer.Close();
            foreach (var sound in collectionOfSounds)
                sound.Close();
        }
        #endregion
    }
}