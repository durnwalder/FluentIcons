using System;
using Microsoft.Maui.Hosting;

namespace FluentIcons
{
    public static class MauiProgramExtensions
    {
        public static MauiAppBuilder UseFluentIcons(this MauiAppBuilder builder)
        {
            var assembly = typeof(MauiProgramExtensions).Assembly;
            var filledFontName = "FluentIcons.Resources.FluentSystemIcons-Filled.ttf";
            var regularFontName = "FluentIcons.Resources.FluentSystemIcons-Regular.ttf";

            if (assembly.GetManifestResourceInfo(filledFontName) == null || assembly.GetManifestResourceInfo(regularFontName) == null)
            {
                throw new InvalidOperationException("FluentIcons fonts are not embedded in the assembly.");
            }

            builder.ConfigureFonts(fonts =>
            {
                fonts.AddFont(filledFontName, "FluentFilled");
                fonts.AddFont(regularFontName, "FluentRegular");
            });

            return builder;
        }
    }
}
