using System;
using Windows.ApplicationModel.DataTransfer;

namespace NoteOne_Core.Contract
{
    public abstract class ShareContract
    {
        private DataTransferManager dataTransferManager;

        public ShareContract()
        {
            RegisterShareContract();
        }

        protected void RegisterShareContract()
        {
            dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested +=
                OnDataRequested;
        }

        protected void UnregisterShareContract()
        {
            if (dataTransferManager != null)
            {
                dataTransferManager.DataRequested -=
                    OnDataRequested;
            }
        }

        private void OnDataRequested(DataTransferManager sender, DataRequestedEventArgs e)
        {
            UnregisterShareContract();
            // Call the scenario specific function to populate the datapackage with the data to be shared.
            if (GetShareContent(e.Request))
            {
                if (String.IsNullOrEmpty(e.Request.Data.Properties.Title))
                {
                    e.Request.FailWithDisplayText("Error");
                }
            }
        }

        protected abstract bool GetShareContent(DataRequest request);
    }
}