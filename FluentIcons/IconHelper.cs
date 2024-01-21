using Microsoft.Maui.Controls;

namespace FluentIcons
{
    public static class IconHelper
    {
        public static FontImageSource GetFontImageSource(string iconName, string fontFile)
        {
            var glyph = typeof(Filled).GetField(iconName)?.GetValue(null) as string;
            if (glyph == null) return null;

            return new FontImageSource
            {
                Glyph = glyph,
                FontFamily = fontFile,
                Size = 24, // or any other size you want
            };
        }
    }
}