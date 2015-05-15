using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media.Animation;

namespace NoteOne_Core.UI.Common
{
    public class MessagePopup
    {
        private readonly string _message;
        private Grid _grid;

        public MessagePopup(string message)
        {
            _message = message;
        }

        /// <summary>
        ///     show message popup
        /// </summary>
        /// <param name="time">duration time, default is 5s, unit is second</param>
        public void Show(int time = 5)
        {
            string popupChildXaml =
                @"<Grid HorizontalAlignment=""Stretch"" xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"">" +
                @"<StackPanel Background=""{StaticResource ListViewItemOverlayBackgroundThemeBrush}"">" +
                @"<TextBlock Text=""{Binding}"" IsHitTestVisible=""False""" +
                @" Foreground=""{StaticResource ListViewItemOverlayForegroundThemeBrush}""" +
                @" Style=""{StaticResource TitleTextStyle}""" +
                @" FontSize=""20""" +
                @" Margin=""20""/>" +
                @"</StackPanel>" +
                @"</Grid>";

            var messagePopup = new Popup { Opacity = 1, IsHitTestVisible = false };
            _grid = XamlReader.Load(popupChildXaml) as Grid;
            if (_grid != null)
            {
                _grid.Width = Window.Current.Bounds.Width;
                _grid.DataContext = _message;
                messagePopup.Child = _grid;

                // register Window size event changed
                Window.Current.SizeChanged += Current_SizeChanged;

                var popupStoryboard = new Storyboard();

                #region animations

                var opacityAniamtion = new DoubleAnimationUsingKeyFrames { EnableDependentAnimation = true };
                opacityAniamtion.KeyFrames.Add(new DiscreteDoubleKeyFrame
                    {
                        KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0)),
                        Value = 0
                    });
                opacityAniamtion.KeyFrames.Add(new LinearDoubleKeyFrame
                    {
                        KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(.5)),
                        Value = 1
                    });
                opacityAniamtion.KeyFrames.Add(new DiscreteDoubleKeyFrame
                    {
                        KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(.5 + time)),
                        Value = 1
                    });
                opacityAniamtion.KeyFrames.Add(new LinearDoubleKeyFrame
                    {
                        KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(1 + time)),
                        Value = 0
                    });
                Storyboard.SetTarget(opacityAniamtion, messagePopup);
                Storyboard.SetTargetProperty(opacityAniamtion, "Opacity");
                popupStoryboard.Children.Add(opacityAniamtion);

                opacityAniamtion.Completed += (obj, e) =>
                {
                    Window.Current.SizeChanged -= Current_SizeChanged;
                    messagePopup.IsOpen = false;
                };

                #endregion

                popupStoryboard.Begin();

                messagePopup.IsOpen = true;
            }
        }

        void Current_SizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
        {
            _grid.Width = Window.Current.Bounds.Width;
        }
    }
}