using System;
using System.Collections.Generic;

namespace ClassLibrary.Models.ContextModels
{
    public interface IUserModel
    {
        Guid userGuid { get; set; }
        bool isBotAdmin { get; set; }
        bool hasLinkedAccount { get; set; }
        ulong userId { get; set; }
        string userNameEntry { get; set; }
        bool slowModeEnabled { get; set; }
        int slowModeTime { get; set; }
        
        string Id { get; set; }
        string UserName { get; set; }
        string NormalizedUserName { get; set; }
        string Email { get; set; }
        string NormalizedEmail { get; set; }
        bool EmailConfirmed { get; set; }
        string PasswordHash { get; set; }
        string SecurityStamp { get; set; }
        string ConcurrencyStamp { get; set; }
        string PhoneNumber { get; set; }
        bool PhoneNumberConfirmed { get; set; }
        bool TwoFactorEnabled { get; set; }
        DateTimeOffset? LockoutEnd { get; set; }
        bool LockoutEnabled { get; set; }
        int AccessFailedCount { get; set; }
        string ToString();
    }
}