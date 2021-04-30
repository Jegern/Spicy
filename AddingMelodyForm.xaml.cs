using System;
using System.Windows;

namespace Spicy
{
    public partial class AddingMelodyForm : Window
    {
        public AddingMelodyForm()
        {
            InitializeComponent();
            InitializeOtherComponent();
        }

        private void InitializeOtherComponent()
        {
            ListBoxFunctions.LoadFileNamesFromFolderToList(ListBoxOfMelodies, "music", "mp3");
            ListBoxFunctions.SortAscending(ListBoxOfMelodies);
        }

        private void AddMelodyButton_Click(object sender, RoutedEventArgs e)
        {
            if (ListBoxOfMelodies.SelectedItem != null)
            {
                ListBoxFunctions.AddSuitableObjectToSuitableListBox(this);
                (Owner as MainForm).RewriteMelodiesFile();
                Close();
            }
            else
                MessageBox.Show("Пожалуйста, выберите мелодию", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
}
