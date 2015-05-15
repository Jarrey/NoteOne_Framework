using System;
using NoteOne_Core.Common;
using NoteOne_Utility.Extensions;
using Windows.ApplicationModel.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace NoteOne_Core.Behaviours
{
    public class NavigateToBehaviour : TriggerBehaviour
    {
        public static readonly DependencyProperty PageTypeProperty =
            DependencyProperty.Register("PageType", typeof (string), typeof (NavigateToBehaviour),
                                        new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty ParameterProperty =
            DependencyProperty.Register("Parameter", typeof (object), typeof (NavigateToBehaviour),
                                        new PropertyMetadata(null));

        public string PageType
        {
            get { return (string) GetValue(PageTypeProperty); }
            set { SetValue(PageTypeProperty, value); }
        }

        public object Parameter
        {
            get { return GetValue(ParameterProperty); }
            set { SetValue(ParameterProperty, value); }
        }

        public override void Invoke(object sender, object e)
        {
            base.Invoke(sender, e);

            var navigationFrame = Window.Current.Content as Frame;
            if (navigationFrame != null)
            {
                if (!string.IsNullOrEmpty(PageType))
                {
                    var parameter = new[] {e, Parameter};
                    string key = Guid.NewGuid().ToString();
                    CoreApplication.Properties.Add(key, parameter);
                    navigationFrame.Navigate(PageType.GenerateType(), key);
                    CoreApplication.Properties.Remove(key);
                }
                else
                {
                    if (e is ItemClickEventArgs && (e as ItemClickEventArgs).ClickedItem != null)
                    {
                        object item = (e as ItemClickEventArgs).ClickedItem;
                        var model = item as ModelBase;
                        if (model != null && model.PrimaryViewType != null)
                        {
                            var parameter = new[] {e, Parameter, model};
                            string key = Guid.NewGuid().ToString();
                            CoreApplication.Properties.Add(key, parameter);
                            navigationFrame.Navigate(model.PrimaryViewType, key);
                            CoreApplication.Properties.Remove(key);
                        }
                    }
                }
            }
        }
    }
}