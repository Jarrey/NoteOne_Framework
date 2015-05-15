using NoteOne_Core.Interfaces;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace NoteOne_Core.UI.Common
{
    public class VariableSizedWrapGridStyleSelector : StyleSelector
    {
        public Style Style1x1 { get; set; }
        public Style Style1x2 { get; set; }
        public Style Style2x1 { get; set; }
        public Style Style2x2 { get; set; }
        public Style Style2x3 { get; set; }
        public Style Style3x2 { get; set; }
        public Style Style1x3 { get; set; }
        public Style Style3x1 { get; set; }
        public Style Style3x3 { get; set; }
        public Style Style4x4 { get; set; }
        public Style Style4x2 { get; set; }
        public Style Style2x4 { get; set; }


        protected override Style SelectStyleCore(object item, DependencyObject container)
        {
            if (item != null && item is IVariableSizedWrapGridStyle)
            {
                var i = item as IVariableSizedWrapGridStyle;
                switch (i.VariableSizedWrapGridStyleKey)
                {
                    case VariableSizedWrapGridStyleKeys.Style1x1:
                        return Style1x1;
                    case VariableSizedWrapGridStyleKeys.Style1x2:
                        return Style1x2;
                    case VariableSizedWrapGridStyleKeys.Style2x1:
                        return Style2x1;
                    case VariableSizedWrapGridStyleKeys.Style2x2:
                        return Style2x2;
                    case VariableSizedWrapGridStyleKeys.Style1x3:
                        return Style1x3;
                    case VariableSizedWrapGridStyleKeys.Style2x3:
                        return Style2x3;
                    case VariableSizedWrapGridStyleKeys.Style3x1:
                        return Style3x1;
                    case VariableSizedWrapGridStyleKeys.Style3x2:
                        return Style3x2;
                    case VariableSizedWrapGridStyleKeys.Style3x3:
                        return Style3x3;
                    case VariableSizedWrapGridStyleKeys.Style4x4:
                        return Style4x4;
                    case VariableSizedWrapGridStyleKeys.Style4x2:
                        return Style4x2;
                    case VariableSizedWrapGridStyleKeys.Style2x4:
                        return Style2x4;
                }
            }
            return base.SelectStyleCore(item, container);
        }
    }
}