using System;
using Windows.UI.Xaml.Data;

namespace NoteOne_Utility.Converters
{
    public class DateTimeToStringFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var datetime = (DateTime) value;
            if (parameter != null)
                return datetime.ToString((string) parameter);
            else
                return datetime.ToString(@"yyyy-MM-dd HH:mm:ss.fff zz");
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}