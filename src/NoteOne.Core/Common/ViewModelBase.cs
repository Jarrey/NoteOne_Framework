using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using NoteOne_Utility.Helpers;
using Windows.Foundation.Metadata;
using Windows.UI.Xaml;

namespace NoteOne_Core.Common
{
    [WebHostHidden]
    public abstract class ViewModelBase : ObservableDictionary<String, Object>, INotifyPropertyChanged
    {
        public ViewModelBase(FrameworkElement view, Dictionary<string, object> pageState)
        {
            View = view;
            PageState = pageState;
        }

        protected FrameworkElement View { get; set; }
        protected Dictionary<string, object> PageState { get; set; }

        /// <summary>
        ///     Multicast event for property change notifications.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///     Checks if a property already matches a desired value.  Sets the property and
        ///     notifies listeners only when necessary.
        /// </summary>
        /// <typeparam name="T">Type of the property.</typeparam>
        /// <param name="storage">Reference to a property with both getter and setter.</param>
        /// <param name="value">Desired value for the property.</param>
        /// <param name="propertyName">
        ///     Name of the property used to notify listeners.  This
        ///     value is optional and can be provided automatically when invoked from compilers that
        ///     support CallerMemberName.
        /// </param>
        /// <returns>
        ///     True if the value was changed, false if the existing value matched the
        ///     desired value.
        /// </returns>
        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] String propertyName = null)
        {
            if (Equals(storage, value)) return false;

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        /// <summary>
        ///     Notifies listeners that a property value has changed.
        /// </summary>
        /// <param name="propertyName">
        ///     Name of the property used to notify listeners.  This
        ///     value is optional and can be provided automatically when invoked from compilers
        ///     that support <see cref="CallerMemberNameAttribute" />.
        /// </param>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler eventHandler = PropertyChanged;
            if (eventHandler != null)
            {
                eventHandler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public abstract void LoadState();

        protected virtual void LoadState(string[] Keys)
        {
            foreach (string key in Keys)
            {
                if (PageState != null && PageState.ContainsKey(key))
                    this[key] = PageState[key];
            }
        }

        public abstract void SaveState(Dictionary<String, Object> pageState);

        public virtual void SaveState(Dictionary<string, object> pageState, string[] Keys)
        {
            if (pageState != null)
            {
                foreach (string key in Keys)
                {
                    if (ContainsKey(key) && this[key] != null)
                        pageState[key] = this[key];
                }
            }
        }
    }
}