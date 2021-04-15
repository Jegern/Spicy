using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Text.RegularExpressions;
using System.Windows.Media;
using System.IO;
using System.ComponentModel;

namespace Spicy
{
    public partial class MainForm : Window
    {
        int musicVolume = 100;
        int ambientVolume = 100;
        int sfxVolume = 100;

        public MainForm()
        {
            InitializeComponent();
        }

        #region Sound Control

        private void MusicVolumeButton_Click(object sender, RoutedEventArgs e)
        {
            RemoveOrReturnSound(MusicVolumeSlider, ref musicVolume);
        }

        private void RemoveOrReturnSound(Slider slider, ref int volume)
        {
            if (volume == 0)
            {
                volume = (int)slider.Value;
                slider.IsEnabled = true;
            }
            else
            {
                volume = 0;
                slider.IsEnabled = false;
            }
        }

        private void MusicVolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            musicVolume = (int)(sender as Slider).Value;
        }

        private void AmbientVolumeButton_Click(object sender, RoutedEventArgs e)
        {
            RemoveOrReturnSound(AmbientVolumeSlider, ref ambientVolume);
        }

        private void AmbientVolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ambientVolume = (int)(sender as Slider).Value;
        }

        private void SfxVolumeButton_Click(object sender, RoutedEventArgs e)
        {
            RemoveOrReturnSound(SfxVolumeSlider, ref sfxVolume);
        }

        private void SfxVolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            sfxVolume = (int)(sender as Slider).Value;
        }

        #endregion

        #region SFX Button Click

        private void SfxButton_Click(object sender, RoutedEventArgs e)
        {
            AddSfxToButton(sender);
        }

        private void AddSfxToButton(object sender)
        {
            Button button = sender as Button;
            string buttonNumber = Regex.Match(button.Name, @"[0-9]{1,2}").ToString();
            AddIconToSfxButton(button, "sfxButtonIcon" + buttonNumber);
            AddTextToSfxLabel("sfxTextBlock" + buttonNumber, "SFX звук");
        }

        private void AddIconToSfxButton(Button button, string buttonIconName)
        {
            Ellipse ellipse = button.Template.FindName(buttonIconName, button) as Ellipse;
            ellipse.Fill = new ImageBrush(new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory+"//images/Sound icon.png")));
        }

        private void AddTextToSfxLabel(string labelName, string text)
        {
            TextBlock label = sfxGrid.FindName(labelName) as TextBlock;
            label.Text = text;
        }
       
        #endregion

        private void ListOfReadyMadeTemplates_Loaded(object sender, RoutedEventArgs e)
        {
            DirectoryInfo directory = new DirectoryInfo("bin/templates/");
            foreach (var fileName in directory.GetFiles("*.bin"))
                ListOfReadyMadeTemplates.Items.Add(fileName.Name.Replace(".bin", ""));
            ListOfReadyMadeTemplates.Items.SortDescriptions.Add(new SortDescription("", ListSortDirection.Ascending));
        }

        private void CreateTemplateButton_Click(object sender, RoutedEventArgs e)
        {
            TemplateCreationForm templateCreationForm = new TemplateCreationForm();
            templateCreationForm.Owner = this;
            templateCreationForm.Show();
        }

        private void DeleteTemplateButton_Click(object sender, RoutedEventArgs e)
        {
            if (ListOfReadyMadeTemplates.SelectedIndex != -1)
            {
                File.Delete("bin/templates/" + ListOfReadyMadeTemplates.SelectedItem.ToString() + ".bin");
                ListOfReadyMadeTemplates.Items.RemoveAt(ListOfReadyMadeTemplates.SelectedIndex);
            }
        }
    }
}
