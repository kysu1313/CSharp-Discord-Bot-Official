using ClassLibrary.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.DataContext
{
    public class AppDataContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {

        ApplicationDbContext IDesignTimeDbContextFactory<ApplicationDbContext>.CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            var config = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory) //   <---- UNCOMMENT FOR MIGRATIONS
                // .SetBasePath(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)) // <-- COMMENT THIS LINE FOR MIGRATIONS
                .AddJsonFile(path: "appsettings.json")
                .Build();

            optionsBuilder.UseSqlServer(config.GetConnectionString("DefaultDb"));

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}
