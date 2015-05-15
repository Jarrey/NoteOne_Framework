using System;
using System.Collections.ObjectModel;

namespace NoteOne_Core
{
    public class Notification : INotification
    {
        public Notification(IService service)
        {
            if (NotificationItems == null)
                NotificationItems = new ObservableCollection<INotificationItem>();

            Service = service;
            InitializeNotification();
            NotificationManager.CurrentNotificationManager.Register(this);
        }

        public Guid ID { get; protected set; }
        public string Name { get; protected set; }
        public string ShortName { get; protected set; }
        public IService Service { get; private set; }

        public ObservableCollection<INotificationItem> NotificationItems { get; private set; }

        protected virtual void InitializeNotification()
        {
        }

        protected virtual INotificationItem RegisterNotificationItem(INotificationItem item)
        {
            if (item == null)
                throw new InvalidOperationException("The notification item object is null");

            var notificationItem = item as NotificationItem;
            if (this[notificationItem.ID] == null)
                NotificationItems.Add(notificationItem);
            return notificationItem;
        }

        protected virtual void UnregisterNotificationItem(INotificationItem item)
        {
            if (item == null)
                throw new InvalidOperationException("The notification item object is null");

            INotificationItem notificationItem = this[(item as NotificationItem).ID];
            if (notificationItem != null)
                NotificationItems.Remove(notificationItem);
        }

        #region Indexer for NotificationItem

        public INotificationItem this[Guid notificationItemID]
        {
            get
            {
                INotificationItem notificationItem = null;
                foreach (INotificationItem n in NotificationItems)
                {
                    if ((n as NotificationItem).ID == notificationItemID)
                    {
                        notificationItem = n;
                        break;
                    }
                }
                return notificationItem;
            }
        }

        #endregion
    }
}