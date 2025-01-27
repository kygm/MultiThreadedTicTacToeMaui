using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using MultiThreadedTicTacToeGui.Helpers;
using MultiThreadedTicTacToeGui.Views;

namespace MultithreadedTicTacToeGui
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder.UseMauiApp<App>().ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            }).UseMauiCommunityToolkit();

            builder.Services.AddSingleton<HomePage>();
            builder.Services.AddSingleton<HomePageVM>();
            //App Build Command: 
            //dotnet publish -f net7.0-windows10.0.19041.0 -c Release -p:RuntimeIdentifierOverride=win10-x64 -p:WindowsPackageType=None
#if DEBUG
            builder.Logging.AddDebug();
#endif
            var app = builder.Build();
            ServiceHelper.Initialize(app.Services);
            return app;
        }
    }
}
