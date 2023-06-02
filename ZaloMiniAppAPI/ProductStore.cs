
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
        public DbSet<ZaloMiniAppAPI.Acount>? Acount { get; set; }

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
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<AccoutDetail>(entity =>
            {
                entity.ToTable("AccoutDetail"); // Tên bảng
                entity.Property(e => e.CustomerId).HasColumnName("CustomerId"); // Tên cột
                entity.Property(e => e.Sdt).HasColumnName("Sdt");
                entity.Property(e => e.Password).HasColumnName("Password");  // Các cột khác...
            });
        }
        public DbSet<ZaloMiniAppAPI.AccoutDetail>? AccoutDetail { get; set; }


    }
}
