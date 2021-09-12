using System;
using ClassLibrary.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Identity;


namespace BotDash.Models.PageModels
{
    public class LinkAccountModel : ComponentBase
    {
        protected string _userId;
        protected string _buttonState = "";
        [Inject] protected ApplicationDbContext _context { get; set; }

        protected void Submit(MouseEventArgs e)
        {
            _buttonState = "Clicked";
            Console.WriteLine(_userId);
        }
    }
}