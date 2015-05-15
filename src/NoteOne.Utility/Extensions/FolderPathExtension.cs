namespace NoteOne_Utility.Extensions
{
    public static class FolderPathExtension
    {
        public static string GetUniqueFolderPath(this string path)
        {
            return path.Replace('/', '_').Replace('\\', '_').Replace(':', '_');
        }
    }
}