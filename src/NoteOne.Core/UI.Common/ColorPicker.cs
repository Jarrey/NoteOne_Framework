/// Reference from: https://github.com/shivamcv/Color_Picker

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using NoteOne_Utility.Extensions;
using Windows.Storage.Streams;
using System.Reflection;
using Windows.UI.Xaml.Media.Imaging;
using System.Runtime.CompilerServices;
using System.IO;

// The Templated Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234235

namespace NoteOne_Core.UI.Common
{
    public sealed class ColorPicker : Control
    {
        #region UI Compoents

        private Grid pointer;
        private Border reference;
        private CompositeTransform rtrnsfrm;
        private Canvas innerCanvas;
        private Grid innerEll;
        private BitmapDecoder bd;
        private Ellipse finalColor;
        private Rectangle rectColor;
        private Image colorImg;
        private GradientStop gdStop;
        private Viewbox clrViewbox;
        private Thumb thumbInnerEll;

        #endregion

        #region Firlds

        private byte[] tempBuffer;
        private bool isLoaded = false;
        private bool isPressed = false;
        private double px, py;

        #endregion

        #region Constructor and Dispose method

        public ColorPicker()
        {
            this.DefaultStyleKey = typeof(ColorPicker);
        }

        #endregion

        #region Dependency Members

        public Color SelectedColor
        {
            get { return (Color)GetValue(SelectedColorProperty); }
            set
            {
                SetValue(SelectedColorProperty, value);
                SelectedBrush = new SolidColorBrush(value);
            }
        }

        public static readonly DependencyProperty SelectedColorProperty =
            DependencyProperty.Register("SelectedColor", typeof(Color), typeof(ColorPicker), new PropertyMetadata(Colors.Yellow));


        public Brush SelectedBrush
        {
            get { return (Brush)GetValue(SelectedBrushProperty); }
            set
            {
                SetValue(SelectedBrushProperty, value);
                OnColorChanged();
            }
        }

        public static readonly DependencyProperty SelectedBrushProperty =
            DependencyProperty.Register("SelectedBrush", typeof(Brush), typeof(ColorPicker), new PropertyMetadata(new SolidColorBrush(Colors.Yellow)));

        public event EventHandler ColorChanged;

        private void OnColorChanged()
        {
            EventHandler eventHandler = this.ColorChanged;
            if (eventHandler != null && isLoaded == true)
            {
                eventHandler(this, new PropertyChangedEventArgs("SelectedColor"));
                eventHandler(this, new PropertyChangedEventArgs("SelectedBrush"));
            }
        }

        #endregion

        #region Event Handlers

        void ColorPickerLoaded(object sender, RoutedEventArgs e)
        {
            isLoaded = true;
        }

        private void ColorImgPointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (isPressed)
            {
                px = e.GetCurrentPoint(reference).Position.X;
                py = e.GetCurrentPoint(reference).Position.Y;

                this.rtrnsfrm.Rotation = Math.Atan2(py, px) * (180 / Math.PI) + 135;
                FillColor();
            }
        }

