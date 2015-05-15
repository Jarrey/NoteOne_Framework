using System.Threading.Tasks;
using Windows.Storage.Streams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoteOne_Utility.Extensions
{
    public static class RandomAccessStreamExtension
    {
        public static async Task<byte[]> GetBytes(this IRandomAccessStream stream)
        {
            using (var reader = new DataReader(stream.GetInputStreamAt(0)))
            {
                var bytes = new byte[stream.Size];
                await reader.LoadAsync((uint)stream.Size);
                reader.ReadBytes(bytes);
                return bytes;
            }
        }
    }
}
