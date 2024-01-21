using Microsoft.Maui.Controls;

namespace FluentIcons
{
    public static class IconHelper
    {
        public static FontImageSource GetFontImageSource(string glyph, string fontFamily)
        {
            return new FontImageSource
            {
                Glyph = glyph,
                FontFamily = fontFamily
            };
        }
    }
}