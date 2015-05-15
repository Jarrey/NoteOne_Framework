using System.Collections.ObjectModel;
using System.Reflection;
using NoteOne_Utility;
using NoteOne_Utility.Extensions;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace NoteOne_Core.UI.Common
{
    public class ControlDataTemplateSelector : DataTemplateSelector
    {
        public ControlDataTemplateSelector()
        {
            DataTemplates = new DataTemplateCollection();
        }

        public DataTemplateCollection DataTemplates { get; set; }
        public string PropertyName { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            var prop = item.GetType().GetTypeInfo().GetMemberInfo(PropertyName, MemberType.Property) as PropertyInfo;
            if (prop != null)
            {
                object value = prop.GetValue(item);
                if (value != null)
                {
                    foreach (DataTemplateModel dataTemplate in DataTemplates)
                    {
                        if (
                            dataTemplate.SelectValue.Trim()
                                        .ToUpperInvariant()
                                        .Equals(value.ToString().Trim().ToUpperInvariant()))
                            return dataTemplate.Template;
                    }
                }
            }
            return null;
        }
    }

    public class DataTemplateCollection : Collection<DataTemplateModel>
    {
    }

    public class DataTemplateModel
    {
        public DataTemplate Template { get; set; }
        public string SelectValue { get; set; }
    }
}