
using Microsoft.AspNetCore.Mvc;


namespace MoviesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenresController : ControllerBase
    {
        private readonly ApplicationDbContext _context; 
        
        public GenresController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var genres = await _context.Genres.OrderBy(G=>G.Name).ToListAsync();
            return Ok(genres);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(CreateGenreDto Dto)
        {
            Genre genre = new() { Name = Dto.Name};
            await _context.AddAsync(genre);
             _context.SaveChanges();

            return Ok(genre); 

        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id , [FromBody] CreateGenreDto Dto)
        {
            var genre = await _context.Genres.SingleOrDefaultAsync(G=>G.Id==id);

            if (genre == null)
                return NotFound($"No genre found with id {id}");
            genre.Name = Dto.Name;
            _context.SaveChanges();
            return Ok(genre);
                  
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var genre = await _context.Genres.SingleOrDefaultAsync(G => G.Id == id);
            if (genre == null)
                return NotFound($"No genre found with id {id}");
            _context.Remove(genre);
            _context.SaveChanges();
            return Ok(genre);
        }
    }
}
