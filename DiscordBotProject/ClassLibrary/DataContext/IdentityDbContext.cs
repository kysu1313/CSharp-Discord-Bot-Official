using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace ClassLibrary.DataContext
{
    public class IdentityUserContext<UserModel>
        : IdentityUserContext<UserModel, string>
        where UserModel : IdentityUser
    {
    }
}