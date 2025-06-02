using ManoelsPackaging.Api.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace ManoelsPackaging.Api.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        #region DbSets
        /// <summary>
        /// Gets or sets the DbSet for Box entities.
        /// </summary>
        public DbSet<Box> Boxes { get; set; }

        /// <summary>
        /// Gets or sets the DbSet for Dimensions entities.
        /// </summary>
        public DbSet<Dimensions> DimensionsSet { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="AppDbContext"/> class.
        /// </summary>
        /// <param name="options">
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        #endregion

        #region Methods

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Dimensions>(dimensionsEntity =>
            {
                dimensionsEntity.HasKey(d => d.Id);
                dimensionsEntity.Property(d => d.Id)
                    .ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<Box>(boxEntity =>
            {
                boxEntity.HasKey(b => b.TypeIdentifier);
                boxEntity.HasOne(b => b.Dimensions)
                    .WithOne()
                    .HasForeignKey<Box>(b => b.DimensionsId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            Seed(modelBuilder);
        }

        /// <summary>
        /// Seeds the database with initial data for some entities.
        /// </summary>
        private void Seed(ModelBuilder modelBuilder)
        {
            var dimension1 = new Dimensions(30, 40, 80) { Id = 1 };
            var dimension2 = new Dimensions(80, 50, 40) { Id = 2 };
            var dimension3 = new Dimensions(50, 80, 60) { Id = 3 };

            modelBuilder.Entity<Dimensions>().HasData(
                dimension1,
                dimension2,
                dimension3
            );

            modelBuilder.Entity<Box>().HasData(
                new { TypeIdentifier = "Caixa 1", DimensionsId = dimension1.Id },
                new { TypeIdentifier = "Caixa 2", DimensionsId = dimension2.Id },
                new { TypeIdentifier = "Caixa 3", DimensionsId = dimension3.Id }
            );
        }
        #endregion
    }
}