using System;
using System.Windows;
using System.IO;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;
using System.Collections.ObjectModel;

namespace Spicy
{
    public partial class TemplateCreationForm : Window
    {
        readonly ObservableCollection<Sound> collectionOfIncludedSounds;
        bool leftMouseDown = false;

        public TemplateCreationForm()
        {
            InitializeComponent();
            TemplateName.Focus();
            collectionOfIncludedSounds = new ObservableCollection<Sound>();
            ListOfIncluded.ItemsSource = collectionOfIncludedSounds;
            ListOfIncluded.DisplayMemberPath = "Name";
        }

        private void ListOfSounds_Loaded(object sender, RoutedEventArgs e)
        {
            DirectoryInfo directory = new DirectoryInfo("sounds/");
            foreach (var fileName in directory.GetFiles("*.wav"))
                ListOfSounds.Items.Add(fileName.Name.Replace(".wav", ""));
            ListOfSounds.Items.SortDescriptions.Add(new SortDescription("", ListSortDirection.Ascending));
        }

        #region Sounds -> Included

        private void ListOfSounds_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MoveSelectedSoundToIncludedAndSort();
        }

        private void MoveSelectedSoundToIncludedAndSort()
        {
            if (ListOfSounds.SelectedIndex != -1)
            {
                string soundName = ListOfSounds.SelectedItem.ToString();
                collectionOfIncludedSounds.Add(NewSoundWithSettings(soundName));
                ListOfSounds.Items.Remove(ListOfSounds.SelectedItem);

                ListOfIncluded.Items.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
            }
        }

        private Sound NewSoundWithSettings(string name)
        {
            double volume = SoundVolumeSlider.Value;
            int.TryParse(SoundRepetitionRateTextbox.Text, out int repetitionRate);

            return new Sound(name, volume, repetitionRate);
        }

        private void ListOfSounds_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            leftMouseDown = true;
        }

        private void ListOfSounds_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            leftMouseDown = false;
        }

        private void ListOfSounds_MouseLeave(object sender, MouseEventArgs e)
        {
            leftMouseDown = false;
        }

        private void ListOfSounds_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (leftMouseDown)
                DragDrop.DoDragDrop(ListOfSounds, ListOfSounds, DragDropEffects.Move);
        }

        private void ListOfIncluded_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetData(typeof(ListBox)) as ListBox == ListOfSounds)
                MoveSelectedSoundToIncludedAndSort();
        }

        #endregion

        #region List of included selection

        private void ListOfIncluded_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SoundVolumeSlider.Value = (ListOfIncluded.SelectedItem as Sound).Volume;
            SoundRepetitionRateTextbox.Text = (ListOfIncluded.SelectedItem as Sound).RepetitionRate.ToString();
        }

        private void SoundVolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (ListOfIncluded.SelectedIndex != -1)
                (ListOfIncluded.SelectedItem as Sound).Volume = SoundVolumeSlider.Value;
        }

        private void SoundRepetitionRateTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ListOfIncluded.SelectedIndex != -1 && SoundRepetitionRateTextbox.Text.Length != 0)
                (ListOfIncluded.SelectedItem as Sound).RepetitionRate = Convert.ToInt32(SoundRepetitionRateTextbox.Text);
        }

        #endregion

        #region Included -> Sounds

        private void ListOfIncluded_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MoveSelectedItemFromIncludedAndSort();
        }

        private void MoveSelectedItemFromIncludedAndSort()
        {
            if (ListOfIncluded.SelectedIndex != -1)
            {
                ListOfSounds.Items.Add((ListOfIncluded.SelectedItem as Sound).Name);
                collectionOfIncludedSounds.RemoveAt(ListOfIncluded.SelectedIndex);

                ListOfSounds.Items.SortDescriptions.Add(new SortDescription("", ListSortDirection.Ascending));
            }
        }

        private void ListOfIncluded_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            leftMouseDown = true;
        }

        private void ListOfIncluded_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            leftMouseDown = false;
        }

        private void ListOfIncluded_MouseLeave(object sender, MouseEventArgs e)
        {
            leftMouseDown = false;
        }

        private void ListOfIncluded_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (leftMouseDown)
                DragDrop.DoDragDrop(ListOfIncluded, ListOfIncluded, DragDropEffects.Move);
        }

        private void ListOfSounds_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetData(typeof(ListBox)) as ListBox == ListOfIncluded)
                MoveSelectedItemFromIncludedAndSort();
        }

        #endregion

        #region Сompletion of template creation

        private void TemplateName_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                CreateTemplateAndCloseForm();
        }

        private void CreateTemplateButton_Click(object sender, RoutedEventArgs e)
        {
            CreateTemplateAndCloseForm();
        }

        private void CreateTemplateAndCloseForm()
        {
            if (TemplateName.Text.Length != 0)
            {
                WriteTemplateInFile();
                AddToListOfReadyMadeTemplates();
                Close();
            }
            else
                MessageBox.Show("Пожалуйста, введите имя шаблона", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void WriteTemplateInFile()
        {
            using (BinaryWriter writer = new BinaryWriter(File.Open("bin/templates/" + TemplateName.Text + ".bin", FileMode.Create)))
                foreach (var sound in collectionOfIncludedSounds)
                {
                    string soundFileName = sound.Name + ".wav";
                    string soundVolume = sound.Volume.ToString();
                    string soundRepetitionRate = sound.RepetitionRate.ToString();
                    writer.Write(soundFileName + " " + soundVolume + " " + soundRepetitionRate);
                }
        }

        private void AddToListOfReadyMadeTemplates()
        {
            ListBox listOfTemplates = (Owner as MainForm).ListOfTemplates;
            listOfTemplates.Items.Add(TemplateName.Text);
            listOfTemplates.Items.SortDescriptions.Add(new SortDescription("", ListSortDirection.Ascending));
        }

        #endregion
    }
}