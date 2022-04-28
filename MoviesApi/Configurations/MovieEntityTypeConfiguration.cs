namespace MoviesApi.Configurations
{
    public class MovieEntityTypeConfiguration : IEntityTypeConfiguration <Movie>
    {
        public void Configure(EntityTypeBuilder<Movie> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Title).HasMaxLength(250);
            builder.Property(x => x.Storyline).HasMaxLength(2500);
            builder.HasOne(x=>x.Genre)
                .WithMany(G=>G.Movies)
                .HasForeignKey(x=>x.GenreId);
        }
    }
}
