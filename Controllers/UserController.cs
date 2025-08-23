using chess.api.common;
using chess.api.dal;
using chess.api.models;
using chess.api.repository;
using ChessApi.repository;
using Microsoft.AspNetCore.Mvc;
using System.Net;

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

        [HttpGet("{id}")]
        public async Task<User> User(Guid id)
        {
            var user = await userDal.GetUserById(id);
            return user;
        }

        [HttpGet("{id}/invites")]
        public async Task<IList<ClubInvite>> GetInvites(Guid id)
        {
            var invites = await userDal.GetInvitesForUser(id);
            return invites;
        }

        [HttpPost]
        public async Task<Guid> User(NewUserModel user)
        {
            var userId = await userDal.CreateUser(user, user.Password);
            return userId;
        }

        [HttpPost("auth")]
        public async Task<SimpleUser> Authenticate(UsernameAndPassword details)
        {
            var user = await userDal.Authenticate(details.Username, details.Password);
            return user;
        }
    }

    public class NewUserModel
    {
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
    }

    public class UsernameAndPassword
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
