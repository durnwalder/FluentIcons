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
                fonts.AddEmbeddedResourceFont(typeof(MauiProgramExtensions).Assembly, FilledFontFamily, FilledFileName);
                fonts.AddEmbeddedResourceFont(typeof(MauiProgramExtensions).Assembly, RegularFontFamily, RegularFileName);
            });

            return builder;
        }
    }
}
