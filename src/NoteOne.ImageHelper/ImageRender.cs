using System;
using System.IO;
using System.Threading.Tasks;
using NoteOne_Utility.Extensions;
using SharpDX;
using SharpDX.DXGI;
using SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using SharpDX.WIC;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using AlphaMode = SharpDX.Direct2D1.AlphaMode;
using Bitmap = SharpDX.WIC.Bitmap;
using BitmapSource = SharpDX.WIC.BitmapSource;
using Factory = SharpDX.Direct2D1.Factory;
using PixelFormat = SharpDX.Direct2D1.PixelFormat;
using TextAntialiasMode = SharpDX.Direct2D1.TextAntialiasMode;

namespace NoteOne_ImageHelper
{
    public class ImageRender : IDisposable
    {
        #region Constructor and Dispose method

        public ImageRender(IRandomAccessStream stream)
        {
            ImageStream = stream;
        }

        public ImageRender(Uri imageUri)
        {
            Task<IRandomAccessStreamWithContentType> t =
                RandomAccessStreamReference.CreateFromUri(imageUri).OpenReadAsync().AsTask();
            t.Wait();
            if (t.Status == TaskStatus.RanToCompletion && t.Result != null)
                ImageStream = t.Result;
        }

        public void Dispose()
        {
            DisposeObject(_wicBitmap);
            DisposeObject(_renderTarget);
            DisposeObject(_dwFactory);
            DisposeObject(_d2DFactory);
            DisposeObject(_wicFactory);
            DisposeObject(_imageStream);
        }

        #endregion

        #region Fields

        private readonly Factory _d2DFactory = new Factory();
        private readonly SharpDX.DirectWrite.Factory _dwFactory = new SharpDX.DirectWrite.Factory();
        private readonly ImagingFactory2 _wicFactory = new ImagingFactory2();
        private IRandomAccessStream _imageStream;
        private bool _isDrawBegun;
        private WicRenderTarget _renderTarget;
        private Bitmap _wicBitmap;

        #endregion

        #region Properties

        /// <summary>
        ///     Get target image stream
        /// </summary>
        public IRandomAccessStream ImageStream
        {
            get
            {
                _imageStream.Seek(0);
                return _imageStream;
            }
            private set
            {
                try
                {
                    if (value == null)
                    {
                        throw new Exception("Value cannot be null");
                    }

                    if (value != _imageStream)
                    {
                        InitializeRenderComponents(
                            _wicFactory,
                            _d2DFactory,
                            value.AsStream(),
                            ref _wicBitmap,
                            ref _renderTarget);
                        value.Seek(0);
                        _imageStream = value;
                    }
                }
                catch (Exception ex)
                {
                    ex.WriteLog();
                    throw new Exception("Cannot set and initialize image stream", ex);
                }
            }
        }

        /// <summary>
        ///     Get target BitmapImage object
        /// </summary>
        public BitmapImage Image
        {
            get
            {
                if (ImageStream == null) return null;

                var bitmapImage = new BitmapImage();
                bitmapImage.SetSource(ImageStream);
                return bitmapImage;
            }
        }

        /// <summary>
        ///     Get target WriteableBitmap object
        /// </summary>
        public WriteableBitmap WriteableImage
        {
            get
            {
                if (ImageStream == null) return null;

                var bitmapImage = new WriteableBitmap(PixelWidth, PixelHeight);
                bitmapImage.SetSource(ImageStream);
                return bitmapImage;
            }
        }

        /// <summary>
        ///     Get target image width
        /// </summary>
        public int PixelWidth { get; private set; }

        /// <summary>
        ///     Get target image height
        /// </summary>
        public int PixelHeight { get; private set; }

        #endregion

        #region Public methods

        /// <summary>
        ///     Begin to draw content
        /// </summary>
        public void BeginDraw()
        {
            try
            {
                if (!Validate()) throw new Exception("The target bitmap is wrong");

                // Start to render
                _renderTarget.BeginDraw();
                _isDrawBegun = true;
            }
            catch (Exception ex)
            {
                ex.WriteLog();
                throw ex;
            }
        }

        /// <summary>
        ///     End of draw content
        /// </summary>
        /// <returns></returns>
        public async Task EndDraw()
        {
            try
            {
                if (!Validate()) throw new Exception("The target bitmap is wrong");

                // End of render
                _renderTarget.EndDraw();
                ImageStream = await GetSteamFromWicContent(_wicFactory, _wicBitmap);
                _isDrawBegun = false;
            }
            catch (Exception ex)
            {
                ex.WriteLog();
                throw ex;
            }
        }

