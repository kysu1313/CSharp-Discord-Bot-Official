using System;
using System.Threading.Tasks;
using ClassLibrary.Data;
using BusinessLogic.Helpers;
using BusinessLogic.ModelDTOs;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Http;


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
                // using (var dto = new UserModelDTO(Context))
                // {
                //     var name = authState.User.Identity.Name;
                //     var userModel = await dto.GetUser(name, null);
                //     if (userModel.hasLinkedAccount)
                //     {
                //         _success = true;
                //     }

                    // await using var helpDto = new Helper(Context, Service);
                    // var uid = ulong.TryParse(_userId, out ulong id) == false ? 0 : id;
                    // await dto.RegisterUser(userModel.userId, id);
                    // await helpDto.RegisterUsersOwnServers(uid); 
                    
                    
                // }
            }
            base.OnInitialized();
        }

        protected async Task SubmitUsr(MouseEventArgs e)
        {
            _buttonState = "Clicked";
            var authState = await AuthenticationStateTask.ConfigureAwait(false);

            if (authState.User is { Identity: { IsAuthenticated: true } })
            {
                // await using (var dto = new UserModelDTO(Context))
                // {
                //     var name = authState.User.Identity.Name;
                //     await using var helpDto = new Helper(Context, Service);
                //     var uid = UInt64.Parse(_userId);
                //     var userModel = await dto.GetUser(name, uid);
                //     if (!userModel.hasLinkedAccount)
                //     {
                //         await dto.RegisterUser(name, uid);
                // await helpDto.RegisterUsersOwnServers(name, uid);
                //     }
                //     _success = true;
                // }
            }
            
            // Console.WriteLine(user);
        }
    }
}