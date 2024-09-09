using BlazorBooks.Mobile.Services;
using BlazorBooks.Shared.Interfaces;
using Microsoft.Extensions.Logging;
using Refit;

namespace BlazorBooks.Mobile
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
                });

            builder.Services.AddMauiBlazorWebView();

#if DEBUG
    		builder.Services.AddBlazorWebViewDeveloperTools();
    		builder.Logging.AddDebug();
#endif

            builder.Services.AddTransient<IBookService, ApiBookFetcher>();

            ConfigureRefit(builder.Services);

            return builder.Build();
        }

        private static void ConfigureRefit(IServiceCollection services)
        {
            var refitSettings = new RefitSettings
            {
                HttpMessageHandlerFactory = () =>
                {
#if ANDROID
                    return new Xamarin.Android.Net.AndroidMessageHandler
                    {
                        ServerCertificateCustomValidationCallback =
                        (httpRequestMessage, certificate, chain, sslPolicyErrors) =>
                                certificate?.Issuer == "CN-localhost"
                                || sslPolicyErrors == System.Net.Security.SslPolicyErrors.None
                    };
#elif IOS    
                    return new NSUrlSessionHandler
                    {
                        TrustOverrideForUrl = (nSUrlSessionHandler, url, SecTrust) =>
                            url.StartsWith("https://localhost")
                    };
#endif
                    return null;
                }
            };

            services.AddRefitClient<IBookApi>(refitSettings)
                    .ConfigureHttpClient(HttpClient =>
                    {
                        string baseUrl;
                        if(DeviceInfo.DeviceType == DeviceType.Physical)
                        {
                            baseUrl = "https://56v26nk1-7282.use.devtunnels.ms/";
                        }
                        else
                        {

                            baseUrl = "https://56v26nk1-7282.use.devtunnels.ms/";
                            //baseUrl = DeviceInfo.Platform == DevicePlatform.Android
                            //            ? "http://10.0.2.2:7282"
                            //            : "https://localhost:7282";
                        }

                    



                        HttpClient.BaseAddress = new Uri(baseUrl);
                    });
        }

    }
}
