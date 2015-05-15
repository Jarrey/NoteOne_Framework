using System;
using System.Threading.Tasks;
using NoteOne_Utility.Extensions;
using Windows.Storage;
using Windows.Storage.Streams;
using Buffer = Windows.Storage.Streams.Buffer;

namespace NoteOne_Utility.Helpers
{
    public class ApplicationHelper
    {
        #region App Global Functions

        /// <summary>
        ///     Store the Application Backgound Image to Roaming folder
        /// </summary>
        /// <param name="imageUrl">The Image URL</param>
        /// <returns></returns>
        public static async Task StoreImageAsync(string imageUrl, StorageFile file)
        {
            try
            {
                var uri = new Uri(imageUrl);
                if (uri.Scheme == "file")
                {
                    StorageFile imageFile = await StorageFile.GetFileFromPathAsync(imageUrl);
                    await imageFile.CopyAndReplaceAsync(file);
                }
                else
                {
                    RandomAccessStreamReference stream = RandomAccessStreamReference.CreateFromUri(uri);
                    using (IRandomAccessStream randomAccessStream = await file.OpenAsync(FileAccessMode.ReadWrite))
                    {
                        Buffer buffer;
                        using (IRandomAccessStream originalImageStream = await stream.OpenReadAsync())
                        {
                            buffer = new Buffer((uint) originalImageStream.Size);
                            await
                                originalImageStream.ReadAsync(buffer, (uint) originalImageStream.Size,
                                                              InputStreamOptions.None);
                        }
                        await randomAccessStream.WriteAsync(buffer);
                        await randomAccessStream.FlushAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                ex.WriteLog();
            }
        }

        #endregion
    }
}