using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Project2IdentityEmail.Entities;

namespace Project2IdentityEmail.Context
{
    public class EmailContext:IdentityDbContext<AppUser>
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=DESKTOP-UGIR0F4\\SQLEXPRESS;initial catalog=Project2EmailDb;integrated security=true");
        }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Category> Categories { get; set; }

    }
}
