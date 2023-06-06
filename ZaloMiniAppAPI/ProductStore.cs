
using Microsoft.EntityFrameworkCore;
using ZaloMiniAppAPI;


namespace ZaloMiniAppAPI
{
    public class ProductStore: DbContext
    {
        public ProductStore(DbContextOptions<ProductStore> options) : base(options)
        {

        }
        public DbSet<ZaloMiniAppAPI.Products>? Products { get; set; }
        public DbSet<ZaloMiniAppAPI.CartItems>? CartItems { get; set; }
        public DbSet<ZaloMiniAppAPI.Orders>? Orders { get; set; }
        public DbSet<ZaloMiniAppAPI.ProDuctOrders>? ProDuctOrders { get; set; }
        public DbSet<ZaloMiniAppAPI.OrderDetail>? OrderDetail { get; set; }
        

        public bool IsDatabaseConnected()
        {
            try
            {
                return Database.CanConnect();
            }
            catch (Exception)
            {
                return false;
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Orders>()
                .Property(o => o.ID)
                .UseIdentityColumn();
            

            modelBuilder.Entity<AccoutDetail>(entity =>
            {
                entity.ToTable("AccoutDetail"); // Tên bảng
                entity.Property(e => e.CustomerId).HasColumnName("CustomerId"); 
                 // Các cột khác...
            });
            base.OnModelCreating(modelBuilder);
        }
        public DbSet<ZaloMiniAppAPI.AccoutDetail>? AccoutDetail { get; set; }


    }
}
