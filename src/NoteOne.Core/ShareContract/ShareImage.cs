using System.Collections.Generic;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.Storage.Streams;

namespace NoteOne_Core.Contract
{
    public class ShareImage : ShareContract
    {
        private readonly string _description;
        private readonly StorageFile _imageFile;
        private readonly string _title;

        public ShareImage(StorageFile imageFile, string title, string description = "")
        {
            _imageFile = imageFile;
            _title = title;
            _description = description;
            DataTransferManager.ShowShareUI();
        }

        protected override bool GetShareContent(DataRequest request)
        {
            bool succeeded = false;

            if (_imageFile != null)
            {
                DataPackage requestData = request.Data;
                requestData.Properties.Title = _title;
                requestData.Properties.Description = _description; // The description is optional.

                var imageItems = new List<IStorageItem> {_imageFile};
                requestData.SetStorageItems(imageItems);

                RandomAccessStreamReference imageStreamRef = RandomAccessStreamReference.CreateFromFile(_imageFile);
                requestData.Properties.Thumbnail = imageStreamRef;
                requestData.SetBitmap(imageStreamRef);
                succeeded = true;
            }
            else
            {
                request.FailWithDisplayText("Select an image you would like to share and try again.");
            }
            return succeeded;
        }
    }
}