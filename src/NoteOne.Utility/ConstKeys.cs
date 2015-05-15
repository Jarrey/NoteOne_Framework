using NoteOne_Utility.Helpers;

namespace NoteOne_Utility
{
    public class ConstKeys
    {
        public const string NETWORK_MONITOR_KEY = "NetworkMonitoeKey";
        public const string HELP_KEY = "HelpKey";

        public static readonly AsyncLock LOG_FILE_WRITE_ASYNC_LOCKER = new AsyncLock();
        public static readonly AsyncLock SETTING_FILE_WRITE_ASYNC_LOCKER = new AsyncLock();
    }
}