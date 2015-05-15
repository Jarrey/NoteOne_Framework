using System;
using System.Collections.Generic;
using Windows.ApplicationModel.Background;
using Windows.Storage;

namespace NoteOne_Core.Common
{
    public class BackgroundTaskController
    {
        private static readonly Dictionary<string, bool> BackgroundTaskRegisterStatus =
            new Dictionary<string, bool>();

        static BackgroundTaskController()
        {
            foreach (var cur in BackgroundTaskRegistration.AllTasks)
            {
                BackgroundTaskRegisterStatus[cur.Value.Name] = true;
            }
        }

        /// <summary>
        ///     Register a background task with the specified taskEntryPoint, name, trigger,
        ///     and condition (optional).
        /// </summary>
        /// <param name="taskEntryPoint">Task entry point for the background task.</param>
        /// <param name="name">A name for the background task.</param>
        /// <param name="trigger">The trigger for the background task.</param>
        /// <param name="condition">An optional conditional event that must be true for the task to fire.</param>
        public static BackgroundTaskRegistration RegisterBackgroundTask(String taskEntryPoint, String name,
                                                                        IBackgroundTrigger trigger,
                                                                        IBackgroundCondition condition)
        {
            var builder = new BackgroundTaskBuilder();

            builder.Name = name;
            builder.TaskEntryPoint = taskEntryPoint;
            builder.SetTrigger(trigger);

            if (condition != null)
            {
                builder.AddCondition(condition);
            }

            BackgroundTaskRegistration task = builder.Register();

            UpdateBackgroundTaskStatus(name, true);

            //
            // Remove previous completion status from local settings.
            //
            ApplicationDataContainer settings = ApplicationData.Current.LocalSettings;
            settings.Values.Remove(name);

            return task;
        }

        /// <summary>
        ///     Unregister background tasks with specified name.
        /// </summary>
        /// <param name="name">Name of the background task to unregister.</param>
        public static void UnregisterBackgroundTasks(string name)
        {
            //
            // Loop through all background tasks and unregister any with SampleBackgroundTaskName or
            // SampleBackgroundTaskWithConditionName.
            //
            foreach (var cur in BackgroundTaskRegistration.AllTasks)
            {
                if (cur.Value.Name == name)
                {
                    cur.Value.Unregister(true);
                }
            }

            UpdateBackgroundTaskStatus(name, false);
        }

        /// <summary>
        ///     Store the registration status of a background task with a given name.
        /// </summary>
        /// <param name="name">Name of background task to store registration status for.</param>
        /// <param name="registered">TRUE if registered, FALSE if unregistered.</param>
        public static void UpdateBackgroundTaskStatus(String name, bool registered)
        {
            BackgroundTaskRegisterStatus[name] = registered;
        }

        /// <summary>
        ///     Get the registration / completion status of the background task with
        ///     given name.
        /// </summary>
        /// <param name="name">Name of background task to retreive registration status.</param>
        public static bool GetBackgroundTaskStatus(String name)
        {
            bool registered = false;
            if (BackgroundTaskRegisterStatus.ContainsKey(name))
                registered = BackgroundTaskRegisterStatus[name];

            return registered;
        }
    }
}