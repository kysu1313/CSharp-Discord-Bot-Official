using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ClassLibrary.Data;
using ClassLibrary.ModelDTOs;
using ClassLibrary.Models.ContextModels;
using Coinbase.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;


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
        [CascadingParameter]
        private Task<AuthenticationState> AuthenticationStateTask { get; set; }

        protected override async void OnInitialized()
        {
            var authState = await AuthenticationStateTask.ConfigureAwait(false);

            if (authState.User != null && authState.User.Identity.IsAuthenticated)
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
                }
            }
            base.OnInitialized();
        }

        protected async Task Submit(MouseEventArgs e)
        {
            _buttonState = "Clicked";
            var authState = await AuthenticationStateTask.ConfigureAwait(false);

            if (authState.User != null && authState.User.Identity.IsAuthenticated)
            {
                await using (var dto = new UserModelDTO(Context))
                {
                    var name = authState.User.Identity.Name;
                    var userModel = await dto.GetUser(name);
                    if (!userModel.hasLinkedAccount)
                    {
                        userModel.userId = ulong.TryParse(_userId, out ulong id) == false ? 0 : id;
                        userModel.hasLinkedAccount = true;
                        userModel.isBotAdmin = true;
                        await Context.SaveChangesAsync();
                    }
                    _success = true;
                }
            }
            
            // Console.WriteLine(user);
        }
    }
}