using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using NoteOne_Core.Common;
using NoteOne_Utility;
using NoteOne_Utility.Converters;
using NoteOne_Utility.Extensions;
using NoteOne_Utility.Helpers;
using Windows.Data.Xml.Dom;
using Windows.Foundation;

namespace NoteOne_Core
{
    public class Service : IService
    {
        public Service(ServiceChannel serviceChannel, XmlElement configXml)
        {
            if (serviceChannel == null)
                throw new InvalidOperationException("Cannot create the Service without one ServiceChannel");

            ConfigXml = configXml;
            ServiceChannel = serviceChannel;
            InitializeService(configXml);
            ServiceChannel.RegisterService(this);
        }

        public ServiceChannel ServiceChannel { get; private set; }
        public XmlElement ConfigXml { get; private set; }
        public Guid ID { get; private set; }
        public string Name { get; private set; }
        public string ShortName { get; private set; }
        public string ServiceApiUri { get; private set; }
        public ServiceTypes ServiceType { get; private set; }
        public ApiParameter ServiceApiParameters { get; private set; }

        public bool IsSupportNotification { get; private set; }
        public INotification Notification { get; private set; }

        public bool IsSupportBackgroundTask { get; private set; }
        public BackgroundTaskService BackgroundTaskService { get; private set; }

        public string Contry { get; private set; }
        public Point ServiceCenter { get; private set; } // x,y : longitude,latitude
        public double ServiceRadius { get; private set; } // unit is KM

        protected virtual void InitializeService(XmlElement configXml)
        {
            try
            {
                ID = new Guid(configXml.GetAttribute("ID")).CheckAndThrow();
                Name = configXml.GetAttribute("Name").CheckAndThrow();
                ShortName = configXml.GetAttribute("ShortName").CheckAndThrow();
                ServiceApiUri = configXml.GetAttribute("ServiceApiUri").CheckAndThrow();
                ServiceType = (ServiceTypes) configXml.GetAttribute("ServiceType").Check().StringToInt();

                string serviceApiParameters = configXml.GetAttribute("ServiceApiParameters").Check();
                if (!string.IsNullOrEmpty(serviceApiParameters))
                    ServiceApiParameters = Activator.CreateInstance(serviceApiParameters.GenerateType()) as ApiParameter;

                IsSupportNotification = configXml.GetAttribute("IsSupportNotification").Check("False").StringToBoolean();
                if (IsSupportNotification)
                {
                    string notification = configXml.GetAttribute("Notification").CheckAndThrow();
                    if (!string.IsNullOrEmpty(notification))
                        Notification = Activator.CreateInstance(notification.GenerateType(), this) as INotification;
                }

                IsSupportBackgroundTask =
                    configXml.GetAttribute("IsSupportBackgroundTask").Check("False").StringToBoolean();
                if (IsSupportBackgroundTask)
                {
                    string backgroundTaskService = configXml.GetAttribute("BackgroundTaskService").CheckAndThrow();
                    if (!string.IsNullOrEmpty(backgroundTaskService))
                        BackgroundTaskService =
                            Activator.CreateInstance(backgroundTaskService.GenerateType(),
                                                     new object[]
                                                         {this, configXml.SelectSingleNode("BackgroundTaskService")}) as
                            BackgroundTaskService;
                }

                Contry = configXml.GetAttribute("Contry").Check();
                ServiceCenter = configXml.GetAttribute("ServiceCenter").Check("0,0").StringToPoint();
                ServiceRadius = configXml.GetAttribute("ServiceRadius").Check("0").StringToDouble();

                // Validate the constructor values
                if (string.IsNullOrEmpty(ServiceApiUri))
                    throw new InvalidOperationException("The API Uri property is null or empty");
            }
            catch (Exception ex)
            {
                ex.WriteLog();
            }
        }

        protected virtual void InitializeParameters(object[] parameters)
        {
            // Check the parameter format
            if (parameters.Length != ServiceApiParameters.ParameterCount)
                throw new InvalidOperationException("The Parameters leght is wrong");
        }

        protected async Task<object> QueryDataAsyncInternal()
        {
            object result = null;
            try
            {
                Uri uri = null;
                if (ServiceApiParameters != null)
                    uri = new Uri(string.Format(ServiceApiUri, ServiceApiParameters.GetParameters()));
                else
                    uri = new Uri(ServiceApiUri);

#if DEBUG
                uri.WriteLog(LogType.Debug);
#endif

                HttpClientHelper.ResponseBody<string> responseBody =
                    await HttpClientHelper.Instance.GetResponseStringAsync(uri);
                if (responseBody.StatusCode == HttpStatusCode.OK)
                    result = responseBody.Body;
            }
            catch (HttpRequestException ex)
            {
                ex.WriteLog();
            }
            catch (Exception ex)
            {
                ex.WriteLog();
            }
            return result;
        }

        protected async Task<object> QueryDataAsyncInternal(string link)
        {
            object result = null;
            try
            {
                Uri uri = null;
                if (ServiceApiParameters != null)
                    uri = new Uri(string.Format(link, ServiceApiParameters.GetParameters()));
                else
                    uri = new Uri(link);

                HttpClientHelper.ResponseBody<string> responseBody =
                    await HttpClientHelper.Instance.GetResponseStringAsync(uri);
                if (responseBody.StatusCode == HttpStatusCode.OK)
                    result = responseBody.Body;
            }
            catch (HttpRequestException ex)
            {
                ex.WriteLog();
            }
            catch (Exception ex)
            {
                ex.WriteLog();
            }
            return result;
        }

        protected bool CheckNetworkStatus()
        {
            if ((bool) AppSettings.Instance.Settings[AppSettings.GLOBAL_INTERNET_STATUS] == false)
            {
                return false;
            }
            return true;
        }
    }
}