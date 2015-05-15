using System;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using NoteOne_Core.Behaviours;
using NoteOne_Utility;
using NoteOne_Utility.Extensions;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Markup;

namespace NoteOne_Core.Interaction
{
    [ContentProperty(Name = "Behaviour")]
    public class EventTrigger : Trigger
    {
        #region DP

        public static readonly DependencyProperty EventNameProperty =
            DependencyProperty.Register("EventName", typeof (string), typeof (EventTrigger),
                                        new PropertyMetadata(String.Empty, OnEventNameChanged));


        public static readonly DependencyProperty BehaviourProperty =
            DependencyProperty.Register("Behaviour", typeof (TriggerBehaviour), typeof (EventTrigger),
                                        new PropertyMetadata(null));

        public String EventName
        {
            get { return (String) GetValue(EventNameProperty); }
            set { SetValue(EventNameProperty, value); }
        }

        public TriggerBehaviour Behaviour
        {
            get { return (TriggerBehaviour) GetValue(BehaviourProperty); }
            set { SetValue(BehaviourProperty, value); }
        }

        #endregion

        private static void OnEventNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var eventtrigger = d as EventTrigger;
            if (eventtrigger.AssociatedObject != null)
                eventtrigger.Update();
        }

        protected override void Update()
        {
            base.Update();

            String routedevent = EventName;

            if (!String.IsNullOrEmpty(routedevent) && AssociatedObject != null)
            {
                var eventInfo =
                    AssociatedObject.GetType().GetTypeInfo().GetMemberInfo(routedevent, MemberType.Event) as EventInfo;

                if (eventInfo != null)
                {
                    WindowsRuntimeMarshal.RemoveAllEventHandlers(
                        a => eventInfo.RemoveMethod.Invoke(AssociatedObject, new object[] {a}));
                    WindowsRuntimeMarshal.AddEventHandler(
                        f => (EventRegistrationToken) eventInfo.AddMethod.Invoke(AssociatedObject, new object[] {f}),
                        a => eventInfo.RemoveMethod.Invoke(AssociatedObject, new object[] {a}),
                        GetEventHandler(eventInfo));
                }
            }
        }

        private Delegate GetEventHandler(EventInfo eventInfo)
        {
            Delegate del = null;
            if (eventInfo == null)
                throw new ArgumentNullException("eventInfo");

            if (eventInfo.EventHandlerType == null)
                throw new ArgumentNullException("eventInfo.EventHandlerType");

            if (del == null)
                del =
                    GetType()
                        .GetTypeInfo()
                        .GetDeclaredMethod("OnEventRaised")
                        .CreateDelegate(eventInfo.EventHandlerType, this);

            return del;
        }

        private void OnEventRaised(object sender, object e)
        {
            if (Behaviour != null)
            {
                Behaviour.AssociatedObject = AssociatedObject;
                Behaviour.Invoke(sender, e);
            }
        }
    }
}