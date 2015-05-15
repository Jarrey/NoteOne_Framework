using System.Collections.Generic;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Markup;

namespace NoteOne_Core.UI.Common
{
    public class FullScreenPopup
    {
        #region Current Popup

        public static Dictionary<string, FullScreenPopup> Current = new Dictionary<string, FullScreenPopup>();

        #endregion

        private readonly Popup _fullScreenPopup;
        private Rect _windowBounds;
        private readonly string id = "";
        private Grid pane;

        private FullScreenPopup(string id)
        {
            _fullScreenPopup = new Popup();
            _windowBounds = Window.Current.Bounds;
            _fullScreenPopup.IsLightDismissEnabled = false;
            _fullScreenPopup.Width = _windowBounds.Width;
            _fullScreenPopup.Height = _windowBounds.Height;

            Window.Current.SizeChanged += Current_SizeChanged;

            pane = new Grid
                {
                    Width = _windowBounds.Width,
                    Height = _windowBounds.Height
                };

            this.id = id;
        }

        private void Current_SizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
        {
            _windowBounds = Window.Current.Bounds;
            pane.Width = _windowBounds.Width;
            pane.Height = _windowBounds.Height;
        }

        public FullScreenPopup(string id, string contentXaml = "")
            : this(id)
        {
            var content = XamlReader.Load(contentXaml) as UIElement;
            if (content != null)
            {
                pane.Children.Clear();
                pane.Children.Add(content);
            }
            _fullScreenPopup.Child = pane;
        }

        public FullScreenPopup(string id, UIElement content)
            : this(id)
        {
            if (content != null)
            {
                pane.Children.Clear();
                pane.Children.Add(content);
            }
            _fullScreenPopup.Child = pane;
        }

        public void Show()
        {
            if (_fullScreenPopup != null)
            {
                _fullScreenPopup.IsOpen = true;
                Current[id] = this;
            }
        }

        public void Close()
        {
            Window.Current.SizeChanged -= Current_SizeChanged;
            _fullScreenPopup.IsOpen = false;
            Current.Remove(id);
        }
    }
}