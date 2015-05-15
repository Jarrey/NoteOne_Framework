using Windows.UI.Xaml;

namespace NoteOne_Core.Interaction
{
    public class Interaction
    {
        public static readonly DependencyProperty TriggersProperty =
            DependencyProperty.RegisterAttached("Triggers", typeof (TriggerCollection), typeof (Interaction),
                                                new PropertyMetadata(null,
                                                                     (d, e) =>
                                                                         {
                                                                             if (e.NewValue != null)
                                                                             {
                                                                                 foreach (
                                                                                     Trigger trigger in
                                                                                         e.NewValue as TriggerCollection
                                                                                     )
                                                                                     trigger.AssociatedObject = d;
                                                                             }
                                                                         }));

        public static TriggerCollection GetTriggers(DependencyObject obj)
        {
            return (TriggerCollection) obj.GetValue(TriggersProperty);
        }

        public static void SetTriggers(DependencyObject obj, TriggerCollection value)
        {
            obj.SetValue(TriggersProperty, value);
        }
    }
}