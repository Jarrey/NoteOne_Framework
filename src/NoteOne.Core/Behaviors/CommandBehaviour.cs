using System.Reflection;
using System.Windows.Input;
using NoteOne_Core.Common;
using NoteOne_Utility;
using NoteOne_Utility.Extensions;
using Windows.UI.Xaml;

namespace NoteOne_Core.Behaviours
{
    public class CommandBehaviour : TriggerBehaviour
    {
        public static readonly DependencyProperty CommandPropertyPathProperty =
            DependencyProperty.Register("CommandPropertyPath", typeof (PropertyPath), typeof (CommandBehaviour),
                                        new PropertyMetadata(null));

        public PropertyPath CommandPropertyPath
        {
            get { return (PropertyPath) GetValue(CommandPropertyPathProperty); }
            set { SetValue(CommandPropertyPathProperty, value); }
        }

        public override void Invoke(object sender, object e)
        {
            base.Invoke(sender, e);

            if (AssociatedObject != null)
            {
                object dataContext = AssociatedObject.GetValue(FrameworkElement.DataContextProperty);
                if (dataContext != null)
                {
                    var commandProperty =
                        dataContext.GetType().GetTypeInfo().GetMemberInfo(CommandPropertyPath.Path, MemberType.Property)
                        as PropertyInfo;
                    if (commandProperty != null)
                    {
                        var command = commandProperty.GetValue(dataContext) as ICommand;
                        if (command != null)
                            ExecuteCommand(command, new[] {sender, e});
                        return;
                    }

                    if (dataContext is ViewModelBase)
                    {
                        var command = (dataContext as ViewModelBase)[CommandPropertyPath.Path] as ICommand;
                        if (command != null)
                            ExecuteCommand(command, new[] {sender, e});
                        return;
                    }
                }
            }
        }

        private void ExecuteCommand(ICommand command, object parameter)
        {
            if (command != null && command.CanExecute(parameter))
                command.Execute(parameter);
        }
    }
}