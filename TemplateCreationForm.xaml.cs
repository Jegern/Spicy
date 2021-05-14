using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Collections.ObjectModel;

namespace Spicy
{
    public partial class TemplateCreationForm : Window
    {
        internal string TemplateName;
        internal bool TemplateIsReady = false;
        readonly ObservableCollection<Sound> collectionOfSelectedSounds = new ObservableCollection<Sound>();
        bool soundRepetitionRateTextBoxGotFocus = false;

        #region Initialization
        public TemplateCreationForm()
        {
            InitializeComponent();
            InitializeOtherComponent();
        }

        void InitializeOtherComponent()
        {
            InitializeListBoxOfAllSounds();
            InitializeListBoxOfSelectedSounds();
        }

        void InitializeListBoxOfAllSounds()
        {
            ListBoxFunctions.LoadFileNamesFromFolderToList(ListBoxOfAllSounds, "sounds", "mp3");
            ListBoxFunctions.SortAscending(ListBoxOfAllSounds);
        }

        void InitializeListBoxOfSelectedSounds()
        {
            ListBoxOfSelectedSounds.ItemsSource = collectionOfSelectedSounds;
            ListBoxOfSelectedSounds.DisplayMemberPath = "Name";
        }
        #endregion


        #region Move from all to selected
        void ListBoxOfAllSounds_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MoveSoundFromAllToSelectedAndSort();
        }

        void MoveSoundFromAllToSelectedAndSort()
        {
            if (ListBoxOfAllSounds.SelectedItem != null)
            {
                MoveSoundFromAllToSelected();
                ListBoxFunctions.SortAscending(ListBoxOfSelectedSounds);
            }
        }

        void MoveSoundFromAllToSelected()
        {
            string soundName = ListBoxOfAllSounds.SelectedItem.ToString();
            collectionOfSelectedSounds.Add(CreateSoundWithSettings(soundName));
            ListBoxOfAllSounds.Items.Remove(soundName);
        }

        Sound CreateSoundWithSettings(string name)
        {
            double volume = SoundVolumeSlider.Value;
            int.TryParse(SoundRepetitionRateTextbox.Text, out int repetitionRate);

            return new Sound(name, volume, repetitionRate);
        }
        #endregion


        #region Move from selected to all
        void ListBoxOfSelectedSounds_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MoveSoundFromSelectedToAllAndSort();
        }

        void MoveSoundFromSelectedToAllAndSort()
        {
            if (ListBoxOfSelectedSounds.SelectedItem != null)
            {
                MoveSoundFromSelectedToAll();
                ListBoxFunctions.SortAscending(ListBoxOfAllSounds);
            }
        }

        void MoveSoundFromSelectedToAll()
        {
            Sound selectedSound = ListBoxOfSelectedSounds.SelectedItem as Sound;
            ListBoxOfAllSounds.Items.Add(selectedSound.Name);
            collectionOfSelectedSounds.Remove(selectedSound);
        }
        #endregion


        #region Control components of selected sounds
        void ListBoxOfSelectedSounds_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListBoxOfSelectedSounds.SelectedItem != null)
            {
                Sound selectedSound = ListBoxOfSelectedSounds.SelectedItem as Sound;
                SoundVolumeSlider.Value = selectedSound.Volume;
                SoundRepetitionRateTextbox.Text = selectedSound.RepetitionRate.ToString();
            }
        }

        void SoundVolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (ListBoxOfSelectedSounds.SelectedItem != null)
                (ListBoxOfSelectedSounds.SelectedItem as Sound).Volume = SoundVolumeSlider.Value;
        }

        void SoundRepetitionRateTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ListBoxOfSelectedSounds.SelectedItem != null && SoundRepetitionRateTextbox.Text.Length > 0)
                (ListBoxOfSelectedSounds.SelectedItem as Sound).RepetitionRate = Convert.ToInt32(SoundRepetitionRateTextbox.Text);
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
        #endregion


        #region Сompletion of template creation
        void TemplateName_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                CompleteTemplateCreation();
        }

        void CreateTemplateButton_Click(object sender, RoutedEventArgs e)
        {
            CompleteTemplateCreation();
        }

        void CompleteTemplateCreation()
        {
            CheckIfTemplateIsReady();
            if (TemplateIsReady)
            {
                FileWork.WriteSoundCollectionToFile(collectionOfSelectedSounds, TemplateNameTextBox.Text);
                TemplateName = TemplateNameTextBox.Text;
                Close();
            }
        }

        void CheckIfTemplateIsReady()
        {
            if (TemplateNameTextBox.Text.Length == 0)
                MessageBox.Show("Пожалуйста, введите имя шаблона", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            else if ((Owner as MainForm).ListBoxOfTemplates.Items.Contains(TemplateNameTextBox.Text))
                MessageBox.Show("Такое имя шаблона уже есть", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            else
                TemplateIsReady = true;
        }
        #endregion
    }
}