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
    public sealed class RenderingTextOption
    {
        #region Static members

        public static RenderingTextOption Default
        {
            get
            {
                return new RenderingTextOption
                    {
                        FontWeight = FontWeights.Normal,
                        Foreground = new Color(),
                        FlowDirection = FlowDirection.TopToBottom,
                        IncrementalTabStop = 4F,
                        ParagraphAlignment = ParagraphAlignment.Near,
                        ReadingDirection = ReadingDirection.LeftToRight,
                        TextAlignment = TextAlignment.Leading,
                        WordWrapping = WordWrapping.Wrap
                    };
            }
        }

        #endregion

        #region Constructors

        public RenderingTextOption(
            string fontFamilyName = "Segoe UI",
            FontStyle fontSytle = FontStyle.Normal,
            FontStretch fontStretch = FontStretch.Normal,
            float fontSize = 11.0F,
            float incrementalTabStop = 4F)
        {
            FontFamilyName = fontFamilyName;
            FontSytle = fontSytle;
            FontStretch = fontStretch;
            FontSize = fontSize;
            IncrementalTabStop = incrementalTabStop;
        }

        #endregion

        #region Properties
#if !WEB_SERVICE
        [DataMember]
#endif
        public string FontFamilyName { get; set; }
#if !WEB_SERVICE
        [DataMember]
#endif
        public FontWeight FontWeight { get; set; }
#if !WEB_SERVICE
        [DataMember]
#endif
        public FontStyle FontSytle { get; set; }
#if !WEB_SERVICE
        [DataMember]
#endif
        public FontStretch FontStretch { get; set; }
#if !WEB_SERVICE
        [DataMember]
#endif
        public Color Foreground { get; set; }
#if !WEB_SERVICE
        [DataMember]
#endif
        public float FontSize { get; set; }
#if !WEB_SERVICE
        [DataMember]
#endif
        public FlowDirection FlowDirection { get; set; }
#if !WEB_SERVICE
        [DataMember]
#endif
        public float IncrementalTabStop { get; set; }
#if !WEB_SERVICE
        [DataMember]
#endif
        public ParagraphAlignment ParagraphAlignment { get; set; }
#if !WEB_SERVICE
        [DataMember]
#endif
        public ReadingDirection ReadingDirection { get; set; }
#if !WEB_SERVICE
        [DataMember]
#endif
        public TextAlignment TextAlignment { get; set; }
#if !WEB_SERVICE
        [DataMember]
#endif
        public WordWrapping WordWrapping { get; set; }
#if !WEB_SERVICE
        [DataMember]
#endif
        public TextHorizontalAlignment HorizontalAlignment { get; set; }

#if !WEB_SERVICE
        [DataMember]
#endif
        public Thickness Margin { get; set; }

        #endregion
    }

    #region Enums and Values

    // Summary:
    //     Represents the style of a font face (for example, normal or italic).
    public enum FontStyle
    {
        // Summary:
        //     Represents a normal font style.
        Normal = 0,
        //
        // Summary:
        //     Represents an oblique font style.
        Oblique = 1,
        //
        // Summary:
        //     Represents an italic font style.
        Italic = 2,
    }

    public enum TextHorizontalAlignment
    {
        Left = 0,
        Center = 1,
        Right = 2
    }

    public enum FlowDirection
    {
        /// <summary>
        ///     Specifies that text lines are placed from top to bottom.
        /// </summary>
        TopToBottom = 0
    }

    public enum ParagraphAlignment
    {
        /// <summary>
        ///     The top of the text flow is aligned to the top edge of the layout box.
        /// </summary>
        Near = 0,

        /// <summary>
        ///     The bottom of the text flow is aligned to the bottom edge of the layout box.
        /// </summary>
        Far = 1,

        /// <summary>
        ///     The center of the flow is aligned to the center of the layout box.
        /// </summary>
        Center = 2
    }

    /// <summary>
    ///     Specifies the direction in which reading progresses.
    /// </summary>
    public enum ReadingDirection
    {
        /// <summary>
        ///     Indicates that reading progresses from left to right.
        /// </summary>
        LeftToRight = 0,

        /// <summary>
        ///     Indicates that reading progresses from right to left.
        /// </summary>
        RightToLeft = 1
    }

    /// <summary>
    ///     Specifies the word wrapping to be used in a particular multiline paragraph.
    /// </summary>
    public enum WordWrapping
    {
        /// <summary>
        ///     Indicates that words are broken across lines to avoid text overflowing the layout box.
        /// </summary>
        Wrap = 0,

        /// <summary>
        ///     Indicates that words are kept within the same line even when it overflows
        ///     the layout box. This option is often used with scrolling to reveal overflow text.
        /// </summary>
        NoWrap = 1
    }

    public enum TextAlignment
    {
        Leading = 0,
        Trailing = 1,
        Center = 2,
        Justified = 3
    }

    // Summary:
    //     Describes the degree to which a font has been stretched, compared to the
    //     normal aspect ratio of that font.
    public enum FontStretch
    {
        // Summary:
        //     No defined font stretch.
        Undefined = 0,
        //
        // Summary:
        //     An ultra-condensed font stretch (50% of normal).
        UltraCondensed = 1,
        //
        // Summary:
        //     An extra-condensed font stretch (62.5% of normal).
        ExtraCondensed = 2,
        //
        // Summary:
        //     A condensed font stretch (75% of normal).
        Condensed = 3,
        //
        // Summary:
        //     A semi-condensed font stretch (87.5% of normal).
        SemiCondensed = 4,
        //
        // Summary:
        //     The normal font stretch that all other font stretch values relate to (100%).
        Normal = 5,
        //
        // Summary:
        //     A semi-expanded font stretch (112.5% of normal).
        SemiExpanded = 6,
        //
        // Summary:
        //     An expanded font stretch (125% of normal).
        Expanded = 7,
        //
        // Summary:
        //     An extra-expanded font stretch (150% of normal).
        ExtraExpanded = 8,
        //
        // Summary:
        //     An ultra-expanded font stretch (200% of normal).
        UltraExpanded = 9,
    }

    // Summary:
    //     Refers to the density of a typeface, in terms of the lightness or heaviness
    //     of the strokes.
    public struct FontWeight
    {
        public FontWeight(ushort weight)
        {
            this.Weight = weight;
        }
        public ushort Weight;
    }

    public sealed class FontWeights
    {
        public static FontWeight Black { get { return new FontWeight(900); } }
        public static FontWeight Bold { get { return new FontWeight(700); } }
        public static FontWeight ExtraBlack { get { return new FontWeight(950); } }
        public static FontWeight ExtraBold { get { return new FontWeight(800); } }
        public static FontWeight ExtraLight { get { return new FontWeight(200); } }
        public static FontWeight Light { get { return new FontWeight(300); } }
        public static FontWeight Medium { get { return new FontWeight(500); } }
        public static FontWeight Normal { get { return new FontWeight(400); } }
        public static FontWeight SemiBold { get { return new FontWeight(600); } }
        public static FontWeight SemiLight { get { return new FontWeight(200); } }
        public static FontWeight Thin { get { return new FontWeight(100); } }
    }

    public struct Color
    {
        public byte A;
        public byte R;
        public byte G;
        public byte B;
        public Color(byte a, byte r, byte g, byte b)
        {
            A = a;
            R = r;
            G = g;
            B = b;
        }
    }

    #endregion
}