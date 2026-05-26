using Microsoft.Extensions.Logging;
using UTN.appMovil.MestroDetalle.Views;

namespace UTN.appMovil.MestroDetalle
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            builder.Services.AddSingleton<UTN.appMovil.MestroDetalle.Data.CategoriasRepository>();
            builder.Services.AddSingleton<UTN.appMovil.MestroDetalle.Data.ProductosRepository>();
            
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<CategoriasPage>();
            builder.Services.AddTransient<ProductosPage>();

            return builder.Build();
        }
    }
}
