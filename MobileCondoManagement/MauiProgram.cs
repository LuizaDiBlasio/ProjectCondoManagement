using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using MobileCondoManagement.Models;
using MobileCondoManagement.Services;
using MobileCondoManagement.Services.Interfaces;
using MobileCondoManagement.ViewModels;
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

            // Serviços
            builder.Services.AddHttpClient<IApiService, ApiService>(client =>
            {
                client.BaseAddress = new Uri("https://10.0.2.2:7001/");
            })
              .ConfigurePrimaryHttpMessageHandler(() =>
              {
                  return new HttpClientHandler
                  {
                      ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
                  };
              });

            // Views e Models
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<CondoMemberDashboardPage>();
            builder.Services.AddTransient<CondoMemberDashboardViewModel>();

            builder.Services.AddTransient<ForgotPasswordViewModel>();
            builder.Services.AddTransient<ForgotPasswordPage>(); // Ensure ForgotPasswordPage exists in the MobileCondoManagement.Views namespace

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
