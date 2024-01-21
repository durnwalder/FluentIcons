using System;
using Microsoft.Maui.Hosting;

namespace FluentIcons
{
    public static class MauiProgramExtensions
    {
        public static MauiAppBuilder UseFluentIcons(this MauiAppBuilder builder)
        {
            builder.ConfigureFonts(fonts =>
            {
                fonts.AddEmbeddedResourceFont(typeof(MauiProgramExtensions).Assembly, FluentIconConstants.FilledFileName, FluentIconConstants.FilledFontFamily);
                fonts.AddEmbeddedResourceFont(typeof(MauiProgramExtensions).Assembly, FluentIconConstants.RegularFileName, FluentIconConstants.RegularFontFamily);
            });

            return builder;
        }
    }
}