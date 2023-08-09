using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using ThirdPartyAPI2.Entities.ModelDb;

namespace ThirdPartyAPI2Test.Entities.ModelDb
{
    public partial class ModelDbContextMock : ModelDbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseInMemoryDatabase(Guid.NewGuid().ToString());
            }
        }
    }
}