        public Size MeansureText(string text, Rect rect, RenderingTextOption option = null, float ratio = 1.0F)
        {
            try
            {
                if (string.IsNullOrEmpty(text)) throw new ArgumentNullException("text");
                if (!Validate() || !_isDrawBegun) throw new Exception("The target bitmap is wrong");

                // use default option for drawing text
                if (option == null) option = RenderingTextOption.Default;

                var textFormat = new TextFormat(_dwFactory,
                                                option.FontFamilyName,
                                                option.FontWeight.ToDXType(),
                                                option.FontSytle.ToDXType(),
                                                option.FontSize / ratio)
                    {
                        FlowDirection = option.FlowDirection.ToDXType(),
                        IncrementalTabStop = option.IncrementalTabStop,
                        ParagraphAlignment = option.ParagraphAlignment.ToDXType(),
                        ReadingDirection = option.ReadingDirection.ToDXType(),
                        TextAlignment = option.TextAlignment.ToDXType(),
                        WordWrapping = option.WordWrapping.ToDXType()
                    };

                var margin = option.Margin;
                var textLayout = new TextLayout(_dwFactory, text, textFormat, (float)rect.Width, (float)rect.Height);
                var textMetrics = textLayout.Metrics;
                return new Size(textMetrics.Width + margin.Left + margin.Right,
                                textMetrics.Height + margin.Top + margin.Bottom);
            }
            catch (Exception ex)
            {
                ex.WriteLog();
                throw new Exception("Falied to render text on tergat image", ex);
            }
        }

        /// <summary>
        ///     Draw text on one image in specific rectangle with option
        /// </summary>
        /// <param name="text">Text to draw</param>
        /// <param name="rect">Rectangle to draw text</param>
        /// <param name="option">Text options</param>
        public Size DrawText(string text, Rect rect, RenderingTextOption option = null, float ratio = 1.0F)
        {
            try
            {
                if (string.IsNullOrEmpty(text)) throw new ArgumentNullException("text");
                if (!Validate() || !_isDrawBegun) throw new Exception("The target bitmap is wrong");

                // use default option for drawing text
                if (option == null) option = RenderingTextOption.Default;

                var textFormat = new TextFormat(_dwFactory,
                                                option.FontFamilyName,
                                                option.FontWeight.ToDXType(),
                                                option.FontSytle.ToDXType(),
                                                option.FontSize / ratio)
                    {
                        FlowDirection = option.FlowDirection.ToDXType(),
                        IncrementalTabStop = option.IncrementalTabStop,
                        ParagraphAlignment = option.ParagraphAlignment.ToDXType(),
                        ReadingDirection = option.ReadingDirection.ToDXType(),
                        TextAlignment = option.TextAlignment.ToDXType(),
                        WordWrapping = option.WordWrapping.ToDXType()
                    };
                var textBrush = new SolidColorBrush(
                    _renderTarget,
                    option.Foreground.ToDXType());

                var textLayout = new TextLayout(_dwFactory, text, textFormat, (float)rect.Width, (float)rect.Height);
                var textMetrics = textLayout.Metrics;

                var margin = option.Margin;

                // Check alignment of text
                var rectangle = new RectangleF((float)(rect.Left + margin.Left),
                                               (float)(rect.Top + margin.Top),
                                               (float)(rect.Width + margin.Left + margin.Right),
                                               (float)(rect.Height + margin.Top + margin.Bottom));
                switch (option.HorizontalAlignment)
                {
                    case TextHorizontalAlignment.Right:
                        rectangle = new RectangleF((float)(rect.Left + margin.Left + rect.Width - textMetrics.Width),
                                                   (float)(rect.Top + margin.Top),
                                                   (float)(rect.Width + margin.Left + margin.Right),
                                                   (float)(rect.Height + margin.Top + margin.Bottom));
                        break;
                    case TextHorizontalAlignment.Center:
                        rectangle = new RectangleF((float)(rect.Left + margin.Left + (rect.Width - textMetrics.Width) / 2.0),
                                                   (float)(rect.Top + margin.Top),
                                                   (float)(rect.Width + margin.Left + margin.Right),
                                                   (float)(rect.Height + margin.Top + margin.Bottom));
                        break;
                }

                _renderTarget.DrawText(text, textFormat, rectangle, textBrush);

                return new Size(textMetrics.Width + margin.Left + margin.Right, textMetrics.Height + margin.Top + margin.Bottom);
            }
            catch (Exception ex)
            {
                ex.WriteLog();
                throw new Exception("Falied to render text on tergat image", ex);
            }
        }

