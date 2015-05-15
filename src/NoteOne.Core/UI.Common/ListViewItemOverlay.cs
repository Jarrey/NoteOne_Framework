using NoteOne_Core.Command;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

// The Templated Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234235

namespace NoteOne_Core.UI.Common
{
    public sealed class ListViewItemOverlay : ContentControl
    {
        private bool _isOverlayShown;

        public ListViewItemOverlay()
        {
            DefaultStyleKey = typeof(ListViewItemOverlay);
        }

        #region Dependency Properties

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(ListViewItemOverlay),
                                        new PropertyMetadata(string.Empty));

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        #endregion

        /// <summary>
        /// Get the show information command instance
        /// </summary>
        public ICommand ShowInforCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    _isOverlayShown = !_isOverlayShown;
                    VisualStateManager.GoToState(this, _isOverlayShown ? "PointerOver" : "Normal", false);
                });
            }
        }

        protected override void OnPointerEntered(PointerRoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "PointerOver", false);
            _isOverlayShown = true;
            base.OnPointerEntered(e);
        }

        protected override void OnPointerExited(PointerRoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "Normal", false);
            _isOverlayShown = false;
            base.OnPointerExited(e);
        }

        protected override void OnPointerPressed(PointerRoutedEventArgs e)
        {
            //if (_isOverlayShown)
            //{
            //    VisualStateManager.GoToState(this, "Normal", false);
            //    _isOverlayShown = false;
            //}
            //else
            //{
            //    VisualStateManager.GoToState(this, "PointerOver", false);
            //    _isOverlayShown = true;
            //}
            e.Handled = true;
            //base.OnPointerPressed(e);
        }
    }
}