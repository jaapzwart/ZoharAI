﻿using Microsoft.Extensions.Logging;
using Plugin.Maui.Audio;
using ZoharBible;
using CommunityToolkit.Maui;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;

namespace ZoharBible;
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>().ConfigureFonts(fonts =>
        {
            fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
        }).UseMauiCommunityToolkitMediaElement();
        builder.Services.AddSingleton<IAudioService, AudioService>();
#if DEBUG
        builder.Logging.AddDebug();
#endif
        return builder.Build();
    }
}