using chess.api.dal;
using chess.api.Dto;
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

        //[Authorize]
        [HttpGet("me")]
        public async Task<IList<Club>> ClubsByUser()
        {
            var userId = GetUserId();
            var clubs = await _clubDal.GetClubsByUserId(userId);
            return clubs;
        }

        //[Authorize]
        [HttpGet("{clubId}")]
        public async Task<Club> Club(Guid clubId)
        {

            var userId = GetUserId();

            BusinessValidation.Club.UserCanViewClub(userId, clubId);
            var club = await _clubDal.GetClubById(clubId, userId);

            return club;
        }

        //[Authorize]
        [HttpGet]
        public async Task<IList<Club>> Clubs()
        {
            var clubs = await _clubDal.GetClubs();
            return clubs;
        }


        //[Authorize]
        [HttpGet("{clubId}/invites")]
        public async Task<IList<ClubInvite>> getInvites(Guid clubId)
        {
            var userId = GetUserId();
            BusinessValidation.Club.UserIsAdmin(userId, clubId);

            var invites = await _clubDal.GetInvitesForClub(clubId);
            return invites;
        }



        //[Authorize]
        [HttpPost]
        public async Task<Guid> Club(NewClubDto model)
        {
            model.OwnerId = GetUserId();
            var clubId = await _clubDal.CreateClub(model);
            return clubId;
        }

        //[Authorize]
        [HttpPost("{clubId}/addStudy/{studyId}")]
        public async Task AddStudy(Guid clubId, Guid studyId)
        {
            var userId = GetUserId();
            BusinessValidation.Club.UserCanViewClub(userId, clubId);
            BusinessValidation.Study.UserCanEditStudy(userId, studyId);

            await _clubDal.AddStudy(clubId, studyId);
        }

        //[Authorize]
        [HttpPost("{clubId}/removeStudy/{studyId}")]
        public async Task RemoveStudy(Guid clubId, Guid studyId)
        {
            var userId = GetUserId();
            BusinessValidation.Club.UserCanViewClub(userId, clubId);
            BusinessValidation.Study.UserCanEditStudy(userId, studyId);

            await _clubDal.RemoveStudy(clubId, studyId);
        }

        //[Authorize]
        [HttpPost("{clubId}/removeMember/{username}")]
        public async Task<HttpStatusCode> RemoveMember(Guid clubId, string username)
        {
            var userId = GetUserId();
            BusinessValidation.Club.UserCanEditClub(userId, clubId);

            await _clubDal.RemoveMember(clubId, username);
            return HttpStatusCode.NoContent;
        }

        //[Authorize]
        [HttpPost("{clubId}/acceptInvite/{username}")]
        public async Task<HttpStatusCode> AddMember(Guid clubId, string username)
        {
            var userId = GetUserId();
            BusinessValidation.Club.UserIsInvited(username, clubId);
            BusinessValidation.User.UserIsNamed(userId, username);

            await _clubDal.AddMember(clubId, username);
            return HttpStatusCode.NoContent;
        }

        //[Authorize]
        [HttpPost("{clubId}/declineInvite/{username}")]
        public async Task<HttpStatusCode> DeclineInvite(Guid clubId, string username)
        {
            var userId = GetUserId();
            BusinessValidation.User.UserIsNamed(userId, username);

            await _clubDal.DeclineInvite(clubId, username);
            return HttpStatusCode.NoContent;
        }

        //[Authorize]
        [HttpPost("inviteToJoin")]
        public async Task<HttpStatusCode> InviteToJoin(InviteToJoinDto request)
        {
            var userId = GetUserId();
            BusinessValidation.Club.UserCanEditClub(userId, request.ClubId);

            await _clubDal.InviteToJoin(request);
            return HttpStatusCode.NoContent;
        }

        //[Authorize]
        [HttpPost("requestToJoin")]
        public async Task<HttpStatusCode> RequestToJoin(RequestToJoinDto request)
        {
            var userId = GetUserId();
            BusinessValidation.User.UserIsNamed(userId, request.FromUsername);

            await _clubDal.RequestToJoin(request);
            return HttpStatusCode.NoContent;
        }

        //[Authorize]
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
}
