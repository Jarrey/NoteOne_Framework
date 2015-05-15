using System;
using System.Collections.ObjectModel;
using System.Linq;
using NoteOne_Core.Common.Models;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

// The Templated Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234235

namespace NoteOne_Core.UI.Common
{
    internal enum ProjectionDirection
    {
        RotationX = 0,
        RotationY = 1
    }

    public sealed class PollImage : Control
    {
        #region Constructor and Fields

        private readonly Random random = new Random(DateTime.Now.Millisecond);
        private readonly DispatcherTimer timer;
        private int currentImageIndex;
        private PlaneProjection planeProjection;

        public PollImage()
        {
            DefaultStyleKey = typeof(PollImage);

            if (timer == null)
                timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(random.Next(10, 60)) };
        }

        #endregion

        #region Dependency Properties

        public static readonly DependencyProperty ImagesProperty =
            DependencyProperty.Register("Images", typeof(Collection<BindableImage>), typeof(PollImage),
                new PropertyMetadata(null,
                                        (o, e) =>
                                        {
                                            var images = e.NewValue as Collection<BindableImage>;
                                            if (images != null && images.Count() > 0)
                                                (o as PollImage).CurrentImage = (e.NewValue as Collection<BindableImage>).ElementAt(0);
                                        }));

        public static readonly DependencyProperty CurrentImageProperty =
            DependencyProperty.Register("CurrentImage", typeof(BindableImage), typeof(PollImage),
                                        new PropertyMetadata(null));

        public static readonly DependencyProperty DurationProperty =
            DependencyProperty.Register("Duration", typeof(double), typeof(PollImage), new PropertyMetadata(1.0));

        public static readonly DependencyProperty IntervalProperty =
            DependencyProperty.Register("Interval", typeof(int), typeof(PollImage), new PropertyMetadata(
                30,
                (o, e) =>
                {
                    var image = o as PollImage;
                    var interval = (double)e.NewValue;
                    if (interval > 10 && interval < 120)
                        image.timer.Interval = TimeSpan.FromSeconds(image.random.Next(10, (int)e.NewValue));
                    else
                        image.timer.Interval = TimeSpan.FromSeconds(10);
                }));


        public static readonly DependencyProperty IsEnableTransitionProperty =
            DependencyProperty.Register("IsEnableTransition", typeof(bool), typeof(PollImage),
                new PropertyMetadata(true,
                                        (o, e) =>
                                        {
                                            var image = o as PollImage;
                                            if ((bool)e.NewValue)
                                                image.timer.Start();
                                            else
                                                image.timer.Stop();
                                        }));

        public Collection<BindableImage> Images
        {
            get { return (Collection<BindableImage>)GetValue(ImagesProperty); }
            set { SetValue(ImagesProperty, value); }
        }

        public BindableImage CurrentImage
        {
            get { return (BindableImage)GetValue(CurrentImageProperty); }
            set { SetValue(CurrentImageProperty, value); }
        }

        public double Duration
        {
            get { return (double)GetValue(DurationProperty); }
            set { SetValue(DurationProperty, value); }
        }

        public int Interval
        {
            get { return (int)GetValue(IntervalProperty); }
            set { SetValue(IntervalProperty, value); }
        }

        public bool IsEnableTransition
        {
            get { return (bool)GetValue(IsEnableTransitionProperty); }
            set { SetValue(IsEnableTransitionProperty, value); }
        }

        #endregion

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            planeProjection = GetTemplateChild("planeProjection") as PlaneProjection;

            if (planeProjection == null)
                throw new Exception("Can not find planeProjection content in this control template");

            timer.Tick -= Timer_Tick;
            timer.Tick += Timer_Tick;
            if (IsEnableTransition)
                timer.Start();
        }

        private void Timer_Tick(object sender, object e)
        {
            if (Images != null && Images.Count() > 1)
            {
                int imageCount = Images.Count();
                int index = currentImageIndex + 1;
                if (index == imageCount) index = 0;
                BindableImage nextImage = Images.ElementAt(index);
                ProjectionTransition(nextImage, (ProjectionDirection)(random.Next() % 2));
                currentImageIndex = index;
            }
        }

        /// <summary>
        ///     Projection by RotationY
        /// </summary>
        /// <param name="toImage"></param>
        private void ProjectionTransition(BindableImage toImage,
                                          ProjectionDirection direction = ProjectionDirection.RotationY)
        {
            double halfDuration = Duration / 2.0;
            var fromStoryboard = new Storyboard();
            var toStoryboard = new Storyboard();

            #region Animations

            var fromAniamtion = new DoubleAnimationUsingKeyFrames { EnableDependentAnimation = true };
            fromAniamtion.KeyFrames.Add(new DiscreteDoubleKeyFrame
                {
                    KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0)),
                    Value = 0
                });
            fromAniamtion.KeyFrames.Add(new LinearDoubleKeyFrame
                {
                    KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(halfDuration)),
                    Value = 90
                });
            fromAniamtion.KeyFrames.Add(new DiscreteDoubleKeyFrame
                {
                    KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(halfDuration)),
                    Value = 90
                });
            Storyboard.SetTarget(fromAniamtion, planeProjection);
            Storyboard.SetTargetProperty(fromAniamtion, Enum.GetName(typeof(ProjectionDirection), direction));
            fromStoryboard.Children.Add(fromAniamtion);

            var toAniamtion = new DoubleAnimationUsingKeyFrames { EnableDependentAnimation = true };
            toAniamtion.KeyFrames.Add(new DiscreteDoubleKeyFrame
                {
                    KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0)),
                    Value = -90
                });
            //toAniamtion.KeyFrames.Add(new LinearDoubleKeyFrame() { KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(halfDuration)), Value = 0 });
            toAniamtion.KeyFrames.Add(new EasingDoubleKeyFrame
                {
                    KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(halfDuration)),
                    Value = 0,
                    EasingFunction =
                        new ElasticEase { Oscillations = 2, Springiness = 3, EasingMode = EasingMode.EaseOut }
                });
            toAniamtion.KeyFrames.Add(new DiscreteDoubleKeyFrame
                {
                    KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(halfDuration)),
                    Value = 0
                });
            Storyboard.SetTarget(toStoryboard, planeProjection);
            Storyboard.SetTargetProperty(toAniamtion, Enum.GetName(typeof(ProjectionDirection), direction));
            toStoryboard.Children.Add(toAniamtion);

            #endregion

            fromAniamtion.Completed += (obj, e) =>
                {
                    CurrentImage = toImage;
                    toStoryboard.Begin();
                };
            fromStoryboard.Begin();
        }
    }
}