using System.Linq;
using System.Threading.Tasks;
using BinderApp.API.Data;
using BinderApp.API.DTOs;
using BinderApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BinderApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly UserManager<User> _userManager;

        public AdminController(DataContext context, UserManager<User> userManager)
        {
            _userManager = userManager;
            _context = context;

        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("usersWithRoles")]
        public async Task<IActionResult> GetUsersWithRoles()
        {
            var userList = await _context.Users
            .OrderBy(u => u.UserName)
            .Select(u => new
            {
                Id = u.Id,
                UserName = u.UserName,
                Roles = (from userRole in u.UserRoles join role in _context.Roles on userRole.RoleId equals role.Id select role.Name).ToList()
            })
            .ToListAsync();

            return Ok(userList);
        }

        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpGet("photosForModeration")]
        public IActionResult GetPhotosForModeration()
        {
            return Ok("Admins or Moderators can see that!");
        }

        [HttpPost("editRoles/{userName}")]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<IActionResult> EditRoles(string userName, RoleEditDto roleEditDto)
        {
            var user = await _userManager.FindByNameAsync(userName);

            var userRoles = await _userManager.GetRolesAsync(user);

            //Get all selected roles from the client
            var selectedRoles = roleEditDto.RoleNames;

            //Return empty array if the user has no roles selected or deselected all roles;
            selectedRoles = selectedRoles ?? new string[] {};
            var result = await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));

            if(!result.Succeeded)
            {
                return BadRequest("Failed to add roles!");
            }

            result = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));

            if(!result.Succeeded)
            {
                return BadRequest("Failed to remove the roles!");
            }

            return Ok(await _userManager.GetRolesAsync(user));
        }
    }
}