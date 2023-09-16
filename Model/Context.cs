using Microsoft.EntityFrameworkCore;

namespace backend_app.Model
{
    public class Context: DbContext
    {
        public Context(DbContextOptions<Context> options)
        : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
    }
}
