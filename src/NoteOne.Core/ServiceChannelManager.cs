using System;
using System.Collections.ObjectModel;
using NoteOne_Core.Common.Models;

namespace NoteOne_Core
{
    public class ServiceChannelManager
    {
        private static ServiceChannelManager _currentServiceChannelManager;

        private ServiceChannelManager()
        {
            if (ServiceChannels == null)
                ServiceChannels = new ObservableCollection<IServiceChannel>();

            if (ServiceChannelGroups == null)
                ServiceChannelGroups = new ServiceChannelGroupCollection();

            if (AvailableModels == null)
                AvailableModels = new ObservableCollection<ServiceChannelModel>();
        }

        public static ServiceChannelManager CurrentServiceChannelManager
        {
            get
            {
                if (_currentServiceChannelManager == null)
                    _currentServiceChannelManager = new ServiceChannelManager();
                return _currentServiceChannelManager;
            }
        }

        public ObservableCollection<IServiceChannel> ServiceChannels { get; private set; }
        public ServiceChannelGroupCollection ServiceChannelGroups { get; private set; }
        public ObservableCollection<ServiceChannelModel> AvailableModels { get; private set; }

        public void Register(IServiceChannel s)
        {
            if (s == null)
                throw new InvalidOperationException("The service channel object is null");

            var serviceChannel = s as ServiceChannel;

            if (serviceChannel != null && serviceChannel.IsEnabled)
            {
                if (this[serviceChannel.ID] == null)
                {
                    ServiceChannels.Add(serviceChannel);

                    if (serviceChannel.Model != null)
                    {
                        if (serviceChannel.Model.IsEnabled)
                        {
                            // Generate the groups for service channels
                            ServiceChannelGroupModel group = ServiceChannelGroups[serviceChannel.Model.GroupID];
                            if (group == null)
                            {
                                group = new ServiceChannelGroupModel(serviceChannel.Model.GroupID);
                                ServiceChannelGroups.AddItem(group);
                            }
                            if (serviceChannel.Model.Index > group.Models.Count)
                                group.Models.Add(serviceChannel.Model);
                            else
                                group.Models.Insert(Math.Max(serviceChannel.Model.Index, 0), serviceChannel.Model);
                        }
                        AvailableModels.Add(serviceChannel.Model);
                    }


                    if (serviceChannel.Models != null)
                    {
                        foreach (ServiceChannelModel model in serviceChannel.Models)
                        {
                            if (model.IsEnabled)
                            {
                                // Generate the groups for service channels
                                ServiceChannelGroupModel group = ServiceChannelGroups[model.GroupID];
                                if (group == null)
                                {
                                    group = new ServiceChannelGroupModel(model.GroupID);
                                    ServiceChannelGroups.AddItem(group);
                                }
                                if (model.Index > group.Models.Count)
                                    group.Models.Add(model);
                                else
                                    group.Models.Insert(model.Index, model);
                            }
                            AvailableModels.Add(model);
                        }
                    }
                }
            }
        }

        public void Unregister(IServiceChannel s)
        {
            if (s == null)
                throw new InvalidOperationException("The service channel object is null");

            var channel = s as ServiceChannel;
            if (channel != null)
            {
                IServiceChannel serviceChannel = this[channel.ID];
                if (serviceChannel != null)
                {
                    ServiceChannels.Remove(serviceChannel);


                    // Remove the service channel from the group
                    ServiceChannelGroupModel group = ServiceChannelGroups[channel.Model.GroupID];
                    if (@group != null)
                    {
                        if (s.Model != null)
                        {
                            @group.Models.Remove(s.Model);
                            AvailableModels.Remove(s.Model);
                        }
                        if (s.Models != null)
                        {
                            foreach (ServiceChannelModel model in s.Models)
                            {
                                @group.Models.Remove(model);
                                if (@group.Models.Count == 0)
                                    ServiceChannelGroups.Remove(@group);
                                AvailableModels.Remove(model);
                            }
                        }
                    }
                }
            }
        }

        #region Indexer for ServiceChannel

        public IServiceChannel this[string serviceChannelName]
        {
            get
            {
                IServiceChannel serviceChannel = null;
                foreach (IServiceChannel s in ServiceChannels)
                {
                    if ((s as ServiceChannel).Name == serviceChannelName ||
                        (s as ServiceChannel).ShortName == serviceChannelName)
                    {
                        serviceChannel = s;
                        break;
                    }
                }
                return serviceChannel;
            }
        }

        public IServiceChannel this[Guid serviceChannelID]
        {
            get
            {
                IServiceChannel serviceChannel = null;
                foreach (IServiceChannel s in ServiceChannels)
                {
                    if ((s as ServiceChannel).ID == serviceChannelID)
                    {
                        serviceChannel = s;
                        break;
                    }
                }
                return serviceChannel;
            }
        }

        #endregion
    }
}