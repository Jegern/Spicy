using System;
using System.Windows;

namespace Spicy
{
    public partial class AddingMelodyForm : Window
    {
        internal bool MelodyIsReady = false;
        internal string NewMelody = string.Empty;

        public AddingMelodyForm()
        {
            InitializeComponent();
        }

        private void AddingMelodyWindow_Loaded(object sender, RoutedEventArgs e)
        {
            ListBoxFunctions.LoadFileNamesFromFolderToList(ListBoxOfMelodies, "music", "mp3");
            ListBoxFunctions.RemoveNamesOfFirstListBoxFromSecondListBox((Owner as MainForm).ListBoxOfMelodies, ListBoxOfMelodies);
            ListBoxFunctions.SortAscending(ListBoxOfMelodies);
        }

        private void AddMelodyButton_Click(object sender, RoutedEventArgs e)
        {
            CheckIfMelodyIsReady();
            if (MelodyIsReady)
            {
                NewMelody = ListBoxOfMelodies.SelectedItem.ToString();
                Close();
            }
        }

        private void CheckIfMelodyIsReady()
        {
            if (ListBoxOfMelodies.SelectedItem == null)
                MessageBox.Show("Пожалуйста, выберите мелодию", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            else
                MelodyIsReady = true;
        }
    }
}