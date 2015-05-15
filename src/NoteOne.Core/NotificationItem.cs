using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using NoteOne_Core.Common;
using NoteOne_Utility.Helpers;

namespace NoteOne_Core
{
    public class NotificationItem : INotificationItem
    {
        private NotificationItemStatusCode _statusCode;

        public NotificationItem(INotification notification)
        {
            Notification = notification;
            ID = Guid.NewGuid();

            if (Conditions == null)
                Conditions = new ObservableCollection<WeakFunc<object[], bool>>();

            if (Actions == null)
                Actions = new ObservableCollection<WeakAction<object[]>>();

            InitializeNotificationItem();
        }

        public INotification Notification { get; private set; }
        public Guid ID { get; private set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public double Interval { get; set; }

        public NotificationItemStatusCode StatusCode
        {
            get { return _statusCode; }
            protected set
            {
                var eventArgs = new NotificationItemStatusCodeEventArgs();
                eventArgs.OldStatusCode = _statusCode;
                _statusCode = value;
                eventArgs.NewStatusCode = value;
                OnStatusCodeChanged(eventArgs);
            }
        }

        public ObservableCollection<WeakFunc<object[], bool>> Conditions { get; private set; }
        public ObservableCollection<WeakAction<object[]>> Actions { get; private set; }

        #region Events

        public event EventHandler<NotificationItemStatusCodeEventArgs> StatusCodeChanged;

        protected virtual void OnStatusCodeChanged(NotificationItemStatusCodeEventArgs e)
        {
            StatusCodeChanged(this, e);
        }

        #endregion

        protected virtual void InitializeNotificationItem()
        {
        }

        public virtual void Start()
        {
            new Task(() =>
                {
                    while (true)
                    {
                        bool result = true;
                        foreach (var condition in Conditions)
                        {
                            result = result && condition.Execute(null);
                        }
                        if (result)
                        {
                            foreach (var action in Actions)
                            {
                                action.Execute(null);
                            }
                        }
                    }
                }).Start();
            StatusCode = NotificationItemStatusCode.Running;
        }
    }
}