        /// <summary>
        ///     Draw image on one image in specific rectangle with option
        /// </summary>
        /// <param name="stream">Image to render</param>
        /// <param name="rect">Rectangle to render image</param>
        /// <param name="option">Image rendering options</param>
        /// <returns></returns>
        public void DrawImage(IRandomAccessStream stream, Rect rect, RenderingImageOption option = null)
        {
            SharpDX.Direct2D1.Bitmap renderingBitmap = null;
            try
            {
                if (stream == null) throw new ArgumentNullException("stream");
                if (!Validate() || !_isDrawBegun) throw new Exception("The target bitmap is wrong");

                // use default option for drawing image
                if (option == null) option = RenderingImageOption.Default;

                using (BitmapSource renderingBitmapSource = LoadBitmap(_wicFactory, stream.AsStream()))
                {
                    renderingBitmap = SharpDX.Direct2D1.Bitmap.FromWicBitmap(_renderTarget, renderingBitmapSource);
                }

                _renderTarget.DrawBitmap(
                    renderingBitmap,
                    new RectangleF((float)rect.Left, (float)rect.Top, (float)rect.Right, (float)rect.Bottom),
                    option.Opacity,
                    option.InterpolationMode.ToDXType());
            }
            catch (Exception ex)
            {
                ex.WriteLog();
                throw new Exception("Falied to render image on tergat image", ex);
            }
            finally
            {
                DisposeObject(renderingBitmap);
            }
        }

        /// <summary>
        ///     Draw image on one image in specific rectangle with option
        /// </summary>
        /// <param name="uri">Image uri to render</param>
        /// <param name="rect">Rectangle to render image</param>
        /// <param name="option">Image rendering options</param>
        public void DrawImage(Uri uri, Rect rect, RenderingImageOption option = null)
        {
            try
            {
                if (uri == null) throw new ArgumentNullException("uri");
                Task<IRandomAccessStreamWithContentType> t =
                    RandomAccessStreamReference.CreateFromUri(uri).OpenReadAsync().AsTask();
                t.Wait();
                if (t.Status == TaskStatus.RanToCompletion && t.Result != null)
                    DrawImage(t.Result, rect, option);
            }
            catch (Exception ex)
            {
                ex.WriteLog();
                throw ex;
            }
        }

        /// <summary>
        ///     Get image in specific region in target image
        /// </summary>
        /// <param name="targetImageStream"></param>
        /// <param name="rect"></param>
        /// <returns></returns>
        public async Task<WriteableBitmap> GetImageFromImage(Rect rect)
        {
            try
            {
                WriteableBitmap wrtibleBitmap =
                    await (BitmapFactory.New(PixelWidth, PixelHeight)).FromStream(ImageStream);
                return wrtibleBitmap.Crop(rect);
            }
            catch (Exception ex)
            {
                ex.WriteLog();
                throw new Exception("Falied to get image in tergat image", ex);
            }
        }

        /// <summary>
        ///     Get image stream in specific region in target image
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        public async Task<IRandomAccessStream> GetImageStreamFromImage(Rect rect)
        {
            try
            {
                WriteableBitmap bitmap = await GetImageFromImage(rect);
                return await bitmap.ToRandomAccessStreamAsync();
            }
            catch (Exception ex)
            {
                ex.WriteLog();
                throw new Exception("Falied to get image in tergat image", ex);
            }
        }

        #endregion

        #region Private help methods

        /// <summary>
        ///     Validate current object
        /// </summary>
        /// <returns></returns>
        private bool Validate()
        {
            return _wicBitmap != null && _renderTarget != null;
        }

