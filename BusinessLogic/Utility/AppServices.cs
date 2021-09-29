using System.Threading.Tasks;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace ClassLibrary.Models.Utility
{
    public static class AppServices
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddSingleton<DiscordSocketClient>();
            return services;
        }
        
    }
}