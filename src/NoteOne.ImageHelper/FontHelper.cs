using System.Collections.Generic;
using System.Linq;
using SharpDX.DirectWrite;

namespace NoteOne_ImageHelper
{
    public class FontHelper
    {
        public static IEnumerable<string> GetFontNames()
        {
            var fonts = new List<string>();

            // DirectWrite factory
            using (var factory = new Factory())
            {
                // Get font collections
                FontCollection fc = factory.GetSystemFontCollection(false);

                for (int i = 0; i < fc.FontFamilyCount; i++)
                {
                    // Get font family and add first name
                    FontFamily ff = fc.GetFontFamily(i);

                    string name = ff.FamilyNames.GetString(0);
                    fonts.Add(name);
                }
            }

            return fonts.OrderBy(f => f);
        }
    }
}