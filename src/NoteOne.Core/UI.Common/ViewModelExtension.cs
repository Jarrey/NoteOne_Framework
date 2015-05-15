using Windows.UI.Xaml;

namespace NoteOne_Core.UI.Common
{
    public sealed class ViewModelExtension : DependencyObject
    {
        public static readonly DependencyProperty InstanceProperty =
            DependencyProperty.Register("Instance", typeof (object), typeof (ViewModelExtension),
                                        new PropertyMetadata(null));

        public object Instance
        {
            get { return GetValue(InstanceProperty); }
            set { SetValue(InstanceProperty, value); }
        }
    }
}