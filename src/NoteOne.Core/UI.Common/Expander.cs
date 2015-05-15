using System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

// The Templated Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234235

namespace NoteOne_Core.UI.Common
{
    public sealed class Expander : ContentControl
    {
        private Button ExpanderButton;
        private string PART_ExpanderButton = "PART_ExpanderButton";
        private string PART_Title = "PART_Title";
        private Border TitleBorder;
        private bool isTitlePressed;

        #region DPs

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(Expander), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty TitleBackgroundProperty =
            DependencyProperty.Register("TitleBackground", typeof(Brush), typeof(Expander),
                                        new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        public static readonly DependencyProperty TitleForegroundProperty =
            DependencyProperty.Register("TitleForeground", typeof(Brush), typeof(Expander),
                                        new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0xDE, 0xFF, 0xFF, 0xFF))));

        public static readonly DependencyProperty IsExpendedProperty =
            DependencyProperty.Register("IsExpended", typeof(bool), typeof(Expander), new PropertyMetadata(true,
            (o, e) =>
            {
                var expander = o as Expander;
                if ((bool)e.NewValue)
                    expander.DoExpand(expander);
                else
                    expander.DoCollapse(expander);
            }));

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public Brush TitleBackground
        {
            get { return (Brush)GetValue(TitleBackgroundProperty); }
            set { SetValue(TitleBackgroundProperty, value); }
        }

        public Brush TitleForeground
        {
            get { return (Brush)GetValue(TitleForegroundProperty); }
            set { SetValue(TitleForegroundProperty, value); }
        }

        public bool IsExpended
        {
            get { return (bool)GetValue(IsExpendedProperty); }
            set { SetValue(IsExpendedProperty, value); }
        }

        #endregion

        public Expander()
        {
            DefaultStyleKey = typeof(Expander);
        }

        #region Eevnt

        public event EventHandler Expanded;
        public event EventHandler Collapsed;

        #endregion

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            TitleBorder = GetTemplateChild(PART_Title) as Border;
            if (TitleBorder != null)
            {
                TitleBorder.PointerExited -= TitleBorder_PointerExited;
                TitleBorder.PointerEntered -= TitleBorder_PointerEntered;
                TitleBorder.PointerExited += TitleBorder_PointerExited;
                TitleBorder.PointerEntered += TitleBorder_PointerEntered;

                TitleBorder.PointerPressed -= TitleBorder_PointerPressed;
                TitleBorder.PointerPressed += TitleBorder_PointerPressed;
                TitleBorder.PointerReleased -= TitleBorder_PointerReleased;
                TitleBorder.PointerReleased += TitleBorder_PointerReleased;
            }

            ExpanderButton = GetTemplateChild(PART_ExpanderButton) as Button;
            if (ExpanderButton != null)
            {
                ExpanderButton.Click -= ExpanderButton_Click;
                ExpanderButton.Click += ExpanderButton_Click;
            }
        }

        private void DoExpand(Expander o)
        {
            VisualStateManager.GoToState(o, "Expanded", false);
            IsExpended = true;
            if (Expanded != null)
                Expanded(o, new EventArgs());
        }

        private void DoCollapse(Expander o)
        {
            VisualStateManager.GoToState(o, "Collapsed", false);
            IsExpended = false;
            if (Collapsed != null)
                Collapsed(o, new EventArgs());
        }

        #region Event Handlers

        private void TitleBorder_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "TitlePointerOver", false);
        }

        private void TitleBorder_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "Normal", false);
        }

        private void TitleBorder_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            isTitlePressed = true;
        }

        private void TitleBorder_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (isTitlePressed)
            {
                if (IsExpended) IsExpended = false;
                else IsExpended = true;
                isTitlePressed = false;
            }
        }

        private void ExpanderButton_Click(object sender, RoutedEventArgs e)
        {
            if (IsExpended) IsExpended = false;
            else IsExpended = true;
        }

        #endregion
    }
}