        /// <summary>
        ///     Initialize render components
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="d2DFactory"></param>
        /// <param name="wicBitmap"></param>
        /// <param name="renderTarget"></param>
        /// <param name="sourceStream"></param>
        private void InitializeRenderComponents(
            ImagingFactory2 factory,
            Factory d2DFactory,
            Stream sourceStream,
            ref Bitmap wicBitmap,
            ref WicRenderTarget renderTarget)
        {
            try
            {
                using (BitmapSource bitmapSource = LoadBitmap(factory, sourceStream))
                {
                    DisposeObject(wicBitmap);
                    wicBitmap = new Bitmap(factory, bitmapSource, BitmapCreateCacheOption.CacheOnLoad);
                    PixelWidth = _wicBitmap.Size.Width;
                    PixelHeight = _wicBitmap.Size.Height;
#if !WIN8
                    var renderTargetProperties = new RenderTargetProperties(
                        RenderTargetType.Default,
                        new PixelFormat(Format.Unknown, AlphaMode.Unknown),
                        DisplayInformation.GetForCurrentView().LogicalDpi,
                        DisplayInformation.GetForCurrentView().LogicalDpi,
                        RenderTargetUsage.None,
                        FeatureLevel.Level_DEFAULT);
#else
                    var renderTargetProperties = new RenderTargetProperties(
                        RenderTargetType.Default,
                        new PixelFormat(Format.Unknown, AlphaMode.Unknown),
                        DisplayProperties.LogicalDpi, // DisplayProperties is obsoleted
                        DisplayProperties.LogicalDpi,
                        RenderTargetUsage.None,
                        FeatureLevel.Level_DEFAULT);
#endif
                    DisposeObject(renderTarget);
                    renderTarget = new WicRenderTarget(d2DFactory, wicBitmap, renderTargetProperties)
                        {
                            AntialiasMode = AntialiasMode.Aliased,
                            TextAntialiasMode = TextAntialiasMode.Cleartype
                        };
                }
            }
            catch (Exception ex)
            {
                ex.WriteLog();
                throw ex;
            }
        }

        /// <summary>
        ///     Load bitmap from stream to BitmapSource
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
        private BitmapSource LoadBitmap(ImagingFactory2 factory, Stream stream)
        {
            try
            {
                BitmapDecoder bitmapDecoder = new BitmapDecoder(
                    factory,
                    stream,
                    DecodeOptions.CacheOnDemand);
                FormatConverter formatConverter = new FormatConverter(factory);
                formatConverter.Initialize(
                    bitmapDecoder.GetFrame(0),
                    SharpDX.WIC.PixelFormat.Format32bppPRGBA,
                    BitmapDitherType.None,
                    null,
                    0.0,
                    BitmapPaletteType.Custom);
                return formatConverter;
            }
            catch (Exception ex)
            {
                ex.WriteLog();
                throw new Exception("Cannot load bitmap", ex);
            }
        }

        /// <summary>
        ///     Generate RandomAccessStream from WicBitmap content
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        private async Task<IRandomAccessStream> GetSteamFromWicContent(ImagingFactory2 factory, Bitmap bitmap)
        {
            var ms = new MemoryStream();
            var wicStream = new WICStream(factory, ms);
            try
            {
                using (BitmapEncoder encoder = new PngBitmapEncoder(factory))
                {
                    encoder.Initialize(wicStream);
                    using (var frameEncoder = new BitmapFrameEncode(encoder))
                    {
                        frameEncoder.Initialize();
                        frameEncoder.SetSize(bitmap.Size.Width, bitmap.Size.Height);
                        frameEncoder.WriteSource(bitmap);
                        frameEncoder.Commit();
                        encoder.Commit();
                    }
                }
                ms.Position = 0;
                return await ToRandomAccessStream(ms);
            }
            catch (Exception ex)
            {
                ex.WriteLog();
                throw new Exception("Cannot convert WicBitmap to RandomAccessStream", ex);
            }
            finally
            {
                DisposeObject(wicStream);
                DisposeObject(ms);
            }
        }

        /// <summary>
        ///     Convert Stream to RandomAccessStream
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        private async Task<IRandomAccessStream> ToRandomAccessStream(Stream stream)
        {
            IRandomAccessStream returnStream = new InMemoryRandomAccessStream();
            try
            {
                using (IInputStream inputStream = stream.AsInputStream())
                {
                    await RandomAccessStream.CopyAsync(inputStream, returnStream);
                }
                returnStream.Seek(0);
            }
            catch (Exception ex)
            {
                ex.WriteLog();
                throw new Exception("Cannot convert image stream to RandomAccessStream", ex);
            }
            return returnStream;
        }

        /// <summary>
        ///     Help to dispose object and release referenced native resources
        /// </summary>
        /// <param name="obj">object to dispose</param>
        private void DisposeObject(IDisposable obj)
        {
            if (obj != null)
            {
                obj.Dispose();
                obj = null;
            }
        }

        #endregion
    }
}