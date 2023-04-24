using Microsoft.EntityFrameworkCore;
using InzynierkaAPI.Models;

namespace InzynierkaAPI.Data
{
    public class DataContext : DbContext
    {
		public DataContext()
		{
		}

		public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<Przetarg> Przetarg { get; set; }

        public DbSet<Plik> Plik { get; set; }

        public DbSet<Produkt> Produkt { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<Dostawca> Dostawca { get; set; }
        public DbSet<Producent> Producent { get; set; }
        public DbSet<WystawcaPrzetargu> WystawcaPrzetargu { get; set; }
        public DbSet<UserLogin> UserLogin { get; set; }
        public DbSet<RaportMagazynu> RaportMagazynu { get; set; }
        public DbSet<Ustawienia> Ustawienia { get; set; }
        public DbSet<Magazyn> Magazyn { get; set; }
        public DbSet<Strefa> Strefa { get; set; }
        public DbSet<KategoriaPlik> KategoriaPlik { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Przetarg>().HasMany(x => x.Pliki).WithOne().OnDelete(DeleteBehavior.Cascade);
        }
        
    }
}
