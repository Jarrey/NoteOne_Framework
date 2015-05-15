using SharpDX;
using This = NoteOne_ImageHelper;
using DirectWrite = SharpDX.DirectWrite;

namespace NoteOne_ImageHelper
{
    public static class WinRTToDXTypeConverter
    {
        #region BitmapInterpolationMode extesion method

        public static SharpDX.Direct2D1.BitmapInterpolationMode ToDXType(this This.BitmapInterpolationMode mode)
        {
            switch (mode)
            {
                case This.BitmapInterpolationMode.Linear:
                    return SharpDX.Direct2D1.BitmapInterpolationMode.Linear;
                case This.BitmapInterpolationMode.NearestNeighbor:
                    return SharpDX.Direct2D1.BitmapInterpolationMode.NearestNeighbor;
            }
            return SharpDX.Direct2D1.BitmapInterpolationMode.Linear;
        }

        #endregion

        #region FontWeight extension method

        public static DirectWrite.FontWeight ToDXType(this This.FontWeight value)
        {
            if (value.Weight == This.FontWeights.Black.Weight)
                return DirectWrite.FontWeight.Black;
            if (value.Weight == This.FontWeights.Bold.Weight)
                return DirectWrite.FontWeight.Bold;
            if (value.Weight == This.FontWeights.ExtraBlack.Weight)
                return DirectWrite.FontWeight.ExtraBlack;
            if (value.Weight == This.FontWeights.ExtraBold.Weight)
                return DirectWrite.FontWeight.ExtraBold;
            if (value.Weight == This.FontWeights.ExtraLight.Weight)
                return DirectWrite.FontWeight.ExtraLight;
            if (value.Weight == This.FontWeights.Light.Weight)
                return DirectWrite.FontWeight.Light;
            if (value.Weight == This.FontWeights.Medium.Weight)
                return DirectWrite.FontWeight.Medium;
            if (value.Weight == This.FontWeights.Normal.Weight)
                return DirectWrite.FontWeight.Normal;
            if (value.Weight == This.FontWeights.SemiBold.Weight)
                return DirectWrite.FontWeight.SemiBold;
            if (value.Weight == This.FontWeights.SemiLight.Weight)
                return DirectWrite.FontWeight.SemiLight;
            if (value.Weight == This.FontWeights.Thin.Weight)
                return DirectWrite.FontWeight.Thin;
            return DirectWrite.FontWeight.Normal;
        }

        #endregion

        #region FontStyle extension method

        public static DirectWrite.FontStyle ToDXType(this This.FontStyle value)
        {
            switch (value)
            {
                case This.FontStyle.Italic:
                    return DirectWrite.FontStyle.Italic;
                case This.FontStyle.Normal:
                    return DirectWrite.FontStyle.Normal;
                case This.FontStyle.Oblique:
                    return DirectWrite.FontStyle.Oblique;
            }
            return DirectWrite.FontStyle.Normal;
        }

        #endregion

        #region FontStretch extension method

        public static DirectWrite.FontStretch ToDXType(this This.FontStretch value)
        {
            switch (value)
            {
                case This.FontStretch.Condensed:
                    return DirectWrite.FontStretch.Condensed;
                case This.FontStretch.Expanded:
                    return DirectWrite.FontStretch.Expanded;
                case This.FontStretch.ExtraCondensed:
                    return DirectWrite.FontStretch.ExtraCondensed;
                case This.FontStretch.ExtraExpanded:
                    return DirectWrite.FontStretch.ExtraExpanded;
                case This.FontStretch.Normal:
                    return DirectWrite.FontStretch.Normal;
                case This.FontStretch.SemiCondensed:
                    return DirectWrite.FontStretch.SemiCondensed;
                case This.FontStretch.SemiExpanded:
                    return DirectWrite.FontStretch.SemiExpanded;
                case This.FontStretch.UltraCondensed:
                    return DirectWrite.FontStretch.UltraCondensed;
                case This.FontStretch.UltraExpanded:
                    return DirectWrite.FontStretch.UltraExpanded;
                case This.FontStretch.Undefined:
                    return DirectWrite.FontStretch.Undefined;
            }
            return DirectWrite.FontStretch.Normal;
        }

        #endregion

        #region SolidColorBrush extension method

        public static Color4 ToDXType(this This.Color value)
        {
            var color = new Color4(value.R, value.G, value.B, value.A);
            return color;
        }

        #endregion

        #region FlowDirection extension method

        public static DirectWrite.FlowDirection ToDXType(this This.FlowDirection value)
        {
            switch (value)
            {
                case This.FlowDirection.TopToBottom:
                    return DirectWrite.FlowDirection.TopToBottom;
            }
            return DirectWrite.FlowDirection.TopToBottom;
        }

        #endregion

        #region ParagraphAlignment extension method

        public static DirectWrite.ParagraphAlignment ToDXType(this This.ParagraphAlignment value)
        {
            switch (value)
            {
                case This.ParagraphAlignment.Far:
                    return DirectWrite.ParagraphAlignment.Far;
                case This.ParagraphAlignment.Near:
                    return DirectWrite.ParagraphAlignment.Near;
                case This.ParagraphAlignment.Center:
                    return DirectWrite.ParagraphAlignment.Center;
            }
            return DirectWrite.ParagraphAlignment.Near;
        }

        #endregion

        #region ReadingDirection extension method

        public static DirectWrite.ReadingDirection ToDXType(this This.ReadingDirection value)
        {
            switch (value)
            {
                case This.ReadingDirection.LeftToRight:
                    return DirectWrite.ReadingDirection.LeftToRight;
                case This.ReadingDirection.RightToLeft:
                    return DirectWrite.ReadingDirection.RightToLeft;
            }
            return DirectWrite.ReadingDirection.LeftToRight;
        }

        #endregion

        #region WordWrapping extension method

        public static DirectWrite.WordWrapping ToDXType(this This.WordWrapping value)
        {
            switch (value)
            {
                case This.WordWrapping.NoWrap:
                    return DirectWrite.WordWrapping.NoWrap;
                case This.WordWrapping.Wrap:
                    return DirectWrite.WordWrapping.Wrap;
            }
            return DirectWrite.WordWrapping.Wrap;
        }

        #endregion

        #region TextAlignment extension method

        public static DirectWrite.TextAlignment ToDXType(this This.TextAlignment value)
        {
            switch (value)
            {
                case This.TextAlignment.Center:
                    return DirectWrite.TextAlignment.Center;
                case This.TextAlignment.Justified:
                    return DirectWrite.TextAlignment.Justified;
                case This.TextAlignment.Leading:
                    return DirectWrite.TextAlignment.Leading;
                case This.TextAlignment.Trailing:
                    return DirectWrite.TextAlignment.Trailing;
            }
            return DirectWrite.TextAlignment.Leading;
        }

        #endregion
    }
}