
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MovieMongo.Dto;
using MovieMongo.Interfaces;
using MovieMongo.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MovieMongo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public UserController(IUserRepository userRepository, IConfiguration configuration, IMapper mapper)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _mapper = mapper;
        }
        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Create(UserDto userDto)
        {
            if (userDto == null)
                return BadRequest(ModelState);

            if (await _userRepository.NameExists(userDto.Username))
                return BadRequest($"User with name '{userDto.Username}' already exsits");

            var userMap = _mapper.Map<User>(userDto);
            await _userRepository.Create(userMap);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            return Ok(userMap);
        }
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody]LoginModel login)
        {
            if (login == null)
                return BadRequest(ModelState);
            var user = await _userRepository.GetuserandPassword(login.Username, login.Password);
            if(user != null)
            {
                var token = Generate(user);
                return Ok(token);
            }
            return NotFound();
        }

        private string Generate(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new []
            {
                new Claim(ClaimTypes.NameIdentifier, user.Username),
                new Claim(ClaimTypes.Role, user.Role),
            };

            var token = new JwtSecurityToken(_configuration["Jwt:Issure"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.UtcNow.AddMinutes(10),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        //private User Authenticate(LoginModel loginModel)
        //{
        //    var currentUser = _userRepository.GetUser(loginModel.Username);
        //    if (currentUser != null)
        //    {
        //        return currentUser;
        //    }
        //    return null;
        //}

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var user = _mapper.Map<List<UserDto>>(await _userRepository.GetUsers());
            return Ok(user);
        }
    }
}