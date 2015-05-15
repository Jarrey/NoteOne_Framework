using System;
using System.Threading.Tasks;
using NoteOne_Core.Resources;
using NoteOne_Core.UI.Common;
using NoteOne_Utility;
using NoteOne_Utility.Extensions;
using Windows.Networking.Connectivity;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace NoteOne_Core.Common
{
    public class NetworkStatusMonitor
    {
        #region For Singleton

        private static NetworkStatusMonitor _CurrentNetworkStatusMonitor;

        public static NetworkStatusMonitor CurrentNetworkStatusMonitor
        {
            get
            {
                if (_CurrentNetworkStatusMonitor == null)
                    _CurrentNetworkStatusMonitor = new NetworkStatusMonitor();
                return _CurrentNetworkStatusMonitor;
            }
        }

        #endregion

        private static bool _registeredNetworkStatusNotif;
        private readonly CoreDispatcher _cd = Window.Current.CoreWindow.Dispatcher;
        private readonly FullScreenPopup _networkDisconnectAlert;
        private NetworkStatusChangedEventHandler _networkStatusCallback;

        private NetworkStatusMonitor()
        {
            string contentXaml = ResourcesLoader.Loader["NetworkStatusXamlContent"];
            _networkDisconnectAlert = new FullScreenPopup(ConstKeys.NETWORK_MONITOR_KEY, contentXaml);
        }

        //
        //Register Network Status Change notifications
        //
        public void RegisterForNetworkStatusChangeNotif()
        {
            _networkStatusCallback =
                OnNetworkStatusChange;
            if (!_registeredNetworkStatusNotif)
            {
                NetworkInformation.NetworkStatusChanged += _networkStatusCallback;
                _registeredNetworkStatusNotif = true;
            }
        }

        //
        //Unregister Network Status Change notifications
        //
        public void UnRegisterForNetworkStatusChangeNotif()
        {
            NetworkInformation.NetworkStatusChanged -= _networkStatusCallback;
            _registeredNetworkStatusNotif = false;
        }

        public async Task CheckInternetStatusAsync()
        {
            //network status changed
            try
            {
                // get the ConnectionProfile that is currently used to connect to the Internet                
                ConnectionProfile internetConnectionProfile = NetworkInformation.GetInternetConnectionProfile();

                if (internetConnectionProfile == null)
                {
                    AppSettings.Instance.Settings[AppSettings.GLOBAL_INTERNET_STATUS] = false;
                    await _cd.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                            var frame = Window.Current.Content as Frame;
                            if (frame != null)
                                frame.IsEnabled = false;
                            _networkDisconnectAlert.Show();
                        });
                }
                else
                {
                    AppSettings.Instance.Settings[AppSettings.GLOBAL_INTERNET_STATUS] = true;
                    await _cd.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                            var frame = Window.Current.Content as Frame;
                            if (frame != null)
                                frame.IsEnabled = true;
                            _networkDisconnectAlert.Close();
                        });
                }
            }
            catch (Exception ex)
            {
                ex.WriteLog();
            }
        }

        //
        // Event handler for Network Status Change event
        // 
        private async void OnNetworkStatusChange(object sender)
        {
            await CheckInternetStatusAsync();
        }
    }
}