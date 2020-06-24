using System.Threading.Tasks;
using BinderApp.API.Data;
using BinderApp.API.DTOs;
using BinderApp.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace BinderApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;

        public AuthController(IAuthRepository repo)
        {
            _repo = repo;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDto userForRegisterDto)
        {
            userForRegisterDto.Username = userForRegisterDto.Username.ToLower();

            if(await _repo.UserExists( userForRegisterDto.Username))
            {
                return BadRequest("Username already exists!");
            }

            User userToCreate = new User
            {
                Username =  userForRegisterDto.Username,
            };

            User createdUser = await _repo.Register(userToCreate, userForRegisterDto.Passowrd);

            return StatusCode(201);
        }
    }
}