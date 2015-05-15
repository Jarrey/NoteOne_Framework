using System;

namespace NoteOne_Core.Common
{
    public class NotificationItemStatusCodeEventArgs : EventArgs
    {
        public NotificationItemStatusCodeEventArgs()
        {
            NewStatusCode = NotificationItemStatusCode.NotStart;
            OldStatusCode = NotificationItemStatusCode.NotStart;
        }

        public NotificationItemStatusCodeEventArgs(NotificationItemStatusCode oldValue)
        {
            OldStatusCode = oldValue;
        }

        public NotificationItemStatusCode NewStatusCode { get; set; }
        public NotificationItemStatusCode OldStatusCode { get; set; }
    }
}