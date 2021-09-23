using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ClassLibrary.Data;
using ClassLibrary.Helpers;
using ClassLibrary.ModelDTOs;
using ClassLibrary.Models.ContextModels;
using Coinbase.Models;
using Discord.WebSocket;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;


namespace BotDash.Models.PageModels
{
    public class LinkAccountModel : ComponentBase
    {
        protected string _userId;
        protected string _buttonState = "";
        protected bool _success = false;
        protected bool _isLoggedIn = false;
        [Inject] private IHttpContextAccessor  HttpContextAccessor { get; set; }
        [Inject] protected ApplicationDbContext Context { get; set; }
        [Inject] protected IServiceProvider Service { get; set; }
        [CascadingParameter]
        private Task<AuthenticationState> AuthenticationStateTask { get; set; }

        protected override async void OnInitialized()
        {
            var authState = await AuthenticationStateTask.ConfigureAwait(false);

            if (authState.User is { Identity: { IsAuthenticated: true } })
            {
                _isLoggedIn = true;
                using (var dto = new UserModelDTO(Context))
                {
                    var name = authState.User.Identity.Name;
                    var userModel = await dto.GetUser(name);
                    if (userModel.hasLinkedAccount)
                    {
                        _success = true;
                    }
                    
                    var socket = Service.GetService<DiscordSocketClient>();
                    
                    await using var helpDto = new Helper(Context, Service);
                    await dto.RegisterUser(name, ulong.TryParse(_userId, out ulong id) == false ? 0 : id);
                    await helpDto.RegisterUsersOwnServers(socket, id);
                    
                    
                }
            }
            base.OnInitialized();
        }

        protected async Task SubmitUsr(MouseEventArgs e)
        {
            _buttonState = "Clicked";
            var authState = await AuthenticationStateTask.ConfigureAwait(false);

            if (authState.User is { Identity: { IsAuthenticated: true } })
            {
                await using (var dto = new UserModelDTO(Context))
                {
                    var socket = Service.GetService<DiscordSocketClient>();
                    
                    var name = authState.User.Identity.Name;
                    var userModel = await dto.GetUser(name);
                    await using var helpDto = new Helper(Context, Service);
                    if (!userModel.hasLinkedAccount)
                    {
                        await dto.RegisterUser(name, ulong.TryParse(_userId, out ulong id) == false ? 0 : id);
                        await helpDto.RegisterUsersOwnServers(socket, id);
                    }
                    _success = true;
                }
            }
            
            // Console.WriteLine(user);
        }
    }
}