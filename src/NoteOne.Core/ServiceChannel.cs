using System;
using System.Collections.ObjectModel;
using System.Linq;
using NoteOne_Core.Common.Models;
using NoteOne_Core.Interfaces;
using NoteOne_Utility.Converters;
using NoteOne_Utility.Extensions;
using Windows.Data.Xml.Dom;

namespace NoteOne_Core
{
    public class ServiceChannel : IServiceChannel, IVariableSizedWrapGridStyle
    {
        public ServiceChannel(XmlElement configXml)
        {
            if (Services == null) Services = new ObservableCollection<IService>();

            ConfigXml = configXml;
            InitializeServiceChannel(configXml);

            // Register into ServiceChannelManager
            ServiceChannelManager.CurrentServiceChannelManager.Register(this);
        }

        public XmlElement ConfigXml { get; private set; }
        public Guid ID { get; private set; }
        public string Name { get; private set; }
        public string ShortName { get; private set; }
        public bool IsEnabled { get; protected set; }
        public ObservableCollection<IService> Services { get; private set; }

        #region Virtual Functions

        public virtual void InitializeLogo()
        {
        }

        protected virtual void InitializeServiceChannel(XmlElement configXml)
        {
            try
            {
                ID = new Guid(configXml.GetAttribute("ID")).CheckAndThrow();
                Name = configXml.GetAttribute("Name").CheckAndThrow();
                ShortName = configXml.GetAttribute("ShortName").CheckAndThrow();
                IsEnabled = configXml.GetAttribute("IsEnabled").Check("false").StringToBoolean();
                VariableSizedWrapGridStyleKey =
                    (VariableSizedWrapGridStyleKeys)
                    configXml.GetAttribute("VariableSizedWrapGridStyleKey").Check("1").StringToInt();

                foreach (XmlElement s in configXml.GetElementsByTagName("Service"))
                {
                    Activator.CreateInstance(s.GetAttribute("Type").CheckAndThrow().GenerateType(),
                                             new object[] {this, s});
                }
            }
            catch (Exception ex)
            {
                ex.WriteLog();
            }
        }

        #endregion

        #region Indexer for Service

        public IService this[Guid serviceID]
        {
            get
            {
                return Services.FirstOrDefault(s =>
                    {
                        var service = s as Service;
                        return service != null && service.ID == serviceID;
                    });
            }
        }

        public IService this[string serviceName]
        {
            get
            {
                return Services.FirstOrDefault(s =>
                    {
                        var service = s as Service;
                        return service != null && (service.Name == serviceName || service.ShortName == serviceName);
                    });
            }
        }

        #endregion

        public ServiceChannelModel Model { get; protected set; }
        public ObservableCollection<ServiceChannelModel> Models { get; protected set; }

        public void RegisterService(Service service)
        {
            if (Services == null) throw new InvalidOperationException("The services list is null");
            if (this[service.ID] != null) throw new InvalidOperationException("The service is already added");
            if (service == null) throw new InvalidOperationException("The service is null");

            Services.Add(service);
        }

        public VariableSizedWrapGridStyleKeys VariableSizedWrapGridStyleKey { get; set; }
    }
}