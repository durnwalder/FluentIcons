## Installation

To install FluentIcons, you can use the following command in the Package Manager Console:

```shell
Install-Package FluentIcons -Version 1.0.0
```

## Usage

After installing the FluentIcons package, you can use it in your .NET MAUI app like this:

First, in your `MauiProgram.cs` file, add the `UseFontIcons` method in the `CreateHostBuilder` method:

```csharp
using Microsoft.Maui;
using Microsoft.Maui.Hosting;
using Microsoft.Maui.Controls.Hosting;
using FluentIcons;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseFluentIcons();

        return builder.Build();
    }
}
```

Then, in your .NET MAUI pages, you can use the FluentIcons like this:

```csharp
var iconLabel = new Label
{
    FontFamily = "FluentRegular", // or "FluentFilled" for filled icons
    Text = FluentIcons.IconIc_fluent_access_time_20_filled
};
```

This will display the "access time" icon from the FluentIcons library.

## Contributing

Contributions are welcome! Please read our contributing guidelines to get started.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.