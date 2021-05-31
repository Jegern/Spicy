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
                (listBox.ItemsSource as ObservableCollection<MediaPlayerWithSound>).Add(name as MediaPlayerWithSound);
            if (listBox.Name != "ListBoxOfMelodies")
                SortAscending(listBox);
        }

        public static void RemoveNamesOfFirstListBoxFromSecondListBox(ListBox firstListBox, ListBox secondListBox)
        {
            if (firstListBox.Name == "ListBoxOfSounds")
                foreach (MediaPlayerWithSound sound in firstListBox.Items)
                    secondListBox.Items.Remove(sound.Name);
            else
                foreach (var item in firstListBox.Items)
                    secondListBox.Items.Remove(item);
        }

        public static Button FindButton(ListBox listBox, string name, string typeOfButton)
        {
            ListBoxItem item = listBox.ItemContainerGenerator.ContainerFromItem(name) as ListBoxItem;
            Button button = item.Template.FindName("Play" + typeOfButton + "Button", item) as Button;
            return button;
        }
    }
}