using Windows.UI.Xaml;

namespace NoteOne_Core.Behaviours
{
    public class TriggerBehaviour : DependencyObject
    {
        public DependencyObject AssociatedObject { get; set; }

        public virtual void Invoke(object sender, object e)
        {
        }
    }
}