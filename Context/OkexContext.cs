using Microsoft.EntityFrameworkCore;
using OKexTime.Models;

namespace OKexTime.Context
{
    public class OkexContext : DbContext
    {
        public DbSet<UsersRequest> Requests { get; set; }
        public DbSet<UsersRequestUSDT> RequestUsdts { get; set; }
        public OkexContext(DbContextOptions<OkexContext> options) : base(options) { }
    }
}
