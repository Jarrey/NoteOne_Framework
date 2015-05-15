using System.Collections.ObjectModel;
using Windows.UI.Xaml;

namespace NoteOne_Core.Interaction
{
    public class Trigger : DependencyObject
    {
        private DependencyObject _associatedObject;

        public DependencyObject AssociatedObject
        {
            get { return _associatedObject; }
            set
            {
                if (_associatedObject != value)
                {
                    _associatedObject = value;
                    Update();
                }
            }
        }

        protected virtual void Update()
        {
        }
    }

    public sealed class TriggerCollection : Collection<Trigger>
    {
    }
}