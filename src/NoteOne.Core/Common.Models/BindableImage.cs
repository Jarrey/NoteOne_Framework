using System;
using System.Runtime.Serialization;
using NoteOne_Utility;
using NoteOne_Utility.Extensions;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;

namespace NoteOne_Core.Common.Models
{
    [DataContract]
    public class BindableImage : ModelBase
    {
        private bool _errorImage;
        private bool _isOriginalImageDownloading;
        private bool _isThumbnailImageDownloading;
        private BitmapImage _localImage;
        private string _localImagePath;
        private IRandomAccessStream _localImageStream;
        private BitmapImage _originalImage;
        private int _originalImageDownloadingProgress;
        private string _originalImageUrl;

        private BitmapImage _thumbnailImage;
        private int _thumbnailImageDownloadingProgress;
        private string _thumbnailImageUrl;

        public BitmapImage OriginalImage
        {
            get
            {
                CoreWindow coreWindow = CoreWindow.GetForCurrentThread();
                if (coreWindow != null && coreWindow.Dispatcher.HasThreadAccess)
                {
                    if (_originalImage == null)
                        _originalImage = new BitmapImage();
                    _originalImage.DownloadProgress -= OriginalImage_DownloadProgress;
                    _originalImage.DownloadProgress += OriginalImage_DownloadProgress;
                }
                return _originalImage;
            }
            set { SetProperty(ref _originalImage, value); }
        }

        public BitmapImage ThumbnailImage
        {
            get
            {
                CoreWindow coreWindow = CoreWindow.GetForCurrentThread();
                if (coreWindow != null && coreWindow.Dispatcher.HasThreadAccess)
                {
                    if (_thumbnailImage == null)
                        _thumbnailImage = new BitmapImage();
                    _thumbnailImage.DownloadProgress -= ThumbnailImage_DownloadProgress;
                    _thumbnailImage.DownloadProgress += ThumbnailImage_DownloadProgress;
                }
                return _thumbnailImage;
            }
            set { SetProperty(ref _thumbnailImage, value); }
        }

        [DataMember]
        public bool IsThumbnailImageDownloading
        {
            get { return _isThumbnailImageDownloading; }
            set { SetProperty(ref _isThumbnailImageDownloading, value); }
        }

        [DataMember]
        public bool IsOriginalImageDownloading
        {
            get { return _isOriginalImageDownloading; }
            set { SetProperty(ref _isOriginalImageDownloading, value); }
        }

        [DataMember]
        public int ThumbnailImageDownloadingProgress
        {
            get { return _thumbnailImageDownloadingProgress; }
            set { SetProperty(ref _thumbnailImageDownloadingProgress, value); }
        }

        [DataMember]
        public int OriginalImageDownloadingProgress
        {
            get { return _originalImageDownloadingProgress; }
            set { SetProperty(ref _originalImageDownloadingProgress, value); }
        }

        [DataMember]
        public string OriginalImageUrl
        {
            get { return _originalImageUrl; }
            set
            {
                if (OriginalImage != null)
                {
                    IsOriginalImageDownloading = true;
                    try
                    {
                        OriginalImage.UriSource = new Uri(value);
                    }
                    catch (Exception ex)
                    {
                        IsOriginalImageDownloading = false;
                        ex.WriteLog();
                        value.WriteLog(LogType.Exception);
                        throw ex;
                    }
                }
                SetProperty(ref _originalImageUrl, value);
            }
        }

        [DataMember]
        public string ThumbnailImageUrl
        {
            get { return _thumbnailImageUrl; }
            set
            {
                if (ThumbnailImage != null)
                {
                    IsThumbnailImageDownloading = true;
                    try
                    {
                        ThumbnailImage.UriSource = new Uri(value);
                    }
                    catch (Exception ex)
                    {
                        IsThumbnailImageDownloading = false;
                        ex.WriteLog();
                        value.WriteLog(LogType.Exception);
                        throw ex;
                    }
                }
                SetProperty(ref _thumbnailImageUrl, value);
            }
        }


        public BitmapImage LocalImage
        {
            get
            {
                CoreWindow coreWindow = CoreWindow.GetForCurrentThread();
                if (coreWindow != null && coreWindow.Dispatcher.HasThreadAccess)
                {
                    if (_localImage == null)
                        _localImage = new BitmapImage();
                }
                return _localImage;
            }
            set { SetProperty(ref _localImage, value); }
        }

        [DataMember]
        public string LocalImagePath
        {
            get { return _localImagePath; }
            set
            {
                if (LocalImage != null)
                {
                    try
                    {
                        SetLocalBitmapImageSource(LocalImage, value);
                    }
                    catch (Exception ex)
                    {
                        ex.WriteLog();
                    }
                }
                SetProperty(ref _localImagePath, value);
            }
        }

        [DataMember]
        public bool ErrorImage
        {
            get { return _errorImage; }
            set { SetProperty(ref _errorImage, value); }
        }

        private void ThumbnailImage_DownloadProgress(object sender, DownloadProgressEventArgs e)
        {
            ThumbnailImageDownloadingProgress = e.Progress;
            if (e.Progress == 100)
            {
                IsThumbnailImageDownloading = false;
                ThumbnailImage.DownloadProgress -= ThumbnailImage_DownloadProgress;
            }
        }

        private void OriginalImage_DownloadProgress(object sender, DownloadProgressEventArgs e)
        {
            OriginalImageDownloadingProgress = e.Progress;
            if (e.Progress == 100)
            {
                IsOriginalImageDownloading = false;
                OriginalImage.DownloadProgress -= OriginalImage_DownloadProgress;
            }
        }

        private void LocalBitmapImage_DisposeStream(object sender, RoutedEventArgs e)
        {
            if (_localImageStream != null)
            {
                _localImageStream.Dispose();
                _localImageStream = null;
            }
            LocalImage.ImageFailed -= LocalBitmapImage_DisposeStream;
            LocalImage.ImageOpened -= LocalBitmapImage_DisposeStream;
        }

        private async void SetLocalBitmapImageSource(BitmapImage bitmapImage, string path)
        {
            try
            {
                StorageFile file = await StorageFile.GetFileFromPathAsync(path);
                using (_localImageStream = await file.OpenAsync(FileAccessMode.Read))
                {
                    CoreWindow coreWindow = CoreWindow.GetForCurrentThread();
                    if (coreWindow != null)
                    {
                        await coreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                            {
                                try
                                {
                                    await bitmapImage.SetSourceAsync(_localImageStream.CloneStream());
                                }
                                catch (Exception ex)
                                {
                                    ErrorImage = true;
                                    ex.WriteLog();
                                }
                                bitmapImage.ImageFailed -= LocalBitmapImage_DisposeStream;
                                bitmapImage.ImageOpened -= LocalBitmapImage_DisposeStream;
                                bitmapImage.ImageFailed += LocalBitmapImage_DisposeStream;
                                bitmapImage.ImageOpened += LocalBitmapImage_DisposeStream;
                            });
                    }
                }
                OnPropertyChanged("LocalImage");
            }
            catch (Exception ex)
            {
                ex.WriteLog();
            }
        }
    }
}