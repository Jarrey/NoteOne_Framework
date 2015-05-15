using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using NoteOne_Utility.Converters;
using NoteOne_Utility.Helpers;
using Windows.Storage;
using Windows.Storage.Search;
using Windows.Storage.Streams;

namespace NoteOne_Utility.Extensions
{
    public static class LogExtension
    {
        private static List<Task> tasks = new List<Task>();
        private static bool isInitilized;

        public static string GetLog<T>(this T p)
        {
            if (p is Exception)
            {
                var ex = p as Exception;
                string innerException = "Null";
                if (ex.InnerException != null) innerException = ex.InnerException.GetLog();
                return
                    string.Format(
                        "\tType: {0}\r\n\tTime: {1}\r\n\tMessage: {2}\r\n\tHResult: {3}\r\n\tSource: {4}\r\n\tInner Exception: [\r\n{5}\r\n]\r\n\tStack: {6}\r\n\tHelp Link: {7}",
                        ex.GetType().FullName,
                        DateTime.Now.ToString(),
                        ex.Message,
                        ex.HResult,
                        ex.Source,
                        innerException,
                        ex.StackTrace,
                        ex.HelpLink);
            }
            else
                return p.ToString();
        }

        public static async void WriteLog<T>(this T log,
                                             LogType type = LogType.Exception,
                                             [CallerMemberName] string callerName = null,
                                             [CallerFilePath] string callerPath = null,
                                             [CallerLineNumber] int callerLineNumber = 0)
        {
#if !DEBUG
            if (type == LogType.Debug) return;
#endif

            var logBuilder = new StringBuilder();
            logBuilder.AppendLine("\n=====================================================\n");
            logBuilder.AppendLine(DateTime.Now.ToString());
#if DEBUG
            logBuilder.AppendLine(string.Format("Caller File Path: {0}", callerPath));
            logBuilder.AppendLine(string.Format("Caller Line Number: {0}", callerLineNumber));
#endif
            logBuilder.AppendLine(string.Format("Caller Name: {0}", callerName));
            logBuilder.AppendLine(string.Format("{0} :\r\n\t{1}", type.GetDescription(), log.GetLog()));

            await Logger.CurrentLogger.WriteToLogFile(type, logBuilder.ToString());
        }

        public static async Task InitializeLogger()
        {
            if (isInitilized == false)
            {
                await Logger.CurrentLogger.InitializeLogger();
                isInitilized = true;
            }
        }

        public static void DestroyLogger()
        {
            if (isInitilized)
            {
                Logger.CurrentLogger.DestroyLogger();
                isInitilized = false;
            }
        }

        internal class Logger
        {
            private static Logger _currentLogger;
            private bool _isCleaned;

            private Logger()
            {
                LogDataWriters = new Dictionary<string, DataWriter>();
            }

            public static Logger CurrentLogger
            {
                get
                {
                    if (_currentLogger == null) _currentLogger = new Logger();
                    return _currentLogger;
                }
            }

            private Dictionary<string, DataWriter> LogDataWriters { get; set; }

            public async Task InitializeLogger()
            {
                StorageFolder logFolder =
                    await
                    ApplicationData.Current.LocalFolder.CreateFolderAsync("log", CreationCollisionOption.OpenIfExists);

                if (!_isCleaned)
                {
                    await CleanHistoryLogfiles(logFolder);
                    _isCleaned = true;
                }

                foreach (string logTypeName in Enum.GetNames(typeof(LogType)))
                {
                    string logFileName = GenerateLogFileName(logTypeName);
                    if (!LogDataWriters.ContainsKey(logFileName))
                    {
                        StorageFile logFile =
                            await logFolder.CreateFileAsync(logFileName, CreationCollisionOption.OpenIfExists);
                        IRandomAccessStream stream = await logFile.OpenAsync(FileAccessMode.ReadWrite);
                        IOutputStream outStream = stream.GetOutputStreamAt(stream.Size);
                        var dw = new DataWriter(outStream);
                        LogDataWriters[logFileName] = dw;
                    }
                }
            }

            public void DestroyLogger()
            {
                foreach (var logDataWriter in LogDataWriters)
                    logDataWriter.Value.Dispose();
                LogDataWriters.Clear();
            }

            public async Task WriteToLogFile(LogType type, string log)
            {
                string logFileName = GenerateLogFileName(Enum.GetName(typeof(LogType), type));
                if (!LogDataWriters.ContainsKey(logFileName)) return;
                else
                {
                    DataWriter dw = LogDataWriters[logFileName];

                    using (AsyncLock.Releaser releaser = await ConstKeys.LOG_FILE_WRITE_ASYNC_LOCKER.LockAsync())
                    {
                        dw.WriteString(log);
                        await dw.FlushAsync();
                        await dw.StoreAsync();
                    }
                }
            }

            private async Task CleanHistoryLogfiles(StorageFolder logFolder)
            {
                StorageFileQueryResult queryResult = logFolder.
                    CreateFileQueryWithOptions(
                        new QueryOptions(CommonFileQuery.OrderByName, new List<string> { ".log" })
                            {
                                FolderDepth = FolderDepth.Shallow
                            });
                IReadOnlyList<StorageFile> fileList = await queryResult.GetFilesAsync();
                for (int i = 0; i < fileList.Count; i++)
                {
                    if (DateTimeOffset.Now.Subtract(fileList[i].DateCreated).Days >
                        AppSettings.Instance[AppSettings.GLOBAL_SETTING_LOG_HISTORY].ToString().StringToInt())
                        await fileList[i].DeleteAsync();
                }
            }

            private string GenerateLogFileName(string logTypeName)
            {
                return DateTimeOffset.Now.Date.ToString("yyyyMMdd_") + logTypeName + ".log";
            }
        }
    }
}