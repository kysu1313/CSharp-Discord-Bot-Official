// using System;
// using ClassLibrary.Data;
// using ClassLibrary.Models.ContextModels;
// using Microsoft.AspNetCore.Hosting;
// using Microsoft.AspNetCore.Identity;
// using Microsoft.AspNetCore.Identity.UI;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.Extensions.Configuration;
// using Microsoft.Extensions.DependencyInjection;
//
// [assembly: HostingStartup(typeof(BotDash.Areas.Identity.IdentityHostingStartup))]
// namespace BotDash.Areas.Identity
// {
//     public class IdentityHostingStartup : IHostingStartup
//     {
//         public void Configure(IWebHostBuilder builder)
//         {
//             builder.ConfigureServices((context, services) => {
//                 services.AddDbContext<ApplicationDbContext>(options =>
//                     options.UseSqlServer(
//                         context.Configuration.GetConnectionString("DefaultDb")));
//
//                 services.AddDefaultIdentity<UserModel>(options => options.SignIn.RequireConfirmedAccount = true)
//                     .AddEntityFrameworkStores<ApplicationDbContext>();
//             });
//         }
//     }
// }