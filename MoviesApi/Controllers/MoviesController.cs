using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MoviesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        private readonly List<string> _allowedExtentions = new List<string>() { ".jpg", ".png" };

        private readonly long _MaxLength = 1048576;
        public MoviesController(ApplicationDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            //var movies = await _context.Movies.ToListAsync();

            //var movies = await _context.Movies.Include(M => M.Genre).ToListAsync();

            var movies = await _context.Movies.Include(M => M.Genre).OrderByDescending(m=>m.Rate).Select(m => new MoviesDetailsDto {
                GenreId = m.GenreId,
                GenreName =m.Genre.Name,
                Id =m.Id,
                Year =m.Year,
                Poster = m.Poster,
                Rate =m.Rate,
                Storyline =m.Storyline,
                Title =m.Title
            }).ToListAsync();

            return Ok(movies);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOneAsync(int id)
        {
            var movie = await _context.Movies.Include(M=>M.Genre).SingleOrDefaultAsync(M=> M.Id == id);

            if (movie == null)
                return NotFound();
            var dto = new MoviesDetailsDto()
            {
                GenreId = movie.GenreId,
                GenreName = movie.Genre.Name,
                Id = movie.Id,
                Year = movie.Year,
                Poster = movie.Poster,
                Rate = movie.Rate,
                Storyline = movie.Storyline,
                Title = movie.Title
            };
            return Ok(dto);
        }

        [HttpGet("GetByGenreId")]

        public async Task<IActionResult> GetByGenreIdAsync(byte genreid)
        {
            var movies = await _context.Movies.Include(M => M.Genre)
                .Where(M=> M.GenreId == genreid)
                .OrderByDescending(m => m.Rate).Select(m => new MoviesDetailsDto
            {
                GenreId = m.GenreId,
                GenreName = m.Genre.Name,
                Id = m.Id,
                Year = m.Year,
                Poster = m.Poster,
                Rate = m.Rate,
                Storyline = m.Storyline,
                Title = m.Title
            }).ToListAsync();

            return Ok(movies);
        }

        [HttpPost]

       public async Task<IActionResult> CreateAsync([FromForm]MovieDto Dto)
        {
            if (!_allowedExtentions.Contains(Path.GetExtension(Dto.Poster.FileName).ToLower()))
                return BadRequest("only .png and .jpg Images !");

            if (_MaxLength < Dto.Poster.Length)
                return BadRequest("The Max Size 1MG !");
         
            using var datastream = new MemoryStream();
            await Dto.Poster.CopyToAsync(datastream);

            var movie = new Movie() 
            {
                Title = Dto.Title,
                GenreId = Dto.GenreId,
                Rate = Dto.Rate,
                Year = Dto.Year,
                Storyline = Dto.Storyline,
                Poster = datastream.ToArray()
            };

            await _context.AddAsync(movie);
            _context.SaveChanges();
            return Ok(movie);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
                return NotFound($"No Movie With id {id}");
            _context.Remove(movie);
            _context.SaveChanges();
            return Ok(movie);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromForm] MovieUpdateDto Dto)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
                return NotFound($"No Movie With id {id}");
            var genre = await _context.Genres.AnyAsync(m => m.Id == Dto.GenreId);
            if(!genre)
                return NotFound($"No Genre with Id {Dto.GenreId}");

            if(Dto.Poster != null)
            {
                if (!_allowedExtentions.Contains(Path.GetExtension(Dto.Poster.FileName).ToLower()))
                {
                    return BadRequest(".jpg .npg images only !");
                }
                if (_MaxLength < Dto.Poster.Length)
                {
                    return BadRequest("max length 1 MG");
                }
                using var datastream = new MemoryStream();
                Dto.Poster.CopyTo(datastream);
                movie.Poster = datastream.ToArray();
            }



            movie.Title = Dto.Title;
            movie.Storyline = Dto.Storyline;
            movie.Rate = Dto.Rate;
            movie.Year=Dto.Year;
            movie.GenreId = Dto.GenreId;
            _context.SaveChanges();

            return Ok(movie);

        }
    }
}
