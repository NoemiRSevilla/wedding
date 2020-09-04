using Microsoft.EntityFrameworkCore;

namespace Wedding.Models
{
    public class MyContext : DbContext
    {
        // base() calls the parent class' constructor passing the "options" parameter along
        public MyContext(DbContextOptions options) : base(options) { }
        public DbSet<User> users { get; set; }
        public DbSet<WeddPlan> weddPlans { get; set; }
        public DbSet<RSVP> rsvps { get; set; }
    }
}