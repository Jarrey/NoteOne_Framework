using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

// The Templated Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234235

namespace NoteOne_Core.UI.Common
{
    public sealed class NarrowPane : ContentControl
    {
        public NarrowPane()
        {
            DefaultStyleKey = typeof (NarrowPane);
        }

        #region dependency properties

        public static readonly DependencyProperty TitleBackgroundProperty =
            DependencyProperty.Register("TitleBackground", typeof (Brush), typeof (NarrowPane),
                                        new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0xDD, 0, 0, 0))));

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof (string), typeof (NarrowPane),
                                        new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty TitleForegroundProperty =
            DependencyProperty.Register("TitleForeground", typeof (Brush), typeof (NarrowPane),
                                        new PropertyMetadata(new SolidColorBrush(Colors.White)));

        public Brush TitleBackground
        {
            get { return (Brush) GetValue(TitleBackgroundProperty); }
            set { SetValue(TitleBackgroundProperty, value); }
        }

        public string Title
        {
            get { return (string) GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public Brush TitleForeground
        {
            get { return (Brush) GetValue(TitleForegroundProperty); }
            set { SetValue(TitleForegroundProperty, value); }
        }

        #endregion

        public Button BackButton { get; set; }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            BackButton = GetTemplateChild("backButton") as Button;
        }
    }
}