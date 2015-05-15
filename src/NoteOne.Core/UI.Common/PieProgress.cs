using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

// The Templated Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234235

namespace NoteOne_Core.UI.Common
{
    public sealed class PieProgress : Control
    {
        public PieProgress()
        {
            this.DefaultStyleKey = typeof(PieProgress);

            this.Loaded += (o, e) => { drawSector(Value); };
            this.SizeChanged += (o, e) => { this.Height = this.Width; drawSector(Value); };
        }

        private Path path;
        private TextBlock label;

        #region Dependency Properties

        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set
            {
                value = Math.Truncate(value * 100) / 100;
                if (value > 100) value = 100; else if (value < 0) value = 0;
                SetValue(ValueProperty, value);
            }
        }
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double), typeof(PieProgress), new PropertyMetadata(0.0, (o, e) =>
            {
                var obj = o as PieProgress;
                if (obj != null)
                {
                    obj.drawSector((double)e.NewValue);
                }
            }));

        public string LabelFormat
        {
            get { return (string)GetValue(LabelFormatProperty); }
            set { SetValue(LabelFormatProperty, value); }
        }
        public static readonly DependencyProperty LabelFormatProperty =
            DependencyProperty.Register("LabelFormat", typeof(string), typeof(PieProgress), new PropertyMetadata("{0:N0} %"));

        public Brush PieForeground
        {
            get { return (Brush)GetValue(PieForegroundProperty); }
            set { SetValue(PieForegroundProperty, value); }
        }
        public static readonly DependencyProperty PieForegroundProperty =
            DependencyProperty.Register("PieForeground", typeof(Brush), typeof(PieProgress), new PropertyMetadata(new SolidColorBrush(Colors.White)));

        public Brush PieBackground
        {
            get { return (Brush)GetValue(PieBackgroundProperty); }
            set { SetValue(PieBackgroundProperty, value); }
        }
        public static readonly DependencyProperty PieBackgroundProperty =
            DependencyProperty.Register("PieBackground", typeof(Brush), typeof(PieProgress), new PropertyMetadata(new SolidColorBrush(Colors.Gray)));

        #endregion

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            path = GetTemplateChild("path") as Path;
            if (path == null)
                throw new Exception("Can not find path content in this control template");

            label = GetTemplateChild("label") as TextBlock;
            if (label == null)
                throw new Exception("Can not find label content in this control template");
        }

        /// <summary>
        /// this method draws the sector based on the value
        /// </summary>
        /// <param name="value"></param>
        private void drawSector(double value)
        {
            /*
             * The circular pie consists of three parts
             * a. the background
             * b. the pie / sector
             * c. the hole
             * 
             * The path object in the user control will be used to
             * create the pie/sector. Ellipses are used to create the other
             * two. This method draws a sector of a circle based on the value
             * passed into this function.
             * 
             * A sector has three parts -
             * a. a line segment from center to circumfrence
             * b. Arcs - parts of the circumfrence
             * c. a line segment from circumfrence to center
             */

            if (path == null) return;
            if (label == null) return;

            // Clean the path
            path.SetValue(Path.DataProperty, null);
            PathGeometry pg = new PathGeometry();
            PathFigure fig = new PathFigure();

            // if the value is zero, do nothing further
            if (value == 0) return;

            // a few variables for good measure
            double height = this.ActualHeight;
            double width = this.ActualWidth;
            double radius = height / 2;
            double theta = (360 * value / 100) - 90; // <--- the coordinate system is flipped with 0,0 at top left. Hence the -90
            double xC = radius;
            double yC = radius;

            // this is to ensure that the fill is complete, otherwise a slight
            // gap is left in the fill of the sector. By incrementing it by one
            // we fill that gap as the angle is now 361 deg (-90 for the coord sys inversion)
            if (value == 1) theta += 1;

            // finalPoint represents the point along the circumfrence after
            // which the sector ends and the line back to the center begins
            // use parametric equations for a circule to figure that out
            double finalPointX = xC + (radius * Math.Cos(theta * 0.0174)); // <--- PI / 180 = 0.0174
            double finalPointY = yC + (radius * Math.Sin(theta * 0.0174));

            // Now we have calculated all the points we need to start drawing the
            // segments for the figure. Drawing a figure in WPF is like using a pencil
            // without lifting the tip. Each segment specifies an end point and
            // is connected to the previous segment's end point. 
            fig.StartPoint = new Point(radius, radius); // start by placing the pencil's tip at the center of the circle
            LineSegment firstLine = new LineSegment();  // the first line segment goes vertically upwards (radius,0)
            firstLine.Point = new Point(radius, 0);     // draw that line segment
            fig.Segments.Add(firstLine);

            // Now we have to draw the arc for this sector. The way drawing works in wpf,
            // in order to get a proper and coherent circumfrence, we need to draw separate
            // arcs everytime the sector exceeds a quarter of the circle. 
            if (value > 25)
            {
                ArcSegment firstQuart = new ArcSegment();
                firstQuart.Point = new Point(width, radius);
                firstQuart.SweepDirection = SweepDirection.Clockwise;
                firstQuart.Size = new Size(radius, radius);
                fig.Segments.Add(firstQuart);
            }

            if (value > 50)
            {
                ArcSegment secondQuart = new ArcSegment();
                secondQuart.Point = new Point(radius, height);
                secondQuart.SweepDirection = SweepDirection.Clockwise;
                secondQuart.Size = new Size(radius, radius);
                fig.Segments.Add(secondQuart);
            }

            if (value > 75)
            {
                ArcSegment thirdQuart = new ArcSegment();
                thirdQuart.Point = new Point(0, radius);
                thirdQuart.SweepDirection = SweepDirection.Clockwise;
                thirdQuart.Size = new Size(radius, radius);
                fig.Segments.Add(thirdQuart);
            }

            ArcSegment finalQuart = new ArcSegment();
            finalQuart.Point = new Point(finalPointX, finalPointY);
            finalQuart.SweepDirection = SweepDirection.Clockwise;
            finalQuart.Size = new Size(radius, radius);
            fig.Segments.Add(finalQuart);

            // Now one line segment, arcs for the circumfrence - all that is done
            // let's draw a line segment from the end of the sector/arcs back to the
            // center of the circle
            LineSegment lastLine = new LineSegment();
            lastLine.Point = new Point(radius, radius);
            fig.Segments.Add(lastLine);

            // Now take these figures and add this to the path geometry
            // then add the path geometry to the path's data property
            pg.Figures.Add(fig);
            path.SetValue(Path.DataProperty, pg);

            // set label
            label.Text = String.Format(LabelFormat, value);
        }
    }
}
