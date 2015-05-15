using System.Runtime.Serialization;

#if WEB_SERVICE
namespace ChameHOT.WebService.Helpers.Models
{
#else
namespace NoteOne_ImageHelper
{
#endif

#if !WEB_SERVICE
    [DataContract]
#endif
    public sealed class RenderingImageOption
    {
        #region Static members

        public static RenderingImageOption Default
        {
            get { return new RenderingImageOption(); }
        }

        #endregion

        #region Constructors

        public RenderingImageOption(float opacity = 1F,
                                    BitmapInterpolationMode interpolationMode = BitmapInterpolationMode.Linear)
        {
            Opacity = opacity;
            InterpolationMode = interpolationMode;
        }

        #endregion

        #region Properties

#if !WEB_SERVICE
        [DataMember]
#endif
        public float Opacity { get; set; }
#if !WEB_SERVICE
        [DataMember]
#endif
        public BitmapInterpolationMode InterpolationMode { get; set; }

#if !WEB_SERVICE
        [DataMember]
#endif
        public Thickness Margin { get; set; }

        #endregion
    }

    #region Enums and Values

    public enum BitmapInterpolationMode
    {
        /// <summary>
        ///     Use the exact color of the nearest bitmap pixel to the current rendering pixel.
        /// </summary>
        NearestNeighbor = 0,

        /// <summary>
        ///     Interpolate a color from the four bitmap pixels that are the nearest to the rendering pixel.
        /// </summary>
        Linear = 1
    }

    #endregion
}