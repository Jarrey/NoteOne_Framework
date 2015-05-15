using System;

#if WEB_SERVICE
namespace Chame.WebService.Helper.Models
{
#else
using NoteOne_Utility.Converters;

namespace NoteOne_ImageHelper.ImageSecurity
{
#endif
    public struct ImageAppCode
    {
        #region Static members

        /// <summary>
        ///     Default app code
        /// </summary>
        public static ImageAppCode Default
        {
            get { return new ImageAppCode(); }
        }

        /// <summary>
        ///     Validate app code string
        /// </summary>
        /// <param name="code">App code string</param>
        /// <returns></returns>
        public static bool Validate(string code)
        {
            string[] codes = code.Split("|".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            if (codes.Length != 5) return false;
            else return true;
        }

        #endregion

        #region Constructors

        public ImageAppCode(byte code1, byte code2, byte code3, byte code4, int offset)
        {
            CodePositionOffset = offset;
            Code1 = code1;
            Code2 = code2;
            Code3 = code3;
            Code4 = code4;
        }

        public ImageAppCode(string code)
        {
            string[] codes = code.Split("|".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            if (codes.Length != 5) throw new Exception("Wrong code format");

            CodePositionOffset = codes[0].StringToInt();
            Code1 = codes[1].StringToByte();
            Code2 = codes[2].StringToByte();
            Code3 = codes[3].StringToByte();
            Code4 = codes[4].StringToByte();
        }

        #endregion

        #region Fields

        public byte Code1;
        public byte Code2;
        public byte Code3;
        public byte Code4;

        /// <summary>
        ///     Offset of App code in bitmap area
        /// </summary>
        public int CodePositionOffset;

        #endregion

        #region Methods

        public override string ToString()
        {
            string format = "{0}|{1}|{2}|{3}|{4}";
            return string.Format(format,
                                 CodePositionOffset.ToString(),
                                 Code1.ToString(),
                                 Code2.ToString(),
                                 Code3.ToString(),
                                 Code4.ToString());
        }

        public string ToFileNameString()
        {
            string format = "{0}_{1}_{2}_{3}_{4}";
            return string.Format(format,
                                 CodePositionOffset.ToString(),
                                 Code1.ToString(),
                                 Code2.ToString(),
                                 Code3.ToString(),
                                 Code4.ToString());
        }

        public override bool Equals(object obj)
        {
            if (obj is ImageAppCode)
                return ToString() == obj.ToString();
            else return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #endregion

        #region Operators

        public static bool operator ==(ImageAppCode code1, ImageAppCode code2)
        {
            return code1.ToString() == code2.ToString();
        }

        public static bool operator !=(ImageAppCode code1, ImageAppCode code2)
        {
            return code1.ToString() != code2.ToString();
        }

        #endregion
    }
}