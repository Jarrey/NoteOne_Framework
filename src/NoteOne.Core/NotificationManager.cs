using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace NoteOne_Core
{
    public class NotificationManager
    {
        private static NotificationManager _currentNotificationManager;

        private NotificationManager()
        {
            if (Notifications == null)
                Notifications = new ObservableCollection<INotification>();
        }

        public static NotificationManager CurrentNotificationManager
        {
            get
            {
                if (_currentNotificationManager == null)
                    _currentNotificationManager = new NotificationManager();
                return _currentNotificationManager;
            }
        }

        public ObservableCollection<INotification> Notifications { get; private set; }

        public void Register(INotification n)
        {
            if (n == null)
                throw new InvalidOperationException("The notification object is null");

            var notification = n as Notification;
            if (notification != null && this[notification.ID] == null)
                Notifications.Add(notification);
        }

        public void Unregister(INotification n)
        {
            if (n == null)
                throw new InvalidOperationException("The notification object is null");

            var notification1 = n as Notification;
            if (notification1 != null)
            {
                INotification notification = this[notification1.ID];
                if (notification != null)
                    Notifications.Remove(notification);
            }
        }

        #region Indexer for Notification

        public INotification this[string notificationName]
        {
            get
            {
                return Notifications.FirstOrDefault(n =>
                    {
                        var notification = n as Notification;
                        return notification != null &&
                               (notification.Name == notificationName || notification.ShortName == notificationName);
                    });
            }
        }

        public INotification this[Guid notificationID]
        {
            get
            {
                return Notifications.FirstOrDefault(n =>
                    {
                        var notification = n as Notification;
                        return notification != null && notification.ID == notificationID;
                    });
            }
        }

        #endregion
    }
}