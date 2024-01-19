using System;
using Microsoft.Maui.Hosting;

namespace FluentIcons
{
    public static class MauiProgramExtensions
    {
        internal static readonly string FilledFontFamily = "FluentRegular";

        internal static readonly string FilledFileName = "Resources.FluentSystemIcons-Regular.ttf";

        internal static readonly string RegularFontFamily = "FluentFilled";
        
        internal static readonly string RegularFileName = "Resources.FluentSystemIcons-Filled.ttf";

        public static MauiAppBuilder UseFluentIcons(this MauiAppBuilder builder)
        {
            builder.ConfigureFonts(fonts =>
            {
                fonts.AddEmbeddedResourceFont(typeof(MauiProgramExtensions).Assembly, FilledFileName, FilledFontFamily);
                fonts.AddEmbeddedResourceFont(typeof(MauiProgramExtensions).Assembly, RegularFileName, RegularFontFamily);
            });

            return builder;
        }
    }
}
