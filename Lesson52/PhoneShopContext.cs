using Lesson52.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Lesson52
{
    public class PhoneShopContext: DbContext
    {
        public DbSet<Phone> Phones { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Review> Reviews { get; set; }


        public PhoneShopContext(DbContextOptions<PhoneShopContext> options)
                : base(options)
        {

        }
    }
}
