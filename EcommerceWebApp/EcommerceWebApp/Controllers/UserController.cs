using Microsoft.Extensions.Logging;
using EcommerceWebApp.Models;
using EcommerceWebApp.Handlers;

namespace EcommerceWebApp.Controllers
{
    public class UserController
    {
        private readonly ILogger<UserController> _logger;
        private readonly DBHandler _dbHandler;
        private static int _currentUserId = 0;

        public UserController(ILogger<UserController> logger, DBHandler dbHandler)
        {
            _logger = logger;
            _dbHandler = dbHandler;
        }

        public User CreateUser()
        {
            try
            {
                _currentUserId++;
                var user = new User(_currentUserId);
                _dbHandler.SaveCart(user.GetCart());
                _logger.LogInformation($"New user created with ID: {_currentUserId}");
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating user: {ex.Message}");
                throw;
            }
        }

        public void ClearUserCart(int userId)
        {
            try
            {
                _dbHandler.DeleteCart(userId);
                _logger.LogInformation($"Shopping cart cleared for user: {userId}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error clearing cart for user {userId}: {ex.Message}");
                throw;
            }
        }
    }
}