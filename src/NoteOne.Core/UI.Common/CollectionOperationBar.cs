using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using NoteOne.Utility;
using System.Windows.Input;

// The Templated Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234235

namespace NoteOne.Core.UI.Common
{
    public sealed class CollectionOperationBar : ContentControl
    {
        public CollectionOperationBar()
        {
            this.DefaultStyleKey = typeof(CollectionOperationBar);
        }


        public object ItemsSource
        {
            get { return (IEnumerable<object>)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }
        public static readonly DependencyProperty ItemsSourceProperty =
           DependencyProperty.Register("ItemsSource", typeof(object), typeof(CollectionOperationBar), new PropertyMetadata(null));



        public object SelectedItem
        {
            get { return (object)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(object), typeof(CollectionOperationBar), new PropertyMetadata(null));



        public string CommandLabelPath
        {
            get { return (string)GetValue(CommandLabelPathProperty); }
            set { SetValue(CommandLabelPathProperty, value); }
        }
        public static readonly DependencyProperty CommandLabelPathProperty =
           DependencyProperty.Register("CommandLabelPath", typeof(string), typeof(CollectionOperationBar), new PropertyMetadata(string.Empty));




        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(CollectionOperationBar), new PropertyMetadata(null));



        public object CommandParameter
        {
            get { return (object)GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(CollectionOperationBar), new PropertyMetadata(null));



        protected async override void OnTapped(TappedRoutedEventArgs e)
        {
            base.OnTapped(e);

            if (ItemsSource != null && ItemsSource is IEnumerable<object>)
            {
                PopupMenu menu = new PopupMenu();

                foreach (object i in ItemsSource as IEnumerable<object>)
                {
                    string label = string.Empty;
                    if (string.IsNullOrEmpty(this.CommandLabelPath))
                        label = i.ToString();
                    else 
                        label = this.CommandLabelPath;

                    menu.Commands.Add(new UICommand(label, (command) =>
                        {
                            Command.Execute(null);
                        }));
                }
                IUICommand chosenCommand = await menu.ShowForSelectionAsync(this.GetElementRect());
                if (chosenCommand != null)
                {
                    this.SelectedItem = chosenCommand.Label;
                }
            }
        }
    }
}
