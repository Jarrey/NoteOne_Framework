using System;
using System.Threading.Tasks;
using NoteOne_Utility.Converters;
using NoteOne_Utility.Extensions;
using NoteOne_Utility.Helpers;
using Windows.Data.Xml.Dom;
using Windows.Storage;
using System.Globalization;

namespace NoteOne_Utility
{
    public enum SettingType
    {
        Local = 1,
        Roaming = 2
    }

    public interface IAppSettings
    {
        string SettingFileName { get; }
        SettingType Type { get; }
        StorageFolder SettingFolder { get; }
        ObservableDictionary<string, object> Settings { get; }

        void Reset();
    }

    public class AppSettings : IAppSettings
    {
        public static async Task InitializeSettings(IAppSettings settingInstance)
        {
            try
            {
                StorageFolder settingFolder = settingInstance.SettingFolder;
                StorageFile appSettingsFile = null;
                if (await settingFolder.CheckFileExisted(settingInstance.SettingFileName))
                {
                    using (var asyncLocker = await ConstKeys.SETTING_FILE_WRITE_ASYNC_LOCKER.LockAsync())
                    {
                        appSettingsFile = await settingFolder.GetFileAsync(settingInstance.SettingFileName);
                        XmlDocument xmlSetting = await XmlDocument.LoadFromFileAsync(appSettingsFile);

                        foreach (XmlElement element in xmlSetting.SelectNodes("/AppSettings/Setting"))
                        {
                            string keyName = element.GetAttribute("KeyName");
                            if (settingInstance.Settings.ContainsKey(keyName))
                            {
                                string typeName = element.GetAttribute("Type");
                                if (typeName == typeof(int).FullName)
                                    settingInstance.Settings[element.GetAttribute("KeyName")] =
                                        element.GetAttribute("Value").ToString(CultureInfo.InvariantCulture).StringToInt();
                                else if (typeName == typeof(double).FullName)
                                    settingInstance.Settings[element.GetAttribute("KeyName")] =
                                        element.GetAttribute("Value").ToString(CultureInfo.InvariantCulture).StringToDouble();
                                else if (typeName == typeof(float).FullName)
                                    settingInstance.Settings[element.GetAttribute("KeyName")] =
                                        element.GetAttribute("Value").ToString(CultureInfo.InvariantCulture).StringToFloat();
                                else if (typeName == typeof(bool).FullName)
                                    settingInstance.Settings[element.GetAttribute("KeyName")] =
                                        element.GetAttribute("Value").StringToBoolean();
                                else if (typeName == typeof(string).FullName)
                                    settingInstance.Settings[element.GetAttribute("KeyName")] = element.GetAttribute("Value");
                            }
                        }
                    }
                }
                await SaveSettings(settingInstance);
            }
            catch (Exception ex)
            {
                ex.WriteLog();
            }
        }

        public static async Task SaveSettings(IAppSettings settingInstance)
        {
            StorageFolder settingFolder = settingInstance.SettingFolder;
            StorageFile appSettingsFile = null;
            using (var asyncLocker = await ConstKeys.SETTING_FILE_WRITE_ASYNC_LOCKER.LockAsync())
            {
                if (await settingFolder.CheckFileExisted(settingInstance.SettingFileName))
                    appSettingsFile = await settingFolder.GetFileAsync(settingInstance.SettingFileName);
                else
                    //InitializeAppSettings, create the setting file and set the default values
                    appSettingsFile = await settingFolder.CreateFileAsync(settingInstance.SettingFileName);
            }

            var xmlSetting = new XmlDocument();
            XmlDocumentFragment xmlDocumentFragment = xmlSetting.CreateDocumentFragment();
            XmlElement rootElement = xmlSetting.CreateElement("AppSettings");
            xmlDocumentFragment.AppendChild(rootElement);

            foreach (var setting in settingInstance.Settings)
            {
                rootElement.AppendChild(CreateSettingsElement(xmlSetting, setting.Key, setting.Value));
            }

            xmlSetting.AppendChild(rootElement);
            using (var asyncLocker = await ConstKeys.SETTING_FILE_WRITE_ASYNC_LOCKER.LockAsync())
            {
                await xmlSetting.SaveToFileAsync(appSettingsFile);
            }
        }

        private static XmlElement CreateSettingsElement(XmlDocument xmlDoc, string keyName, object value)
        {
            XmlElement settingElement = xmlDoc.CreateElement("Setting");
            settingElement.SetAttribute("KeyName", keyName);
            settingElement.SetAttribute("Value", value.ToString(CultureInfo.InvariantCulture));
            settingElement.SetAttribute("Type", value.GetType().FullName);
            return settingElement;
        }

        #region For instance

        private static AppSettings _instance;

        // For instance
        private AppSettings()
        {
            Settings = new ObservableDictionary<string, object>();
            Reset();
        }

        public void Reset()
        {
            // Create Settings for AppSettings
            Settings[GLOBAL_SETTING_LOG_HISTORY] = 5; // Default log history time, unit is DAY
            Settings[GLOBAL_INTERNET_STATUS] = true;
            Settings[GLOBAL_NETWORK_TIMEOUT] = 30000; // 30 seconds timeout in default
            Settings[GLOBAL_NETWORK_RETRY_TIMES] = 3; // Retry times, default value is 3 times

            // User agents
            Settings[GLOBAL_USER_AGENT_CHROME] = @"Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/30.0.1599.101 Safari/537.36";
            Settings[GLOBAL_USER_AGENT_IE] = @"Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.1; WOW64; Trident/6.0)";
            Settings[GLOBAL_USER_AGENT_FIREFOX] = @"Mozilla/5.0 (Windows NT 6.1; WOW64; rv:22.0) Gecko/20100101 Firefox/22.0";
        }

        #region Setting fields

        public const string GLOBAL_SETTING_LOG_HISTORY = "GLOBAL_SETTING_LOG_HISTORY";
        public const string GLOBAL_INTERNET_STATUS = "GLOBAL_INTERNET_STATUS";
        public const string GLOBAL_NETWORK_TIMEOUT = "GLOBAL_NETWORK_TIMEOUT";
        public const string GLOBAL_NETWORK_RETRY_TIMES = "GLOBAL_NETWORK_RETRY_TIMES";

        // User agents
        public const string GLOBAL_USER_AGENT_CHROME = "GLOBAL_USER_AGENT_CHROME";
        public const string GLOBAL_USER_AGENT_IE = "GLOBAL_USER_AGENT_IE";
        public const string GLOBAL_USER_AGENT_FIREFOX = "GLOBAL_USER_AGENT_FIREFOX";

        #endregion

        public static AppSettings Instance
        {
            get
            {
                if (_instance == null) _instance = new AppSettings();
                return _instance;
            }
        }

        public object this[string keyName]
        {
            get
            {
                if (Settings.ContainsKey(keyName)) return Settings[keyName];
                else
                    return null;
            }
            set { if (Settings.ContainsKey(keyName)) Settings[keyName] = value; }
        }

        public string SettingFileName
        {
            get { return "AppSettings.setting"; }
        }

        public SettingType Type
        {
            get { return SettingType.Roaming; }
        }

        public StorageFolder SettingFolder
        {
            get { return ApplicationData.Current.RoamingFolder; }
        }

        public ObservableDictionary<string, object> Settings { get; private set; }

        #endregion
    }
}