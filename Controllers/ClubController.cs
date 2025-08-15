using chess.api.dal;
using chess.api.models;
using Microsoft.AspNetCore.Mvc;

namespace chess.api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ClubController : ControllerBase
    {
        private readonly ClubDal _clubDal = new ClubDal();
        public ClubController() { }



        [HttpGet("{id}")]
        public async Task<Club> Club(Guid id)
        {
            var club = await _clubDal.GetClubById(id);
            return club;
        }

        [HttpGet]
        public async Task<IList<Club>> Clubs()
        {
            var clubs = await _clubDal.GetClubs();
            return clubs;
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

        [HttpPost("{clubId}/add/{userId}")]
        public async Task AddMember(Guid clubId, Guid userId)
        {
            await _clubDal.AddMember(clubId, userId);
        }

        [HttpPost("{clubId}/remove/{userId}")]
        public async Task RemoveMember(Guid clubId, Guid userId)
        {
            await _clubDal.AddMember(clubId, userId);
        }
    }

    public class ClubPostModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid OwnerId { get; set; }
    }
}
