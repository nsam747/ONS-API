using Acre.Backend.Ons.Data.Entity;
using Microsoft.EntityFrameworkCore;
namespace Acre.Backend.Ons.Data
{
    // Don't use interface as that hides other aspects of the DbContext?
    // public interface IOnsDbContext {
    //     DbSet<Category> Categories { get; set; }
    //     DbSet<Subcategory> Subcategories { get; set; }
    //     DbSet<OnsByAge> OnsByAge { get;set; }
    //     DbSet<OnsByComposition> OnsByComposition { get;set; }
    //     DbSet<OnsByRegion> OnsByRegion { get;set; }
    //     DbSet<Region> Regions { get;set; }
    // }
    
    public class OnsDbContext : DbContext
    {
        public OnsDbContext(DbContextOptions<OnsDbContext> options) : base(options)
        {
        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Subcategory> Subcategories { get; set; }
        public DbSet<OnsByAge> OnsByAge {get;set;}
        public DbSet<OnsByComposition> OnsByComposition {get;set;}
        public DbSet<OnsByRegion> OnsByRegion {get;set;}
        public DbSet<Region> Regions {get;set;}
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Subcategory>().HasMany<OnsByRegion>(x => x.OnsByRegion).WithOne(ons => ons.Subcategory);
            modelBuilder.Entity<Subcategory>().HasMany<OnsByAge>(x => x.OnsByAge).WithOne(ons => ons.Subcategory);
            modelBuilder.Entity<Subcategory>().HasMany<OnsByComposition>(x => x.OnsByComposition).WithOne(ons => ons.Subcategory);

            modelBuilder.Entity<OnsByAge>().ToTable(new OnsByAge().TableName)
                .HasIndex(ons => new { ons.UpperBoundAge, ons.LowerBoundAge });
            modelBuilder.Entity<OnsByAge>().HasIndex(ons => ons.SubcategoryId);
            
            modelBuilder.Entity<OnsByComposition>().ToTable(new OnsByComposition().TableName)
                .HasIndex(ons => new { ons.EmploymentStatus, ons.DependantCount, ons.AdultCount });
            modelBuilder.Entity<OnsByComposition>().HasIndex(ons => ons.SubcategoryId);
            
            modelBuilder.Entity<OnsByRegion>().ToTable(new OnsByRegion().TableName)
                .HasIndex(ons => ons.RegionId);
            modelBuilder.Entity<OnsByRegion>().HasIndex(ons => ons.SubcategoryId);
            
            modelBuilder.Entity<Subcategory>().HasIndex(s => s.CategoryId);
        }
    }
}