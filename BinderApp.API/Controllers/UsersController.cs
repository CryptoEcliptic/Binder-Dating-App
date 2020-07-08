using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using BinderApp.API.Data;
using BinderApp.API.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BinderApp.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IDatingRepository _repo;
        private readonly IMapper _mapper;
        public UsersController(IDatingRepository repo, IMapper mapper)
        {
            _mapper = mapper;
            _repo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _repo.GetUsers();

            var returnedUsers = _mapper.Map<IEnumerable<UserForListDto>>(users);

            return Ok(returnedUsers);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _repo.GetUser(id);
            var returnedUser = _mapper.Map<UserForDetailedDto>(user);

            return Ok(returnedUser);
        }
    }
}