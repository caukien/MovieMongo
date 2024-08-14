using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MovieMongo.Dto.Actor;
using MovieMongo.Interfaces;
using MovieMongo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieMongo.Controllers
{
    [Authorize(Roles = "user")]
    [Route("api/[controller]")]
    [ApiController]
    public class ActorController : ControllerBase
    {
        private readonly IActorRepository _actorRepisotory;
        private readonly IMapper _mapper;
        private readonly CloudinaryService _cloudinaryService;

        public ActorController(IActorRepository actorRepisotory, IMapper mapper, CloudinaryService cloudinaryService)
        {
            _actorRepisotory = actorRepisotory;
            _mapper = mapper;
            _cloudinaryService = cloudinaryService;
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromForm]CreateActorDto createActorDto)
        {
            try
            {
                if (createActorDto == null)
                    return BadRequest(ModelState);
                if (await _actorRepisotory.NameExists(createActorDto.Name))
                    return BadRequest($"Actor with name '{createActorDto.Name}' already exists");

                var actor = _mapper.Map<Actor>(createActorDto);

                if (createActorDto.Image != null && createActorDto.Image.Length > 0)
                {
                    var uploadResult = await _cloudinaryService.UploadImageAsync(createActorDto.Image);
                    actor.Image = uploadResult.SecureUrl.ToString();
                    actor.PublicId = uploadResult.PublicId;
                }

                await _actorRepisotory.Create(actor);
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                return Ok(actor);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request", error = ex.Message });
            }

        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var actor = await _actorRepisotory.GetActors();
            if (actor == null)
                return NoContent();
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            return Ok(actor);
        }
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOne([FromRoute]string id)
        {
            if (id == null)
                return BadRequest();
            var actor = await _actorRepisotory.GetActor(id);
            if (actor == null)
                return NotFound();
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            return Ok(actor);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpDate([FromRoute] string id, [FromForm] CreateActorDto createActorDto)
        {
            try
            {
                if (id == null || createActorDto == null)
                    return BadRequest();

                if (!await _actorRepisotory.Exists(id))
                    return NotFound();

                var actorToUpdate = await _actorRepisotory.GetActor(id);

                string filepath = actorToUpdate.Image;
                string image_Id = actorToUpdate.PublicId;

                if (createActorDto.Image != null && createActorDto.Image.Length > 0)
                {
                    var uploadResult = await _cloudinaryService.UploadImageAsync(createActorDto.Image);

                    if (!string.IsNullOrEmpty(actorToUpdate.PublicId))
                    {
                        await _cloudinaryService.DeleteImageAsync(actorToUpdate.PublicId);
                    }
                    filepath = uploadResult?.SecureUrl?.ToString();
                    image_Id = uploadResult?.PublicId?.ToString();
                }

                var item = _mapper.Map(createActorDto, actorToUpdate);
                item.Image = filepath;
                item.PublicId = image_Id;

                await _actorRepisotory.Update(id, item);

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                return Ok(actorToUpdate);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request", error = ex.Message });
            }
            
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] string id)
        {
            if (id == null)
                return BadRequest();
            var item = await _actorRepisotory.GetActor(id);
            if (!string.IsNullOrEmpty(item.PublicId))
            {
                await _cloudinaryService.DeleteImageAsync(item.PublicId);
            }
            await _actorRepisotory.Delete(id);
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            return NoContent();
        }
    }
}
