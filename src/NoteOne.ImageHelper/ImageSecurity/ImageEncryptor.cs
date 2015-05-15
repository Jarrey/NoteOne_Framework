using System;
using Windows.UI;
using Windows.UI.Xaml.Media.Imaging;
using WColor = Windows.UI.Color;

namespace NoteOne_ImageHelper.ImageSecurity
{
    public class ImageEncryptor
    {
        #region Public static methods
        
        /// <summary>
        ///     Encryt image with image code
        /// </summary>
        /// <param name="image"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public static WriteableBitmap EncrytImage(WriteableBitmap image, ImageAppCode code)
        {
            if (image == null) throw new ArgumentNullException("image");

            WColor cleanColor = WColor.FromArgb(0, 0, 0, 0);

            int offset = code.CodePositionOffset;
            SetPixelColor(image, 0 + offset, 0, 0xff, code.Code1, 0, 0);
            SetPixelColor(image, image.PixelWidth - 1, 0 + offset, 0xff, 0, code.Code2, 0);
            SetPixelColor(image, image.PixelWidth - 1 - offset, image.PixelHeight - 1, 0xff, 0, 0, code.Code3);
            SetPixelColor(image, 0, image.PixelHeight - 1 - offset, 0xff, code.Code4, 0, 0);

            return image;
        }

        /// <summary>
        ///     Dencryt image and return image code
        /// </summary>
        /// <param name="image"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static ImageAppCode DencrytImage(WriteableBitmap image, int offset)
        {
            if (image == null) throw new ArgumentNullException("image");
            if (offset < 0 || offset > Math.Min(image.PixelHeight, image.PixelWidth))
                throw new ArgumentException("Argument is out of range", "offset");

            WColor color1 = GetPixelColor(image, 0 + offset, 0);
            WColor color2 = GetPixelColor(image, image.PixelWidth - 1, 0 + offset);
            WColor color3 = GetPixelColor(image, image.PixelWidth - 1 - offset, image.PixelHeight - 1);
            WColor color4 = GetPixelColor(image, 0, image.PixelHeight - 1 - offset);

            return new ImageAppCode(color1.R, color2.G, color3.B, color4.R, offset);
        }

        #endregion

        #region Private helper methods

        private static WColor GetPixelColor(WriteableBitmap image, int x, int y)
        {
            using (BitmapContext context = image.GetBitmapContext())
            {
                int c = context.Pixels[y * context.Width + x];
                var a = (byte)(c >> 24);
                return WColor.FromArgb(a,
                                      (byte)((c >> 16) & 0xFF),
                                      (byte)((c >> 8) & 0xFF),
                                      (byte)(c & 0xFF));
            }
        }

        private static void SetPixelColor(WriteableBitmap image, int x, int y, byte a, byte r, byte g, byte b)
        {
            using (BitmapContext context = image.GetBitmapContext())
            {
                context.Pixels[y * context.Width + x] = (a << 24) | (r << 16) | (g << 8) | b;
            }
        }

        #endregion
    }
}