using Microsoft.Extensions.Logging;
using EcommerceWebApp.Models;
using EcommerceWebApp.Handlers;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceWebApp.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly DBHandler _dbHandler;
        private static int _currentUserId = 0;

        public UserController(ILogger<UserController> logger, DBHandler dbHandler)
        {
            _logger = logger;
            _dbHandler = dbHandler;
        }

        // POST: Create a new user
        [HttpPost("create")]
        public IActionResult CreateUser()
        {
            try
            {
                _currentUserId++;
                var user = new User(_currentUserId);
                _dbHandler.SaveCart(user.GetCart());
                _logger.LogInformation($"New user created with ID: {_currentUserId}");
                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating user: {ex.Message}");
                return BadRequest($"Error creating user: {ex.Message}");
            }
        }

        // GET: Retrieve user by ID
        [HttpGet("{userId}")]
        public IActionResult GetUser(int userId)
        {
            try
            {
                var user = _dbHandler.GetUserById(userId);
                if (user == null)
                {
                    return NotFound($"User with ID {userId} not found.");
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving user {userId}: {ex.Message}");
                return BadRequest($"Failed to retrieve user: {ex.Message}");
            }
        }

        // PUT: Update user information
        [HttpPut("update/{userId}")]
        public IActionResult UpdateUser(int userId, [FromBody] User updatedUser)
        {
            try
            {
                var existingUser = _dbHandler.GetUserById(userId);
                if (existingUser == null)
                {
                    return NotFound();
                }

                _dbHandler.UpdateUser(userId, updatedUser);
                return Ok(updatedUser);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating user {userId}: {ex.Message}");
                return BadRequest($"Failed to update user: {ex.Message}");
            }
        }

        
        [HttpPatch("update/{userId}")]
        public IActionResult PatchUser(int userId, [FromBody] User updatedUser)
        {
            try
            {
                var existingUser = _dbHandler.GetUserById(userId);
                if (existingUser == null)
                {
                    return NotFound();
                }

                _dbHandler.PatchUser(userId, updatedUser);
                return Ok(updatedUser);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error patching user {userId}: {ex.Message}");
                return BadRequest($"Failed to patch user: {ex.Message}");
            }
        }

        // DELETE: Delete a user
        [HttpDelete("delete/{userId}")]
        public IActionResult DeleteUser(int userId)
        {
            try
            {
                var user = _dbHandler.GetUserById(userId);
                if (user != null)
                {
                    _dbHandler.DeleteCart(userId);
                    _logger.LogInformation($"Deleted user with ID: {userId}");
                    return Ok("User deleted.");
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting user {userId}: {ex.Message}");
                return BadRequest($"Failed to delete user: {ex.Message}");
            }
        }

        [HttpOptions]
        public IActionResult OptionsUser()
        {
            Response.Headers.Add("Allow", "GET, POST, PUT, PATCH, DELETE, OPTIONS");
            return Ok();
        }
    }
}
