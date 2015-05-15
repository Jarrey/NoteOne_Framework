using System.Collections.ObjectModel;
using NoteOne_Core.Common;
using NoteOne_Core.Common.Models;

namespace NoteOne_Core
{
    public class ServiceChannelGroupCollection : ObservableCollection<ServiceChannelGroupModel>
    {
        public ServiceChannelGroupModel this[ServiceChannelGroupID id]
        {
            get
            {
                ServiceChannelGroupModel group = null;
                foreach (ServiceChannelGroupModel g in this)
                {
                    if (g.GroupID == id)
                        group = g;
                }
                return group;
            }
        }

        public void AddItem(ServiceChannelGroupModel model)
        {
            for (int i = 0; i < Count; i++)
            {
                if (model.GroupID < this[i].GroupID)
                {
                    Insert(i, model);
                    return;
                }
            }
            Add(model);
        }
    }
}