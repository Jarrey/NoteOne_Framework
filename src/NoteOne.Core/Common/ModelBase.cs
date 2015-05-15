using System;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace NoteOne_Core.Common
{
    [DataContract]
    public class ModelBase : BindableBase
    {
        [DataMember] private Type _primaryViewType;

        public ModelBase()
        {
            if (Services == null) Services = new ObservableCollection<IService>();
        }

        public ModelBase(IService[] services)
            : this()
        {
            foreach (IService service in services)
                Services.Add(service);
        }

        public ObservableCollection<IService> Services { get; private set; }

        public Type PrimaryViewType
        {
            get { return _primaryViewType; }
            set { SetProperty(ref _primaryViewType, value); }
        }

        #region Indexer for Service

        public IService this[Guid serviceID]
        {
            get
            {
                IService service = null;
                foreach (IService s in Services)
                {
                    if ((s as Service).ID == serviceID)
                    {
                        service = s;
                        break;
                    }
                }
                return service;
            }
        }

        public IService this[string serviceName]
        {
            get
            {
                IService service = null;
                foreach (IService s in Services)
                {
                    if ((s as Service).Name == serviceName ||
                        (s as Service).ShortName == serviceName)
                    {
                        service = s;
                        break;
                    }
                }
                return service;
            }
        }

        #endregion
    }
}