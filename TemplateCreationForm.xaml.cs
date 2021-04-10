using System.Windows;
using System.IO;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;

namespace Spicy
{
    public partial class TemplateCreationForm : Window
    {
        bool leftMouseDown = false;

        public TemplateCreationForm()
        {
            InitializeComponent();
            TemplateName.Focus();
        }

        private void CreateTemplateButton_Click(object sender, RoutedEventArgs e)
        {
            using (BinaryWriter writer = new BinaryWriter(File.Open("bin/templates/" + TemplateName.Text + ".bin", FileMode.Create)))
                foreach (var item in ListOfIncluded.Items)
                    writer.Write(item.ToString() + ".wav");

            AddToListOfReadyMadeTemplates();
            Close();
        }

        private void TemplateName_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                CreateTemplateButton_Click(null, null);
        }

        private void AddToListOfReadyMadeTemplates()
        {
            ListBox listOfTemplates = (Owner as MainForm).listOfReadyMadeTemplates;
            listOfTemplates.Items.Add(TemplateName.Text);
            listOfTemplates.Items.SortDescriptions.Add(new SortDescription("", ListSortDirection.Ascending));
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
            if (ListOfSounds.SelectedIndex != -1)
            {
                ListOfIncluded.Items.Add(ListOfSounds.SelectedItem);
                ListOfSounds.Items.Remove(ListOfSounds.SelectedItem);
                ListOfIncluded.Items.SortDescriptions.Add(new SortDescription("", ListSortDirection.Ascending));
            }
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
            {
                ListOfIncluded.Items.Add(ListOfSounds.SelectedItem);
                ListOfSounds.Items.Remove(ListOfSounds.SelectedItem);
                ListOfIncluded.Items.SortDescriptions.Add(new SortDescription("", ListSortDirection.Ascending));
            }
        }

        #endregion

        #region Included -> Sounds

        private void ListOfIncluded_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ListOfIncluded.SelectedIndex != -1)
            {
                ListOfSounds.Items.Add(ListOfIncluded.SelectedItem);
                ListOfIncluded.Items.Remove(ListOfIncluded.SelectedItem);
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
            {
                ListOfSounds.Items.Add(ListOfIncluded.SelectedItem);
                ListOfIncluded.Items.Remove(ListOfIncluded.SelectedItem);
                ListOfSounds.Items.SortDescriptions.Add(new SortDescription("", ListSortDirection.Ascending));
            }
        }

        #endregion
    }
}