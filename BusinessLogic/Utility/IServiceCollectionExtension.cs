using ClassLibrary.Data;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace ClassLibrary.Models.Utility
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddSingleton<DiscordSocketClient>();
            services.AddSingleton<CommandService>();
            services.AddTransient<CommandContext>();
            // services.AddSingleton<CommandHandler>();
            services.AddDbContext<ApplicationDbContext>();
            return services;
        }
    }
}