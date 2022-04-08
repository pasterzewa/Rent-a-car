using IdentityServer4.EntityFramework.Options;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Rent_a_Car.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rent_a_Car.Data
{
    public class ApplicationDbContext : ApiAuthorizationDbContext<ApplicationUser>
    {
        public ApplicationDbContext(
            DbContextOptions options,
            IOptions<OperationalStoreOptions> operationalStoreOptions) : base(options, operationalStoreOptions)
        {
        }

        public DbSet<Car> Car { get; set; }
        public DbSet<CarDetails> CarDetails { get; set; }
        public DbSet<Company> Company { get; set; }
        public DbSet<Customer> Customer { get; set; }
        public DbSet<Employer> Employer { get; set; }
        public DbSet<RentCarEvent> RentCar { get; set; }
        public DbSet<ReturnFile> ReturnFile { get; set; }
    }
}
