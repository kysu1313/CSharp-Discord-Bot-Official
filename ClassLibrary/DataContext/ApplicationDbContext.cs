using ClassLibrary.Models;
using ClassLibrary.Models.ContextModels;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ClassLibrary.Data
{
    public class ApplicationDbContext : IdentityDbContext<UserModel>
    {

        private static IConfiguration _config { get; set; }
        public DbContextOptions _options;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) //, IConfiguration config)
                : base(options)
        {
            //_config = config;
            _options = options;
            //_options.UseSqlite($"Data Source={_config.GetConnectionString("DefaultDb")}");
            IConfigurationBuilder _tmpConfig = new ConfigurationBuilder();
            _tmpConfig.AddJsonFile("appsettings.json");
            _config = _tmpConfig.Build();

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                //optionsBuilder.UseSqlite(_config.GetConnectionString("DefaultDb"));
                optionsBuilder.UseSqlServer(_config.GetConnectionString("DefaultDb"));

            }
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserModel>().HasKey(s => s.userId);
            modelBuilder.Entity<UserExperience>().HasKey(s => s.id);
            modelBuilder.Entity<ServerModel>().HasKey(s => s.serverId);
            modelBuilder.Entity<CryptoModel>().HasKey(s => s.id);
            modelBuilder.Entity<UserDash>().HasKey(s => s.id);
            modelBuilder.Entity<DashItem>().HasKey(s => s.id);
            modelBuilder.Entity<StatModel>().HasKey(s => s.id); 
            modelBuilder.Entity<UserStatsModel>().HasKey(s => s.id);
            modelBuilder.Entity<ReminderModel>().HasKey(s => s.id);
            modelBuilder.Entity<CommandModel>().HasKey(s => s.commandId);
            modelBuilder.Entity<ServerCommands>().HasKey(s => s.serverId);
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<UserModel> UserModels { get; set; }
        public DbSet<ServerModel> ServerModels { get; set; }
        public DbSet<UserExperience> UserExperiences { get; set; }
        public DbSet<CryptoModel> CryptoModels { get; set; }
        public DbSet<UserDash> UserDashes { get; set; }
        public DbSet<DashItem> DashItems { get; set; }
        public DbSet<StatModel> StatModels { get; set; }
        public DbSet<UserStatsModel> UserStatModels { get; set; }
        public DbSet<ReminderModel> ReminderModels { get; set; }
        public DbSet<CommandModel> CommandModels { get; set; }
        public DbSet<ServerCommands> ServerCommandModels { get; set; }

    }
}
