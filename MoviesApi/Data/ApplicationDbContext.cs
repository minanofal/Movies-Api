
using MoviesApi.Configurations;

namespace MoviesApi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            new GenreEntityTypeConfiguration().Configure(modelBuilder.Entity<Genre>());
            new MovieEntityTypeConfiguration().Configure(modelBuilder.Entity<Movie>());
        }

        public DbSet<Genre> Genres {get; set; }
        public DbSet<Movie> Movies {get; set; }
    }
}
