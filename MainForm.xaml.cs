using System;
using System.Windows;
using System.Windows.Controls;

namespace Spicy
{
    public partial class MainForm : Window
    {
        public MainForm()
        {
            InitializeComponent();
            InitializeOtherComponent();
        }

        private void InitializeOtherComponent()
        {
            MinimizeMenu(SfxGrid);
            MinimizeMenu(TemplateGrid);
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
                Width -= 225;
            }
        }

        void ExpandMenu(ColumnDefinition grid)
        {
            if (grid.Width == new GridLength(0))
            {
                grid.Width = new GridLength(0.75, GridUnitType.Star);
                Width += 225;
            }
        }
    }
}