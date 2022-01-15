using ClinetOnline.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ClinetOnline.Extensions
{
    public static class HttpClientServiceCollection
    {
        public static IServiceCollection AddHttpClientServices(this IServiceCollection services)
        {
            services.AddHttpClient<IAppService, AppService>();

            return services;
        }
    }
}
