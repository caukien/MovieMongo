using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MovieMongo.Dto.Movie;
using MovieMongo.Interfaces;
using MovieMongo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieMongo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovieController : ControllerBase
    {
        private readonly IMovieRepository _movieRepository;
        private readonly IMapper _mapper;
        private readonly CloudinaryService _cloudinaryService;

        public MovieController(IMovieRepository movie, IMapper mapper, CloudinaryService cloudinaryService)
        {
            _movieRepository = movie;
            _mapper = mapper;
            _cloudinaryService = cloudinaryService;
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromForm]CreateMovieDto createMovieDto)
        {
            try
            {
                if (createMovieDto == null)
                    return BadRequest(ModelState);
                if (await _movieRepository.NameExists(createMovieDto.Title))
                    return BadRequest($"Cast with name '{createMovieDto.Title}' already exsits");

                var item = _mapper.Map<Movie>(createMovieDto);

                if(createMovieDto.Image !=null && createMovieDto.Image.Length > 0)
                {
                    var res = await _cloudinaryService.UploadImageAsync(createMovieDto.Image);
                    item.Image = res.SecureUrl.ToString();
                    item.PublicId = res.PublicId;
                }

                await _movieRepository.Create(item);
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                return Ok(item);
            }
            catch(InvalidOperationException ex)
            {
                return BadRequest(new { ex.Message });
            }
            catch(Exception ex)
            {
                return StatusCode(500, new { ex.Message });
            }
        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var movie = await _movieRepository.GetMoviesWithDetails();
            if (movie == null)
                return NoContent();
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            return Ok(movie);
        }
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOne([FromRoute] string id)
        {
            if (id == null)
                return BadRequest();
            var movie = await _movieRepository.GetMovieWithDetails(id);
            if (movie == null)
                return NotFound();
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            return Ok(movie);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpDate([FromRoute] string id, [FromForm] CreateMovieDto createMovieDto)
        {
            try
            {
                if (id == null || createMovieDto == null)
                    return BadRequest();

                if (!await _movieRepository.Exists(id))
                    return NotFound();

                string path = null;
                string public_id = null;

                var movie = await _movieRepository.GetMovie(id);

                path = movie.Image;
                public_id = movie.PublicId;

                if (createMovieDto.Image != null && createMovieDto.Image.Length > 0)
                {
                    var res = await _cloudinaryService.UploadImageAsync(createMovieDto.Image);
                    if (!string.IsNullOrEmpty(movie.PublicId))
                    {
                        await _cloudinaryService.DeleteImageAsync(movie.PublicId);
                    }
                    path = res.SecureUrl.ToString();
                    public_id = res.PublicId;
                }

                var item = _mapper.Map(createMovieDto, movie);
                item.Image = path;
                item.PublicId = public_id;

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                await _movieRepository.Update(id, item);
                return Ok(movie);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { ex.Message });
            }
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] string id)
        {
            if (id == null)
                return BadRequest();
            if (!await _movieRepository.Exists(id))
                return NotFound();
            var item = await _movieRepository.GetMovie(id);
            if (!string.IsNullOrEmpty(item.PublicId))
            {
                await _cloudinaryService.DeleteImageAsync(item.PublicId);
            }
            await _movieRepository.Delete(id);
            return NoContent();
        }
    }
}
