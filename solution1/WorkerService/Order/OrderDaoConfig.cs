using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WorkerService.Order
{
    public class OrderDaoConfig : IEntityTypeConfiguration<OrderDao>
    {
        public void Configure(EntityTypeBuilder<OrderDao> builder)
        {
            builder.HasKey(order => order.Id);
            builder.Property(order => order.Id)
                .ValueGeneratedNever();
        }
    }
}