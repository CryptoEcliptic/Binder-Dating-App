using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BinderApp.API.Data;
using BinderApp.API.DTOs;
using BinderApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace BinderApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AuthController(IConfiguration configuration, IMapper mapper,
                 UserManager<User> userManager,
                 SignInManager<User> signInManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _configuration = configuration;
            _mapper = mapper;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDto userForRegisterDto)
        {

            User userToCreate = _mapper.Map<User>(userForRegisterDto);

            var result = await _userManager.CreateAsync(userToCreate, userForRegisterDto.Password)
;

            var userToReturn = _mapper.Map<UserForDetailedDto>(userToCreate);

           if(result.Succeeded)
           {
                return CreatedAtRoute("GetUser", new { Controller = "Users", id = userToCreate.Id }, userToReturn);
           }

            return BadRequest(result.Errors);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDto userForLoginDto)
        {
            var user = await _userManager.FindByNameAsync(userForLoginDto.Username);

            var result = await _signInManager.CheckPasswordSignInAsync(user, userForLoginDto.Password, false);

            if(result.Succeeded)
            {
                var appUser = _mapper.Map<UserForListDto>(user);

                return Ok(new
                {
                    token = GenerateJWTToken(user),
                    user = appUser
                });
            }

            return Unauthorized();
        }

        private string GenerateJWTToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName)
            };

            //Creating a security key with which the server signs the Jwt token
            var key = new SymmetricSecurityKey(Encoding.UTF8
                                                .GetBytes(_configuration
                                                .GetSection("AppSettings:Token").Value));

            //Create credentials and encrypt the key with sha512
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            //Creating token descriptor adding claims and signincredentials
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(2),
                SigningCredentials = creds
            };

            //Create instance of token handler
            var tokenHandler = new JwtSecurityTokenHandler();

            //Use the tokenHandler to create jwt token
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);

        }
    }
}