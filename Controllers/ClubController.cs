using chess.api.dal;
using chess.api.models;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace chess.api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ClubController : ControllerBase
    {
        private readonly ClubDal _clubDal = new ClubDal();
        public ClubController() { }



        [HttpGet("{id}")]
        public async Task<Club> Club(Guid id, [FromQuery] Guid userId = default(Guid))
        {
            var club = await _clubDal.GetClubById(id,userId);
            return club;
        }

        [HttpGet]
        public async Task<IList<Club>> Clubs()
        {
            var clubs = await _clubDal.GetClubs();
            return clubs;
        }


        [HttpGet("{clubId}/invites")]
        public async Task<IList<ClubInvite>> getInvites(Guid clubId)
        {
            var invites = await _clubDal.GetInvitesForClub(clubId);
            return invites;
        }


        [HttpGet("user/{id}")]
        public async Task<IList<Club>> ClubsByUserId(Guid userId)
        {
            var clubs = await _clubDal.GetClubsByUserId(userId);
            return clubs;
        }

        [HttpPost]
        public async Task<Guid> Club(ClubPostModel model)
        {
            var clubId = await _clubDal.CreateClub(model);
            return clubId;
        }

        [HttpPost("{clubId}/addStudy/{studyId}")]
        public async Task AddStudy(Guid clubId, Guid studyId)
        {
            await _clubDal.AddStudy(clubId, studyId);
        }

        [HttpPost("{clubId}/removeStudy/{studyId}")]
        public async Task RemoveStudy(Guid clubId, Guid studyId)
        {
            await _clubDal.RemoveStudy(clubId, studyId);
        }

        [HttpPost("{clubId}/add/{username}")]
        public async Task<HttpStatusCode> AddMember(Guid clubId, string username)
        {
            await _clubDal.AddMember(clubId, username);
            return HttpStatusCode.NoContent;
        }

        [HttpPost("{clubId}/remove/{username}")]
        public async Task<HttpStatusCode> RemoveMember(Guid clubId, string username)
        {
            await _clubDal.RemoveMember(clubId, username);
            return HttpStatusCode.NoContent;
        }

        [HttpPost("{clubId}/declineInvite/{username}")]
        public async Task<HttpStatusCode> DeclineInvite(Guid clubId, string username)
        {
            await _clubDal.DeclineInvite(clubId, username);
            return HttpStatusCode.NoContent;
        }

        [HttpPost("requestToJoin")]
        public async Task<HttpStatusCode> RequestToJoin(RequestToJoinModel request)
        {
            await _clubDal.RequestToJoin(request);
            return HttpStatusCode.NoContent;
        }

        [HttpPost("inviteToJoin")]
        public async Task<HttpStatusCode> InviteToJoin(InviteToJoinModel request)
        {
            await _clubDal.InviteToJoin(request);
            return HttpStatusCode.NoContent;
        }

        [HttpGet("{clubId}/hasMember/{userId}")]
        public async Task<bool> HasMember(Guid clubId, Guid userId)
        {
            var hasMember = await _clubDal.HasMember(clubId, userId);
            return hasMember;
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
        public string username { get; set; }
    }
}
