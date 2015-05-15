using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace NoteOne_Utility.Converters
{
    public sealed class BooleanToAlignmentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (parameter != null &&
                (parameter.ToString().ToUpper() == "V" || parameter.ToString().ToUpper() == "Vertical"))
                return (value is bool && (bool)value) ? VerticalAlignment.Top : VerticalAlignment.Bottom;
            else
                return (value is bool && (bool)value) ? HorizontalAlignment.Left : HorizontalAlignment.Right;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (parameter != null &&
                (parameter.ToString().ToUpper() == "V" || parameter.ToString().ToUpper() == "Vertical"))
                return value is VerticalAlignment && (VerticalAlignment)value == VerticalAlignment.Top;
            else
                return value is HorizontalAlignment && (HorizontalAlignment)value == HorizontalAlignment.Left;
        }
    }
}
