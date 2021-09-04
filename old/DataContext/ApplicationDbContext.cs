using DiscBotConsole.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DiscBotConsole.Data
{
    public class ApplicationDbContext : DbContext
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
            // Duplicate here any configuration sources you use.
            _tmpConfig.AddJsonFile("appsettings.json");
            _config = _tmpConfig.Build();

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite(_config.GetConnectionString("DefaultDb"));
                //optionsBuilder.UseSqlServer(_config.GetConnectionString("DefaultDb"));  
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
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<UserModel> UserModels { get; set; }
        public DbSet<ServerModel> ServerModels { get; set; }
        public DbSet<UserExperience> UserExperiences { get; set; }
        public DbSet<CryptoModel> CryptoModels { get; set; }
        public DbSet<UserDash> UserDashes { get; set; }
        public DbSet<DashItem> DashItems { get; set; }
        

    }
}
