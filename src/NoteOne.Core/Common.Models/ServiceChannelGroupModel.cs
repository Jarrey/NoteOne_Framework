using System;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using NoteOne_Core.Resources;

namespace NoteOne_Core.Common.Models
{
    [DataContract]
    public class ServiceChannelGroupModel : ModelBase
    {
        public ServiceChannelGroupModel(ServiceChannelGroupID id)
        {
            GroupID = id;
            Title = id.GetDescriptionViaResources<Enum>();

            if (Models == null)
                Models = new ObservableCollection<ServiceChannelModel>();
        }

        [DataMember]
        public ServiceChannelGroupID GroupID { get; set; }

        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public ObservableCollection<ServiceChannelModel> Models { get; private set; }

        //private bool _hasAction;
        //[DataMember]
        //public bool HasAction
        //{
        //    get { return _hasAction; }
        //    set { this.SetProperty(ref _hasAction, value); }
        //}

        //private RelayCommand _action;
        //public RelayCommand Action
        //{
        //    get { return _action; }
        //    set { this.SetProperty(ref _action, value); }
        //}
    }
}