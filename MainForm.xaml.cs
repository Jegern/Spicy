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
            HideTemplateAndSfxMenu();
        }

        private void HideTemplateAndSfxMenu()
        {
            TemplateGrid.Width = new GridLength(0);
            SfxGrid.Width = new GridLength(0);
            Width -= Width / 2;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DisableButtonHoverAnimation();
        }

        private void DisableButtonHoverAnimation()
        {
            var chrome = MasterVolumeButton.Template.FindName("Chrome", MasterVolumeButton) as Xceed.Wpf.Toolkit.Chromes.ButtonChrome;
            if (chrome != null)
            {
                chrome.RenderMouseOver = false;
                chrome.RenderPressed = false;
            }
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
    }
}