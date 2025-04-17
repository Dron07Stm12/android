using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Hosting;
using System;
using System.IO.Ports;

namespace DronApp1
{
    public static class MauiProgram
    {

       // private SerialPort _serialPort;

        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            //builder
            //    .UseMauiApp<App>()
            //    .ConfigureFonts(fonts =>
            //    {
            //        fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            //        fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            //    });

            builder.UseMauiApp<App>().ConfigureFonts(delegate (IFontCollection fonts) {
              fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
              fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold"); 


            });

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
