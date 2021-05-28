using System.Windows;

namespace Spicy
{
    public class Extensions
    {
        public static readonly DependencyProperty SoundProperty =
            DependencyProperty.RegisterAttached("MediaPlayerWithSound", typeof(MediaPlayerWithSound), typeof(Extensions), new PropertyMetadata(default(MediaPlayerWithSound)));

        public static void SetSound(UIElement element, MediaPlayerWithSound sound)
        {
            element.SetValue(SoundProperty, sound);
        }

        public static MediaPlayerWithSound GetSound(UIElement element)
        {
            return (MediaPlayerWithSound)element.GetValue(SoundProperty);
        }
    }
}