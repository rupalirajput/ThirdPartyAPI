using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ThirdPartyAPI2.Entities.ModelDb
{
    public partial class ModelDbContext : DbContext
    {

        public virtual DbSet<BodyObject> BObject { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseInMemoryDatabase("ThirdPartyAPIMemoryDatabase");
            }

            /**
             * if we had to use a persistent store then we'd do something like
             * this and udpate the appsettings.json config with the DB connection string:
             */

            // IConfigurationRoot cfg = new ConfigurationBuilder()
            //     .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            //     .AddJsonFile("appsettings.json")
            //     .Build();
            //
            // optionsBuilder.UseSqlServer(cfg.GetConnectionString("DBConn"));

            // We'll have to embed the below objects to the appsettings.json file in the solution:
            //{
            //    "ConnectionStrings": {
            //        "DBConn": "Server=HOST; Database=DB_NAME"
            //    }
            //}
        }
    }
}