        private void ColorImgPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            isPressed = true;
        }

        private void ColorImgPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            isPressed = false;
        }

        private void ColorImgTapped(object sender, TappedRoutedEventArgs e)
        {
            px = e.GetPosition(reference).X;
            py = e.GetPosition(reference).Y;
            this.rtrnsfrm.Rotation = Math.Atan2(py, px) * (180 / Math.PI) + 135;
            FillColor();
        }
        private void RectanglePointerPressed(object sender, PointerRoutedEventArgs e)
        {
            double x, y;

            x = e.GetCurrentPoint(innerCanvas).Position.X;
            y = e.GetCurrentPoint(innerCanvas).Position.Y;

            this.innerEll.SetValue(Canvas.LeftProperty, x - 4);
            this.innerEll.SetValue(Canvas.TopProperty, y - 4);
            this.finalColor.Fill = new SolidColorBrush(LinearGdHelperClass.GetColorAtPoint(rectColor, new Point(x + 4, y + 4)));
        }

        private void ThumbDragDelta(object sender, DragDeltaEventArgs e)
        {
            Grid gd = (Grid)((Thumb)sender).Parent;

            var x = (double)gd.GetValue(Canvas.LeftProperty) + e.HorizontalChange;
            var y = (double)gd.GetValue(Canvas.TopProperty) + e.VerticalChange;

            if (x < this.innerCanvas.Width - 4 && y < this.innerCanvas.Height - 4 && x > -4 && y > -4)
            {
                this.innerEll.SetValue(Canvas.LeftProperty, x);
                this.innerEll.SetValue(Canvas.TopProperty, y);

                this.finalColor.Fill = new SolidColorBrush(LinearGdHelperClass.GetColorAtPoint(rectColor, new Point(x + 4, y + 4)));
                this.SelectedColor = LinearGdHelperClass.GetColorAtPoint(rectColor, new Point(x + 4, y + 4));
            }
        }

        #endregion

        #region Methods

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.clrViewbox = GetTemplateChild("clrViewbox") as Viewbox;
            this.pointer = GetTemplateChild("pointer") as Grid;
            this.reference = GetTemplateChild("reference") as Border;
            this.rtrnsfrm = GetTemplateChild("rtrnsfrm") as CompositeTransform;
            this.innerCanvas = GetTemplateChild("innerCanvas") as Canvas;
            this.innerEll = GetTemplateChild("innerEll") as Grid;
            this.colorImg = GetTemplateChild("colorImg") as Image;
            this.thumbInnerEll = GetTemplateChild("thumbInnerEll") as Thumb;
            this.rectColor = GetTemplateChild("rectColor") as Rectangle;
            this.gdStop = GetTemplateChild("gdStop") as GradientStop;
            this.finalColor = GetTemplateChild("finalColor") as Ellipse;

            this.Loaded += ColorPickerLoaded;
            this.colorImg.Tapped += ColorImgTapped;
            this.rectColor.PointerPressed += RectanglePointerPressed;
            this.thumbInnerEll.DragDelta += ThumbDragDelta;
            this.colorImg.PointerReleased += ColorImgPointerReleased;
            this.colorImg.PointerPressed += ColorImgPointerPressed;
            this.colorImg.PointerMoved += ColorImgPointerMoved;

            this.gdStop.Color = this.SelectedColor;
            this.finalColor.Fill = new SolidColorBrush(this.SelectedColor);

            GeneralTransform gt = this.pointer.TransformToVisual(reference);

            var p = new Point();
            p = gt.TransformPoint(p);
            this.px = p.X;
            this.py = p.Y;

            InitControl();
        }

        private async void InitControl()
        {
            Assembly assembly = typeof(ColorPicker).GetTypeInfo().Assembly;
            using (InMemoryRandomAccessStream res = new InMemoryRandomAccessStream())
            {
                using (var imgStream = assembly.GetManifestResourceStream("NoteOne_Core.Resources.ColorTemplate.png"))
                {
                    await imgStream.CopyToAsync(res.AsStreamForWrite());
                }

                res.Seek(0);
                BitmapImage bmp = new BitmapImage();

                bmp.SetSource(res);
                this.colorImg.Source = bmp;

                BitmapDecoder decoder = await BitmapDecoder.CreateAsync(res);
                this.bd = decoder;

                var pixelData = await decoder.GetPixelDataAsync(
                    BitmapPixelFormat.Bgra8,
                    BitmapAlphaMode.Straight,
                    new BitmapTransform() { ScaledHeight = 100, ScaledWidth = 100 },
                    ExifOrientationMode.IgnoreExifOrientation,
                    ColorManagementMode.DoNotColorManage);

                this.tempBuffer = pixelData.DetachPixelData();
            }
        }

        private void FillColor()
        {
            GeneralTransform gt = this.pointer.TransformToVisual(this.colorImg);
            Point p = gt.TransformPoint(p);
            int dx = (int)p.X;
            int dy = (int)p.Y;
            double k = (dy * 100 + dx) * 4;

            try
            {
                byte b = tempBuffer[(int)k + 0];
                byte g = tempBuffer[(int)k + 1];
                byte r = tempBuffer[(int)k + 2];
                byte a = tempBuffer[(int)k + 3];
                this.gdStop.Color = Color.FromArgb(0xff, r, g, b);

                var slelectedColor = LinearGdHelperClass.GetColorAtPoint(
                    this.rectColor, new Point(
                        (double)innerEll.GetValue(Canvas.LeftProperty) + 4, 
                        (double)innerEll.GetValue(Canvas.TopProperty) + 4));
                this.finalColor.Fill = new SolidColorBrush(slelectedColor);
                this.SelectedColor = slelectedColor;
            }
            catch (Exception ex)
            {
                ex.WriteLog();
            }
        }

        #endregion

        #region Embened Helper Class

        internal class LinearGdHelperClass
        {
            /// <summary>
            /// Calculates the color of a point in a rectangle that is filled with a LearGradientBrush.
            /// </summary>
            /// <param name="theRec"></param>
            /// <param name="thePoint"></param>
            /// <returns></returns>
            internal static Color GetColorAtPoint(Rectangle theRec, Point thePoint)
            {
                // Get properties    
                LinearGradientBrush br = (LinearGradientBrush)theRec.Fill;
                double y3 = thePoint.Y;
                double x3 = thePoint.X;
                double x1 = br.StartPoint.X * theRec.ActualWidth;
                double y1 = br.StartPoint.Y * theRec.ActualHeight;

                Point p1 = new Point(x1, y1);

                // Starting point     
                double x2 = br.EndPoint.X * theRec.ActualWidth;
                double y2 = br.EndPoint.Y * theRec.ActualHeight;
                Point p2 = new Point(x2, y2); // End point  

                // Calculate intersecting points     
                Point p4 = new Point();

                if (y1 == y2) // Horizontal case    
                {
                    p4 = new Point(x3, y1);
                }
                else if (x1 == x2) // Vertical case    
                {
                    p4 = new Point(x1, y3);
                }
                else // Diagnonal case    
                {
                    double m = (y2 - y1) / (x2 - x1);
                    double m2 = -1 / m;
                    double b = y1 - m * x1;
                    double c = y3 - m2 * x3;
                    double x4 = (c - b) / (m - m2);
                    double y4 = m * x4 + b;
                    p4 = new Point(x4, y4);
                }

                // Calculate distances relative to the vector start    
                double d4 = Dist(p4, p1, p2);
                double d2 = Dist(p2, p1, p2);
                double x = d4 / d2;

                // Clip the input if before or after the max/min offset values    
                double max = br.GradientStops.Max(n => n.Offset);
                if (x > max)
                {

                    x = max;

                }
                double min = br.GradientStops.Min(n => n.Offset);
                if (x < min)
                {
                    x = min;
                }

                // Find gradient stops that surround the input value    
                GradientStop gs0 = br.GradientStops.Where(n => n.Offset <= x).OrderBy(n => n.Offset).Last();
                GradientStop gs1 = br.GradientStops.Where(n => n.Offset >= x).OrderBy(n => n.Offset).First();
                float y = 0f;
                if (gs0.Offset != gs1.Offset)
                {
                    y = (float)((x - gs0.Offset) / (gs1.Offset - gs0.Offset));
                }

                // Interpolate color channels 
                Color cx = new Color();
                if (br.ColorInterpolationMode == ColorInterpolationMode.SRgbLinearInterpolation)
                {
                    byte aVal = (byte)((gs1.Color.A - gs0.Color.A) * y + gs0.Color.A);
                    byte rVal = (byte)((gs1.Color.R - gs0.Color.R) * y + gs0.Color.R);
                    byte gVal = (byte)((gs1.Color.G - gs0.Color.G) * y + gs0.Color.G);
                    byte bVal = (byte)((gs1.Color.B - gs0.Color.B) * y + gs0.Color.B);
                    cx = Color.FromArgb(aVal, rVal, gVal, bVal);
                }

                return cx;
            }

            /// <summary>
            /// Helper method for GetColorAtPoint
            /// Returns the signed magnitude of a point on a vector with origin po and pointing to pfprivate 
            /// </summary>
            /// <param name="px"></param>
            /// <param name="po"></param>
            /// <param name="pf"></param>
            /// <returns></returns>
            private static double Dist(Point px, Point po, Point pf)
            {
                double d = Math.Sqrt((px.Y - po.Y) * (px.Y - po.Y) + (px.X - po.X) * (px.X - po.X));
                if (((px.Y < po.Y) && (pf.Y > po.Y)) ||
                    ((px.Y > po.Y) && (pf.Y < po.Y)) ||
                    ((px.Y == po.Y) && (px.X < po.X) && (pf.X > po.X)) ||
                    ((px.Y == po.Y) && (px.X > po.X) && (pf.X < po.X)))
                {
                    d = -d;
                }

                return d;
            }
        }

        #endregion
    }
}