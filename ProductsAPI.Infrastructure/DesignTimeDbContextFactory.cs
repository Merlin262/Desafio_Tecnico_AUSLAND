using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using ProductsAPI.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProductsAPI.Infrastructure
{
    public class DesignTimeDbContextFactory
    : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

            optionsBuilder.UseNpgsql(
                "Host=localhost;Port=5432;Username=postgres;Password=Ns4YQ}Um9)ZQxa5G{aTZ4C;Database=ProductsDb");

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
