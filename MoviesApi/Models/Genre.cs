namespace MoviesApi.Models
{
    public class Genre
    {
        public byte Id { get; set; }

        public string Name { get; set; }

        public List<Movie> Movies { get; set; }
    }
}
