using System;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Input;
using NoteOne_Utility;
using NoteOne_Utility.Extensions;
using Windows.UI.Xaml;

namespace NoteOne_Core.Interaction
{
    public class AttachedCommand
    {
        public static readonly DependencyProperty RoutedEventProperty =
            DependencyProperty.RegisterAttached("RoutedEvent", typeof (string), typeof (AttachedCommand),
                                                new PropertyMetadata(string.Empty, OnRoutedEventChanged));

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached("Command", typeof (ICommand), typeof (AttachedCommand),
                                                new PropertyMetadata(null));

        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.RegisterAttached("CommandParameter", typeof (object), typeof (AttachedCommand),
                                                new PropertyMetadata(null));

        public static string GetRoutedEvent(DependencyObject obj)
        {
            return (string) obj.GetValue(RoutedEventProperty);
        }

        public static void SetRoutedEvent(DependencyObject obj, string value)
        {
            obj.SetValue(RoutedEventProperty, value);
        }


        public static ICommand GetCommand(DependencyObject obj)
        {
            return (ICommand) obj.GetValue(CommandProperty);
        }

        public static void SetCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(CommandProperty, value);
        }


        public static object GetCommandParameter(DependencyObject obj)
        {
            return obj.GetValue(CommandParameterProperty);
        }

        public static void SetCommandParameter(DependencyObject obj, object value)
        {
            obj.SetValue(CommandParameterProperty, value);
        }


        private static void OnRoutedEventChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var routedevent = (String) e.NewValue;

            if (!String.IsNullOrEmpty(routedevent))
            {
                var eventHooker = new EventHooker();
                eventHooker.AttachedCommandObject = d;
                var eventInfo = d.GetType().GetTypeInfo().GetMemberInfo(routedevent, MemberType.Event) as EventInfo;

                if (eventInfo != null)
                {
                    WindowsRuntimeMarshal.RemoveAllEventHandlers(
                        a => eventInfo.RemoveMethod.Invoke(d, new object[] {a}));
                    WindowsRuntimeMarshal.AddEventHandler(
                        f => (EventRegistrationToken) eventInfo.AddMethod.Invoke(d, new object[] {f}),
                        a => eventInfo.RemoveMethod.Invoke(d, new object[] {a}),
                        eventHooker.GetEventHandler(eventInfo));
                }
            }
        }

        internal sealed class EventHooker
        {
            public DependencyObject AttachedCommandObject { get; set; }

            public Delegate GetEventHandler(EventInfo eventInfo)
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
                // the second parameter in Windows.UI.Xaml.EventHandler is Object
            {
                var command = (ICommand) (sender as DependencyObject).GetValue(CommandProperty);
                object commandParameter = (sender as DependencyObject).GetValue(CommandParameterProperty);
                if (commandParameter == null) commandParameter = new[] {sender, e};

                if (command != null && command.CanExecute(commandParameter))
                {
                    command.Execute(commandParameter);
                }
            }
        }
    }
}