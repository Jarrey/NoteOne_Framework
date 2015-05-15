using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Graphics.Display;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace NoteOne_ImageHelper
{
    public static class WriteableBitmapHelper
    {
        public static async Task<IRandomAccessStream> ToRandomAccessStreamAsync(this WriteableBitmap image)
        {
            var stream = new InMemoryRandomAccessStream();
            BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);
#if !WIN8
            encoder.SetPixelData(BitmapPixelFormat.Bgra8,
                                 BitmapAlphaMode.Premultiplied,
                                 (uint) image.PixelWidth,
                                 (uint) image.PixelHeight,
                                 DisplayInformation.GetForCurrentView().LogicalDpi,
                                 DisplayInformation.GetForCurrentView().LogicalDpi,
                                 image.ToByteArray());
#else
            encoder.SetPixelData(BitmapPixelFormat.Bgra8,
                                 BitmapAlphaMode.Premultiplied,
                                 (uint) image.PixelWidth,
                                 (uint) image.PixelHeight,
                                 DisplayProperties.LogicalDpi, // DisplayProperties is obsoleted
                                 DisplayProperties.LogicalDpi,
                                 image.ToByteArray());
#endif
            await encoder.FlushAsync();
            stream.Seek(0);
            return stream;
        }

        public static async Task<Stream> ToStreamAsync(this WriteableBitmap image)
        {
            return (await ToRandomAccessStreamAsync(image)).AsStream();
        }
    }
}