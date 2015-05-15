using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace NoteOne_Utility.Helpers
{
    public class VisualTreeHelperExtension
    {
        public static T GetVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            T child = default(T);
            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < numVisuals; i++)
            {
                DependencyObject d = VisualTreeHelper.GetChild(parent, i);
                child = d as T;
                if (child == null)
                    child = GetVisualChild<T>(d);
                if (child != null)
                    break;
            }
            return child;
        }

        public static T GetVisualChild<T>(DependencyObject parent, string name) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                var controlName = child.GetValue(FrameworkElement.NameProperty) as string;
                if (controlName == name)
                    return child as T;
                else
                {
                    var result = GetVisualChild<T>(child, name);
                    if (result != null)
                        return result;
                }
            }
            return null;
        }

        public static T GetVisualParent<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);
            if (parentObject == null) return null;
            var parent = parentObject as T;
            if (parent != null)
                return parent;
            else
                return GetVisualParent<T>(parentObject);
        }
    }
}