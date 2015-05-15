using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using NoteOne_Core.Common;
using NoteOne_Utility.Extensions;
using Windows.ApplicationModel.Background;
using Windows.Data.Xml.Dom;
using Windows.Storage;
using Windows.UI.ViewManagement;

namespace NoteOne_Core
{
    public class BackgroundTaskService : BindableBase
    {
        static BackgroundTaskService()
        {
            Services = new ObservableCollection<Service>();
        }

        public BackgroundTaskService(Service service, XmlElement configXml, string backgroundTaskSettingFileName)
        {
            if (!Services.Contains(service))
                Services.Add(service);
            Service = service;
            BackgroundTaskSettingFileName = backgroundTaskSettingFileName;

            Initialize(configXml);
        }

        #region Properties

        public string Name { get; private set; }
        public string EntryPoint { get; private set; }
        public string Parameter { get; private set; }
        public Service Service { get; private set; }
        public string BackgroundTaskSettingFileName { get; private set; }

        public bool ServiceStatus
        {
            get { return BackgroundTaskController.GetBackgroundTaskStatus(Name); }
        }

        public BackgroundTaskRegistration Task { get; private set; }

        #endregion

        public static ObservableCollection<Service> Services { get; private set; }
        public Func<Dictionary<string, string>, Task> DoAsync { get; protected set; }

        protected virtual void Initialize(XmlElement configXml)
        {
            try
            {
                EntryPoint = configXml.GetAttribute("EntryPoint").CheckAndThrow();
                Name = configXml.GetAttribute("Name").CheckAndThrow();
            }
            catch (Exception ex)
            {
                ex.WriteLog();
            }
        }

        public async void InitializeBackgroundTask(IBackgroundTrigger trigger, IBackgroundCondition condition,
                                                   string parameter = "")
        {
#if WIN8
            // can not show Popup in snapped view
            if (ApplicationView.Value == ApplicationViewState.Snapped &&
                !ApplicationView.TryUnsnap())
            {
                throw new Exception("Cannot unsnap the view");
            }
#endif
            // for catching exception throw in Similator
            try
            {
#if WIN8
                await BackgroundExecutionManager.RequestAccessAsync();
#else
                var backgroundAccessStatus = await BackgroundExecutionManager.RequestAccessAsync();
                if (backgroundAccessStatus == BackgroundAccessStatus.AllowedMayUseActiveRealTimeConnectivity ||
                    backgroundAccessStatus == BackgroundAccessStatus.AllowedWithAlwaysOnRealTimeConnectivity)
                {
#endif
                    // fix register multi background tasks in system
                    // if (BackgroundTaskController.GetBackgroundTaskStatus(Name))
                    BackgroundTaskController.UnregisterBackgroundTasks(Name);
                    Task = BackgroundTaskController.RegisterBackgroundTask(EntryPoint, Name, trigger, condition);
                    this.OnPropertyChanged("ServiceStatus");

                    await StoreBackgroundTaskSettingAsync(parameter);
#if !WIN8
                }
#endif
            }
            catch (Exception ex)
            {
                ex.WriteLog();
            }
        }

        public void UnregisterBackgroundTask()
        {
#if WIN8
            // can not show Popup in snapped view
            if (ApplicationView.Value == ApplicationViewState.Snapped &&
                !ApplicationView.TryUnsnap())
            {
                throw new Exception("Cannot unsnap the view");
            }
#endif
            BackgroundTaskController.UnregisterBackgroundTasks(Name);
            this.OnPropertyChanged("ServiceStatus");
        }

        private async Task StoreBackgroundTaskSettingAsync(string parameter = "")
        {
            StorageFile settingFile =
                await
                ApplicationData.Current.LocalFolder.CreateFileAsync(BackgroundTaskSettingFileName,
                                                                    CreationCollisionOption.ReplaceExisting);

            var xmlSetting = new XmlDocument();

            XmlDocumentFragment xmlDocumentFragment = xmlSetting.CreateDocumentFragment();
            XmlElement rootElement = xmlSetting.CreateElement("BackgroundTaskServices");
            xmlDocumentFragment.AppendChild(rootElement);

            XmlElement serviceElement = xmlSetting.CreateElement("BackgroundTaskService");
            serviceElement.SetAttribute("ServiceType", Service.ServiceType.ToString());
            serviceElement.SetAttribute("ServiceChannel", Service.ServiceChannel.ID.ToString());
            serviceElement.SetAttribute("Service", Service.ID.ToString());
            serviceElement.SetAttribute("Parameter", parameter);

            serviceElement.AppendChild(xmlSetting.CreateTextNode(Service.ServiceChannel.ConfigXml.GetXml()));
            rootElement.AppendChild(serviceElement);
            xmlSetting.AppendChild(rootElement);

            await xmlSetting.SaveToFileAsync(settingFile);
        }
    }
}