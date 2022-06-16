using Company_Management.Modules;
using Microsoft.EntityFrameworkCore;

namespace Company_Management.Context
{
    public class DataContext : DbContext
    {
        protected readonly IConfiguration Configuration;

        public DataContext(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            // connect to sql server database
            options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<FeedbackDetails> FeedbackDetails { get; set; }
    }
}