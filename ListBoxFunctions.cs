using System.IO;
using System.ComponentModel;
using System.Windows.Controls;

namespace Spicy
{
    public static class ListBoxFunctions
    {
        public static void SortAscending(ListBox listBox)
        {
            listBox.Items.SortDescriptions.Add(new SortDescription("", ListSortDirection.Ascending));
        }

        public static void SortAscending(ListBox listBox, string propertyName)
        {
            listBox.Items.SortDescriptions.Add(new SortDescription(propertyName, ListSortDirection.Ascending));
        }

        public static void LoadFileNamesFromFolderToList(ListBox listBox, string folder, string extension)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(folder + "/");
            foreach (var fileName in directoryInfo.GetFiles("*." + extension))
                listBox.Items.Add(fileName.Name.Replace("." + extension, ""));
        }
    }
}
