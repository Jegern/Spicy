using System.IO;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;

namespace Spicy
{
    public static class ListBoxFunctions
    {
        public static void SortAscending(ListBox listBox)
        {
            if (listBox.ItemsSource == null)
                listBox.Items.SortDescriptions.Add(new SortDescription("", ListSortDirection.Ascending));
            else
                listBox.Items.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
        }

        public static void LoadFileNamesFromFolderToList(ListBox listBox, string folder, string extension)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(folder + "/");
            foreach (var fileName in directoryInfo.GetFiles("*." + extension))
                listBox.Items.Add(fileName.Name.Replace("." + extension, ""));
        }

        public static void AddObjectToListBox(object name, ListBox listBox)
        {
            if (listBox.ItemsSource == null)
                listBox.Items.Add(name);
            else
                (listBox.ItemsSource as ObservableCollection<Sound>).Add(name as Sound);
            if (listBox.Name != "ListBoxOfMelodies")
                SortAscending(listBox);
        }

        public static void AddSuitableObjectToSuitableListBox(Window window)
        {
            AddObjectToListBox(GetSuitableObject(window), GetSuitableListBox(window));
        }

        static object GetSuitableObject(Window window)
        {
            object suitableObject = null;
            if (window is TemplateCreationForm)
                suitableObject = (window as TemplateCreationForm).TemplateName.Text;
            else if (window is AddingMelodyForm)
                suitableObject = (window as AddingMelodyForm).ListBoxOfMelodies.SelectedItem;
            else if (window is AddingSoundForm)
                suitableObject = (window as AddingSoundForm).NewSound;
            return suitableObject;
        }

        static ListBox GetSuitableListBox(Window window)
        {
            ListBox suitableListBox = null;
            if (window is TemplateCreationForm)
                suitableListBox = ((window as TemplateCreationForm).Owner as MainForm).ListBoxOfTemplates;
            else if (window is AddingMelodyForm)
                suitableListBox = ((window as AddingMelodyForm).Owner as MainForm).ListBoxOfMelodies;
            else if (window is AddingSoundForm)
                suitableListBox = ((window as AddingSoundForm).Owner as MainForm).ListBoxOfSounds;
            return suitableListBox;
        }

        public static void RemoveNamesOfFirstListBoxFromSecondListBox(ListBox firstListBox, ListBox secondListBox)
        {
            if (firstListBox.Name == "ListBoxOfSounds")
                foreach (Sound sound in firstListBox.Items)
                    secondListBox.Items.Remove(sound.Name);
            else
                foreach (var item in firstListBox.Items)
                    secondListBox.Items.Remove(item);
        }

    }
}