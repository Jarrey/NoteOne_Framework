using System;
using System.Threading.Tasks;
using NoteOne_Utility.Extensions;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;

namespace NoteOne_Utility.Converters
{
    public class LocalFileToBitmapImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string path = value.ToString();
            try
            {
                if (new Uri(path).Scheme == "file")
                {
                    Task<BitmapImage> t = GetBitmapImageAsync(path);
                    t.Wait();
                    return t.Result;
                }
            }
            catch (Exception ex)
            {
                ex.WriteLog();
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }

        private async Task<BitmapImage> GetBitmapImageAsync(string path)
        {
            var bitmapImage = new BitmapImage();
            StorageFile file = await StorageFile.GetFileFromPathAsync(path);
            using (IRandomAccessStream s = await file.OpenAsync(FileAccessMode.Read))
            {
                bitmapImage.SetSource(s);
            }
            return bitmapImage;
        }
    }
}