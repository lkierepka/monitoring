using Microsoft.EntityFrameworkCore;
using WorkerService.Order;

namespace WorkerService
{
    public class WorkerDbContext : DbContext
    {
        public WorkerDbContext(DbContextOptions<WorkerDbContext> options) : base(options)
        {
        }

        public DbSet<OrderDao> Orders { get; set; }
    }
}