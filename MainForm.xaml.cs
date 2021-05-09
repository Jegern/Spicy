using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace Spicy
{
    public partial class MainForm : Window
    {
        #region Инициализация
        public MainForm()
        {
            InitializeComponent();
            InitializeOtherComponent();
        }

        private void InitializeOtherComponent()
        {
            HideTemplateAndSfxMenu();
            InitializeListBoxOfTemplateSounds();
            LoadTemplatesInListBox();
        }

        private void HideTemplateAndSfxMenu()
        {
            TemplateGrid.Width = new GridLength(0);
            SfxGrid.Width = new GridLength(0);
            Width -= Width / 2;
        }

        void InitializeListBoxOfTemplateSounds()
        {
            ListBoxOfSounds.ItemsSource = collectionOfSounds;
            ListBoxOfSounds.DisplayMemberPath = "Name";
        }

        void LoadTemplatesInListBox()
        {
            ListBoxFunctions.LoadFileNamesFromFolderToList(ListBoxOfTemplates, "sound templates", "bin");
            ListBoxFunctions.SortAscending(ListBoxOfTemplates);
        }

        private void TemplateMenu_Click(object sender, RoutedEventArgs e)
        {
            MinimizeMenu(SfxGrid);
            ExpandMenu(TemplateGrid);
        }

        private void SfxMenu_Click(object sender, RoutedEventArgs e)
        {
            MinimizeMenu(TemplateGrid);
            ExpandMenu(SfxGrid);
        }

        void MinimizeMenu(ColumnDefinition grid)
        {
            if (grid.Width != new GridLength(0))
            {
                grid.Width = new GridLength(0);
                Width -= Width / 3;
            }
        }

        void ExpandMenu(ColumnDefinition grid)
        {
            if (grid.Width == new GridLength(0))
            {
                grid.Width = new GridLength(1, GridUnitType.Star);
                Width += Width / 2;
            }
        }
        #endregion


        #region Управление звуками
        readonly ObservableCollection<Sound> collectionOfSounds = new ObservableCollection<Sound>();

        private void AddSoundButton_Click(object sender, RoutedEventArgs e)
        {
            AddingSoundForm addingSoundForm = new AddingSoundForm(this);
            addingSoundForm.Show();
        }

        private void DeleteSoundButton_Click(object sender, RoutedEventArgs e)
        {
            collectionOfSounds.Remove(((sender as Button).TemplatedParent as ListBoxItem).Content as Sound);
        }
        #endregion


        #region Управление мелодиями
        private void AddMelodyButton_Click(object sender, RoutedEventArgs e)
        {
            AddingMelodyForm addingMelodyForm = new AddingMelodyForm(this);
            addingMelodyForm.Show();
        }

        private void DeleteMelodyButton_Click(object sender, RoutedEventArgs e)
        {
            ListBoxOfMelodies.Items.Remove(((sender as Button).TemplatedParent as ListBoxItem).Content);
        }
        #endregion


        #region Управление шаблонами
        private void AddTemplateButton_Click(object sender, RoutedEventArgs e)
        {
            TemplateCreationForm templateCreationForm = new TemplateCreationForm();
            templateCreationForm.Show();
        }
        #endregion
    }
}