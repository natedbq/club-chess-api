using chess.api.dal;
using chess.api.models;
using chess.api.Validations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;

namespace chess.api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ClubController : ControllerBase
    {
        private readonly ClubDal _clubDal = new ClubDal();
        public ClubController() { }



        [Authorize]
        [HttpGet("{clubId}")]
        public async Task<Club> Club(Guid clubId)
        {

            var userId = GetUserId();

            BusinessValidation.Club.UserCanViewClub(userId, clubId);
            var club = await _clubDal.GetClubById(clubId, userId);

            return club;
        }

        [Authorize]
        [HttpGet]
        public async Task<IList<Club>> Clubs()
        {
            var clubs = await _clubDal.GetClubs();
            return clubs;
        }


        [Authorize]
        [HttpGet("{clubId}/invites")]
        public async Task<IList<ClubInvite>> getInvites(Guid clubId)
        {
            var userId = GetUserId();
            BusinessValidation.Club.UserIsAdmin(userId, clubId);

            var invites = await _clubDal.GetInvitesForClub(clubId);
            return invites;
        }


        [Authorize]
        [HttpGet("me")]
        public async Task<IList<Club>> ClubsByUser()
        {
            var userId = GetUserId();
            var clubs = await _clubDal.GetClubsByUserId(userId);
            return clubs;
        }

        [Authorize]
        [HttpPost]
        public async Task<Guid> Club(ClubPostModel model)
        {
            model.OwnerId = GetUserId();
            var clubId = await _clubDal.CreateClub(model);
            return clubId;
        }

        [Authorize]
        [HttpPost("{clubId}/addStudy/{studyId}")]
        public async Task AddStudy(Guid clubId, Guid studyId)
        {
            var userId = GetUserId();
            BusinessValidation.Club.UserCanViewClub(userId, clubId);
            BusinessValidation.Study.UserCanEditStudy(userId, studyId);

            await _clubDal.AddStudy(clubId, studyId);
        }

        [Authorize]
        [HttpPost("{clubId}/removeStudy/{studyId}")]
        public async Task RemoveStudy(Guid clubId, Guid studyId)
        {
            var userId = GetUserId();
            BusinessValidation.Club.UserCanViewClub(userId, clubId);
            BusinessValidation.Study.UserCanEditStudy(userId, studyId);

            await _clubDal.RemoveStudy(clubId, studyId);
        }

        [Authorize]
        [HttpPost("{clubId}/add/{username}")]
        public async Task<HttpStatusCode> AddMember(Guid clubId, string username)
        {
            var userId = GetUserId();
            BusinessValidation.Club.UserIsInvited(username, clubId);
            BusinessValidation.User.UserIsNamed(userId, username);

            await _clubDal.AddMember(clubId, username);
            return HttpStatusCode.NoContent;
        }

        [Authorize]
        [HttpPost("{clubId}/remove/{username}")]
        public async Task<HttpStatusCode> RemoveMember(Guid clubId, string username)
        {
            var userId = GetUserId();
            BusinessValidation.Club.UserCanEditClub(userId, clubId);

            await _clubDal.RemoveMember(clubId, username);
            return HttpStatusCode.NoContent;
        }

        [Authorize]
        [HttpPost("{clubId}/declineInvite/{username}")]
        public async Task<HttpStatusCode> DeclineInvite(Guid clubId, string username)
        {
            var userId = GetUserId();
            BusinessValidation.User.UserIsNamed(userId, username);

            await _clubDal.DeclineInvite(clubId, username);
            return HttpStatusCode.NoContent;
        }

        [Authorize]
        [HttpPost("requestToJoin")]
        public async Task<HttpStatusCode> RequestToJoin(RequestToJoinModel request)
        {
            var userId = GetUserId();
            BusinessValidation.User.UserIsNamed(userId, request.FromUsername);

            await _clubDal.RequestToJoin(request);
            return HttpStatusCode.NoContent;
        }

        [Authorize]
        [HttpPost("inviteToJoin")]
        public async Task<HttpStatusCode> InviteToJoin(InviteToJoinModel request)
        {
            var userId = GetUserId();
            BusinessValidation.Club.UserCanEditClub(userId, request.ClubId);

            await _clubDal.InviteToJoin(request);
            return HttpStatusCode.NoContent;
        }

        [Authorize]
        [HttpGet("{clubId}/hasMember/me")]
        public async Task<bool> HasMember(Guid clubId)
        {
            var userId = GetUserId();

            var hasMember = await _clubDal.HasMember(clubId, userId);
            return hasMember;
        }

        private Guid GetUserId()
        {
            return new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub));
        }
    }


    public class InviteToJoinModel
    {
        public string ToUsername { get; set; }
        public string FromUsername { get; set; }
        public Guid ClubId { get; set; }
        public string Message { get; set; }
    }
    public class RequestToJoinModel
    {
        public string FromUsername { get; set; }
        public Guid ClubId { get; set; }
        public string Message { get; set; }
    }

    public class ClubPostModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid OwnerId { get; set; }
    }
}
