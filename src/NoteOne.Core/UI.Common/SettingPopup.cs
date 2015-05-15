using System;
using Windows.Foundation;
using Windows.UI.ApplicationSettings;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media.Animation;

namespace NoteOne_Core.UI.Common
{
    public enum SettingSourceFlag
    {
        ByCharm = 1,
        ByApp = 0
    }

    public class SettingPopup
    {
        #region Current Popup

        public static SettingPopup Current;

        #endregion

        private readonly SettingSourceFlag _settingSourceFlag = SettingSourceFlag.ByApp;
        private readonly Popup _settingsPopup;
        private readonly Rect _windowBounds;
        private readonly NarrowPane pane;

        public SettingPopup(string title, SettingSourceFlag flag = SettingSourceFlag.ByApp, double settingsWidth = 380)
        {
            _settingSourceFlag = flag;
            _settingsPopup = new Popup();
            _settingsPopup.Opened += OnPopupOpened;
            _settingsPopup.Closed += OnPopupClosed;
            Window.Current.Activated += OnWindowActivated;
            _windowBounds = Window.Current.Bounds;
            _settingsPopup.IsLightDismissEnabled = true;
            _settingsPopup.Width = settingsWidth;
            _settingsPopup.Height = _windowBounds.Height;

            pane = new NarrowPane
                {
                    Title = title,
                    Width = settingsWidth,
                    Height = _windowBounds.Height
                };
            pane.Loaded += OnPaneLoaded;

            _settingsPopup.Child = pane;
            _settingsPopup.HorizontalOffset = _windowBounds.Width - settingsWidth;
            _settingsPopup.VerticalOffset = 0;
        }

        public Object Content
        {
            get { return pane.Content; }
            set { pane.Content = value; }
        }

        #region events

        public event EventHandler<object> PopupClosed;

        private void OnWindowActivated(object sender, WindowActivatedEventArgs e)
        {
            if (e.WindowActivationState == CoreWindowActivationState.Deactivated)
            {
                _settingsPopup.IsOpen = false;
            }
        }

        private void OnPaneLoaded(object sender, RoutedEventArgs e)
        {
            pane.BackButton.Click += OnBackButtonClick;
        }

        private void OnBackButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OnPopupOpened(object sender, object e)
        {
            if (pane != null && pane.BackButton != null)
            {
                pane.BackButton.Click -= OnBackButtonClick;
                pane.BackButton.Click += OnBackButtonClick;
            }
        }

        private void OnPopupClosed(object sender, object e)
        {
            if (PopupClosed != null)
                PopupClosed(sender, e);

            pane.BackButton.Click -= OnBackButtonClick;
            pane.Loaded -= OnPaneLoaded;
            Window.Current.Activated -= OnWindowActivated;
            _settingsPopup.Closed -= OnPopupClosed;
        }

        #endregion

        public void Show()
        {
            if (_settingsPopup != null)
            {
                // Play open Popup animation

                #region animations

                var popupStoryboard = new Storyboard();
                var opacityAniamtion = new DoubleAnimation
                    {
                        EnableDependentAnimation = true,
                        From = 0,
                        To = 1,
                        Duration = TimeSpan.FromSeconds(.3)
                    };
                Storyboard.SetTarget(opacityAniamtion, _settingsPopup);
                Storyboard.SetTargetProperty(opacityAniamtion, "Opacity");
                popupStoryboard.Children.Add(opacityAniamtion);
                popupStoryboard.Begin();

                #endregion

                _settingsPopup.IsOpen = true;
                Current = this;
            }
        }

        public void Close()
        {
            _settingsPopup.IsOpen = false;
            Current = null;
            if (_settingSourceFlag == SettingSourceFlag.ByCharm)
                SettingsPane.Show();
        }
    }
}