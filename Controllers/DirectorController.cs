using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MovieMongo.Dto.Director;
using MovieMongo.Interfaces;
using MovieMongo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieMongo.Controllers
{
    [Authorize(Roles ="admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class DirectorController : ControllerBase
    {
        private readonly IDirectorRepository _directorRepository;
        private readonly IMapper _mapper;

        public DirectorController(IDirectorRepository directorRepository, IMapper mapper)
        {
            _directorRepository = directorRepository;
            _mapper = mapper;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var dr = await _directorRepository.GetMoviesWithDirectors();

            return Ok(dr);
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody]CreateDirectorDto createDirectorDto)
        {
            if (createDirectorDto == null)
            {
                return BadRequest("Director is null.");
            }
            if (await _directorRepository.DirectorNameExist(createDirectorDto.Name))
                return Conflict();
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var item = _mapper.Map<Director>(createDirectorDto);
            await _directorRepository.CreateDirector(item);
            return Ok(item);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOne([FromRoute]string id)
        {
            //if (!await _directorRepository.DirectorExist(id))
            //    return NotFound();
            var dr = await _directorRepository.GetMovieWithDirectors(id);
            if (dr == null)
                return NotFound();
            return Ok(dr);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromBody]CreateDirectorDto createDirectorDto, string id)
        {
            if (createDirectorDto == null)
            {
                return BadRequest("Director is null.");
            }

            var director = await _directorRepository.GetDirector(id);
            if (director == null)
            {
                return NotFound("The Director record couldn't be found.");
            }
            var item = _mapper.Map(createDirectorDto, director);
            await _directorRepository.UpdateDirector(item, id);
            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute]string id)
        {
            if (id == null)
                return BadRequest();
            if (!await _directorRepository.DirectorExist(id))
                return NotFound();
            await _directorRepository.DeleteDirector(id);
            return NoContent();
        }
    }
}
