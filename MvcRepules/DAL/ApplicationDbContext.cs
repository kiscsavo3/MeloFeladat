using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Model;
using MvcRepules.Model;
using System;

namespace MvcRepules.DAL
{
    public class ApplicationDbContext : IdentityDbContext<User, Role, Guid>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<User> User { get; set; }
        public DbSet<PilotLog> PilotLog { get; set; }
        public DbSet<Flight> Flight { get; set; }
        public DbSet<Airport> Airport { get; set; }
        public DbSet<GlobalPoint> GlobalPoint { get; set; }
        public DbSet<Gps> Gps { get; set; }
    }
}
