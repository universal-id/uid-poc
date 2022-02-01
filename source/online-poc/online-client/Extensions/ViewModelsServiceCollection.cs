using ClinetOnline.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace ClinetOnline.Extensions
{
    public static class ViewModelsServiceCollection
    {
        public static IServiceCollection AddViewModels(this IServiceCollection services)
        {
            services.AddSingleton<MainWindowViewModel>();
            services.AddSingleton<StartViewModel>();
            services.AddSingleton<ContactsViewModel>();
            services.AddSingleton<ConnectViewModel>();
            services.AddSingleton<AddEditContactViewModel>();


            return services;
        }
    }
}
