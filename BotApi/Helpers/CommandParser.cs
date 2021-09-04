using ClassLibrary.Data;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotApi.Helpers
{
    public class CommandParser
    {

        private readonly ApplicationDbContext _context;
        private readonly Apis _apis;
        private readonly Helper _helper;
        private readonly string[] _coins = 
        { 
            "AUR", "BCH", "BTC", "DASH", "DOGE", "EOS", 
            "ETC", "ETH", "GRC", "LTC", "MZC", "NANO", 
            "NEO", "NMC", "NXT", "POT", "PPC", "TIT", 
            "USDC", "USDT", "VTC", "XEM", "XLM", "XMR", 
            "XPM", "XRP", "XVG", "ZEC" 
        };
        private readonly string[] _greetings = 
        { 
            "hiya", "hi", "hey", "yo", "hello", "whats up", 
            "what\'s up", "yoo", "yooo", "sup", "ayo", 
            "ayoo", "howdy" 
        };



        public CommandParser(ApplicationDbContext context, IServiceProvider services)
        {
            _context = context;
            _apis = new Apis(context, services);
            _helper = new Helper(context, services);
        }

        public async Task ParseMessage(SocketMessage rawMessage, ICommandContext context)
        {
            StringBuilder sb = new StringBuilder();
            string[] splitMsg = rawMessage.Content.Split(" ");

            if (splitMsg.Contains("bitch")){
                await context.Channel.SendMessageAsync("fuck you bitch");
            }

            if (splitMsg.Contains("🔪"))
            {
                await context.Channel.SendMessageAsync("🔫🙂");
            }

            if (splitMsg.Contains("kms") || splitMsg.Contains("KMS") || splitMsg.Contains("Kms"))
            {
                await context.Channel.SendMessageAsync("🙂  Here, take this 🔫");
            }

            if (splitMsg.Contains("kys"))
            {
                await context.Channel.SendMessageAsync("Use this 🔪    🙂");
            }

            foreach (string item in splitMsg)
            {
                if (_coins.Contains(item.ToUpper()))
                {
                    var result = await _apis.CryptoApi(item.ToUpper());
                    await context.Channel.SendMessageAsync($"Current price of {item.ToUpper()} is {result}");
                }

                if (_greetings.Contains(item))
                {
                    await context.Channel.SendMessageAsync("Hi 🙂");
                }
            }

            if (context.Guild != null)
            {
                await _helper.UpdateUser(context.Guild, iusr: context.User);
            }

        }
    }
}
