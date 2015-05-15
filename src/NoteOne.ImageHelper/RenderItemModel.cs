using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

#if WEB_SERVICE
namespace ChameHOT.WebService.Helpers.Models
{
#else
namespace NoteOne_ImageHelper
{
#endif

#if !WEB_SERVICE
    [DataContract]
#endif
    public class RenderItemModel
    {
        public RenderItemModel()
        {
            TextOption = new RenderingTextOption();
            ImageOption = new RenderingImageOption();
        }

#if !WEB_SERVICE
        [DataMember]
#endif
        public int Row { get; set; }
#if !WEB_SERVICE
        [DataMember]
#endif
        public int Column { get; set; }
#if !WEB_SERVICE
        [DataMember]
#endif
        public RenderType Type { get; set; }
#if !WEB_SERVICE
        [DataMember]
#endif
        public string Content { get; set; }
#if !WEB_SERVICE
        [DataMember]
#endif
        public RenderingTextOption TextOption { get; set; }
#if !WEB_SERVICE
        [DataMember]
#endif
        public RenderingImageOption ImageOption { get; set; }
    }

    #region Enums and Values
    public enum RenderType
    {
        /// <summary>
        ///     Render Text
        /// </summary>
        Text = 1,

        /// <summary>
        ///     Render Image
        /// </summary>
        Image = 2,

        /// <summary>
        ///     Render Shape
        /// </summary>
        Shape = 3
    }

    // Summary:
    //     Describes the thickness of a frame around a rectangle. Four System.Double
    //     values describe the Windows.UI.Xaml.Thickness.Left, Windows.UI.Xaml.Thickness.Top,
    //     Windows.UI.Xaml.Thickness.Right, and Windows.UI.Xaml.Thickness.Bottom sides
    //     of the rectangle, respectively.
    public struct Thickness
    {
        //
        // Summary:
        //     Initializes a Windows.UI.Xaml.Thickness structure that has the specified
        //     uniform length on each side.
        //
        // Parameters:
        //   uniformLength:
        //     The uniform length applied to all four sides of the bounding rectangle.
        public Thickness(double uniformLength)
        {
            this.Left = this.Top = this.Right = this.Bottom = uniformLength;
        }

        //
        // Summary:
        //     Initializes a Windows.UI.Xaml.Thickness structure that has specific lengths
        //     (supplied as a System.Double) applied to each side of the rectangle.
        //
        // Parameters:
        //   left:
        //     The thickness for the left side of the rectangle.
        //
        //   top:
        //     The thickness for the upper side of the rectangle.
        //
        //   right:
        //     The thickness for the right side of the rectangle
        //
        //   bottom:
        //     The thickness for the lower side of the rectangle.
        public Thickness(double left, double top, double right, double bottom)
        {
            this.Left = left;
            this.Top = top;
            this.Right = right;
            this.Bottom = bottom;
        }

        public double Bottom;
        public double Left;
        public double Right;
        public double Top;
    }

    #endregion
}
