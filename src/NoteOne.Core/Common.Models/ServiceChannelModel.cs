using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using NoteOne_Core.Interfaces;

namespace NoteOne_Core.Common.Models
{
    [DataContract]
    public class ServiceChannelModel : ModelBase, IVariableSizedWrapGridStyle
    {
        [DataMember] private ServiceChannelGroupID _groupID;
        [DataMember] private bool _isEnabled;
        private IEnumerable<BindableImage> _logo;
        [DataMember] private string _subTitle;
        [DataMember] private string _title;

        public ServiceChannelModel(ServiceChannel channel)
        {
            Channel = channel;
            ID = channel.ID;
            VariableSizedWrapGridStyleKey = channel.VariableSizedWrapGridStyleKey;
        }

        public ServiceChannel Channel { get; private set; }

        /// <summary>
        ///     The order index in group
        /// </summary>
        [DataMember]
        public int Index { get; protected set; }

        [DataMember]
        public Guid ID { get; private set; }

        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public string SubTitle
        {
            get { return _subTitle; }
            set { SetProperty(ref _subTitle, value); }
        }

        public IEnumerable<BindableImage> Logo
        {
            get { return _logo; }
            set { SetProperty(ref _logo, value); }
        }

        public virtual bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                if (value) AddModel();
                else RemoveModel();
                SetProperty(ref _isEnabled, value);
            }
        }

        public ServiceChannelGroupID GroupID
        {
            get { return _groupID; }
            set { SetProperty(ref _groupID, value); }
        }

        [DataMember]
        public VariableSizedWrapGridStyleKeys VariableSizedWrapGridStyleKey { get; set; }


        private void AddModel()
        {
            ServiceChannelGroupModel group =
                ServiceChannelManager.CurrentServiceChannelManager.ServiceChannelGroups[GroupID];
            if (group == null)
            {
                group = new ServiceChannelGroupModel(GroupID);
                ServiceChannelManager.CurrentServiceChannelManager.ServiceChannelGroups.AddItem(group);
            }
            if (Index > group.Models.Count)
                group.Models.Add(this);
            else
                group.Models.Insert(Index, this);

            Channel.InitializeLogo();
        }

        private void RemoveModel()
        {
            ServiceChannelGroupModel group =
                ServiceChannelManager.CurrentServiceChannelManager.ServiceChannelGroups[GroupID];
            group.Models.Remove(this);
            if (group.Models.Count == 0)
                ServiceChannelManager.CurrentServiceChannelManager.ServiceChannelGroups.Remove(group);
        }
    }
}