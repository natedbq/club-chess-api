using chess.api.common;
using chess.api.dal;
using chess.api.models;
using chess.api.repository;
using ChessApi.repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace chess.api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly UserDal userDal = new UserDal();

        public UserController(ILogger<UserController> logger)
        {
            _logger = logger;
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<User> GetMyProfile()
        {
            var id = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            var userId = new Guid(id);
            var user = await userDal.GetUserById(userId);
            return user;
        }

        [Authorize]
        [HttpGet("{id}/invites")]
        public async Task<IList<ClubInvite>> GetInvites(Guid id)
        {
            var invites = await userDal.GetInvitesForUser(id);
            return invites;
        }

        [Authorize]
        [HttpPost]
        public async Task<Guid> CreateUser(NewUserModel user)
        {
            var userId = await userDal.CreateUser(user, user.Password);
            return userId;
        }
    }

    public class NewUserModel
    {
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
    }
}
