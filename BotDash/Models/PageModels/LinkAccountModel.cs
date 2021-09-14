using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ClassLibrary.Data;
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
        [Inject] private IHttpContextAccessor  HttpContextAccessor { get; set; }
        [Inject] protected ApplicationDbContext Context { get; set; }
        [CascadingParameter]
        private Task<AuthenticationState> AuthenticationStateTask { get; set; }

        protected async Task Submit(MouseEventArgs e)
        {
            _buttonState = "Clicked";
            var authState = await AuthenticationStateTask.ConfigureAwait(false);
            var user = authState.User.Identity.Name;
            var u =  await Context.UserModels.FirstOrDefaultAsync(x => x.UserName == user);
            u.userId = ulong.TryParse(_userId, out ulong id) == false ? 0 : id;
            await Context.SaveChangesAsync();
            _success = true;
            // Console.WriteLine(user);
        }
    }
}