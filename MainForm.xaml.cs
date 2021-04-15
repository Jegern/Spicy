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

        private void musicVolumeButton_Click(object sender, RoutedEventArgs e)
        {
            RemoveOrReturnSound(musicVolumeSlider, ref musicVolume);
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

        private void musicVolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            musicVolume = (int)(sender as Slider).Value;
        }

        private void ambientVolumeButton_Click(object sender, RoutedEventArgs e)
        {
            RemoveOrReturnSound(ambientVolumeSlider, ref ambientVolume);
        }

        private void ambientVolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ambientVolume = (int)(sender as Slider).Value;
        }

        private void sfxVolumeButton_Click(object sender, RoutedEventArgs e)
        {
            RemoveOrReturnSound(sfxVolumeSlider, ref sfxVolume);
        }

        private void sfxVolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            sfxVolume = (int)(sender as Slider).Value;
        }

        #endregion

        #region SFX Button Click

        private void sfxButton1_Click(object sender, RoutedEventArgs e)
        {
            AddSfxToButton(sender);
        }

        private void AddSfxToButton(object sender)
        {
            Button button = sender as Button;
            //string buttonNumber = button.Name.Substring(button.Name.Length - 1);
            string buttonNumber = Regex.Match(button.Name, @"[0-9]{1,2}").ToString();
            addIconToSfxButton(button, "sfxButtonIcon" + buttonNumber);
            addTextToSfxLabel("sfxTextBlock" + buttonNumber, "SFX звук");
        }

        private void addIconToSfxButton(Button button, string buttonIconName)
        {
            Ellipse ellipse = button.Template.FindName(buttonIconName, button) as Ellipse;
            ellipse.Fill = new ImageBrush(new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory+"//images/Sound icon.png")));
        }

        private void addTextToSfxLabel(string labelName, string text)
        {
            TextBlock label = sfxGrid.FindName(labelName) as TextBlock;
            label.Text = text;
        }

       
        #endregion

        private void listOfReadyMadeTemplates_Loaded(object sender, RoutedEventArgs e)
        {
            DirectoryInfo directory = new DirectoryInfo("bin/templates/");
            foreach (var fileName in directory.GetFiles("*.bin"))
                listOfReadyMadeTemplates.Items.Add(fileName.Name.Replace(".bin", ""));
            listOfReadyMadeTemplates.Items.SortDescriptions.Add(new SortDescription("", ListSortDirection.Ascending));
        }

        private void createTemplateButton_Click(object sender, RoutedEventArgs e)
        {
            TemplateCreationForm templateCreationForm = new TemplateCreationForm();
            templateCreationForm.Owner = this;
            templateCreationForm.Show();
        }

        private void deleteTemplateButton_Click(object sender, RoutedEventArgs e)
        {
            if (listOfReadyMadeTemplates.SelectedIndex != -1)
            {
                File.Delete("bin/templates/" + listOfReadyMadeTemplates.SelectedItem.ToString() + ".bin");
                listOfReadyMadeTemplates.Items.RemoveAt(listOfReadyMadeTemplates.SelectedIndex);
            }
        }
    }
}
