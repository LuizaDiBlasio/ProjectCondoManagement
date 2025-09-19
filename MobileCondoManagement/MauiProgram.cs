using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using MobileCondoManagement.Models;
using MobileCondoManagement.Services;
using MobileCondoManagement.Views;

namespace MobileCondoManagement
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            MauiAppBuilder mauiAppBuilder = builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            //Serviços
            builder.Services.AddHttpClient();
            builder.Services.AddSingleton<ApiService>();

            //Views e Models
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<CondoManagerDashboardPage>();
            builder.Services.AddTransient<CondoMemberDashboardViewModel>();


#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
