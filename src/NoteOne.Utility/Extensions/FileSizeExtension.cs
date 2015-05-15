using System;

namespace NoteOne_Utility.Extensions
{
    public static class FileSizeExtension
    {
        public static string ToFileSize(this ulong size, int d = 2)
        {
            return ConvertToFileSize(size, d);
        }

        public static string ToFileSize(this long size, int d = 2)
        {
            return ConvertToFileSize((ulong) size, d);
        }

        public static string ToFileSize(this int size, int d = 2)
        {
            return ConvertToFileSize((ulong) size, d);
        }

        public static string ToFileSize(this uint size, int d = 2)
        {
            return ConvertToFileSize(size, d);
        }

        private static string ConvertToFileSize(ulong size, int d)
        {
            if (size <= 0) return "0";
            var units = new string[5] {"B", "KB", "MB", "GB", "TB"};
            var digitGroups = (ulong) (Math.Log10(size)/Math.Log10(1024));
            return (size/Math.Pow(1024, digitGroups)).ToString("N" + d) + " " + units[digitGroups];
        }
    }
}