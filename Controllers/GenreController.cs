using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MovieMongo.Dto.Genre;
using MovieMongo.Interfaces;
using MovieMongo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieMongo.Controllers
{
    [Authorize(Roles = "admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class GenreController : ControllerBase
    {
        private readonly IGenreRepository _genre;
        private readonly IMapper _mapper;

        public GenreController(IGenreRepository genre, IMapper mapper)
        {
            _genre = genre;
            _mapper = mapper;
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody]CreateGenreDto createGenreDto)
        {
            if (createGenreDto == null)
                return BadRequest(ModelState);
            if(await _genre.NameExists(createGenreDto.Name))
                return BadRequest($"Genre with name '{createGenreDto.Name}' already exists.");

            var genre = _mapper.Map<Genre>(createGenreDto);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            await _genre.CreateGenre(genre);
            return Ok(genre);
        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var genres = await _genre.GetGenresWithMovies();
            return Ok(genres);
        }
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute]string id)
        {
            if (id == null)
                return BadRequest();
            if (!await _genre.Exists(id))
                return NotFound();
            var gen = await _genre.GetGenreWithMovies(id);
            return Ok(gen);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute]string id, [FromBody]CreateGenreDto createGenreDto)
        {
            if (id == null || createGenreDto == null)
                return BadRequest();
            var genre = _mapper.Map(createGenreDto, await _genre.GetGenre(id));
            await _genre.UpdateGenre(id, genre);
            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] string id)
        {
            if (id == null)
                return BadRequest(); ;
            if (!await _genre.Exists(id))
                return NotFound();
            await _genre.DeleteGenre(id);
            return NoContent();
        }
    }
}
