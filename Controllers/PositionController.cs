using chess.api.dal;
using chess.api.models;
using chess.api.repository;
using ChessApi.repository;
using HealthTrackerApi.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace chess.api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PositionController : ControllerBase
    {
        private readonly ILogger<PositionController> _logger;
        private readonly PositionRepository positionRepo;

        public PositionController(ILogger<PositionController> logger)
        {
            _logger = logger;
            positionRepo = new PositionRepository();
        }

        //[Authorize]
        [HttpPut("{positionId}/study/me")]
        public async Task<HttpStatusCode> StudyPosition(Guid positionId)
        {
            var userId = GetUserId();
            positionRepo.StudyPosition(positionId, userId);

            return HttpStatusCode.NoContent;
        }

        //[Authorize]
        [HttpPut("{positionId}/mistake/me")]
        public HttpStatusCode MistakePosition(Guid positionId)
        {
            var userId = GetUserId();
            positionRepo.Mistake(positionId, userId);

            return HttpStatusCode.NoContent;
        }

        //[Authorize]
        [HttpPut("{positionId}/correct/me")]
        public HttpStatusCode CorrectPosition(Guid positionId)
        {
            var userId = GetUserId();
            positionRepo.Correct(positionId, userId);

            return HttpStatusCode.NoContent;
        }

        //[Authorize]
        [HttpPost("delete/{positionId}")]
        public HttpStatusCode DeletePosition(Guid positionId)
        {
            positionRepo.Delete(positionId);
            return HttpStatusCode.NoContent;
        }

        //[Authorize]
        [HttpPost]
        public HttpStatusCode SavePosition([FromBody] Position position, Guid userId)
        {
            positionRepo.Save(position, userId);
            return HttpStatusCode.NoContent;
        }


        //[Authorize]
        [HttpGet("{positionId}")]
        public Position GetPosition(Guid positionId, [FromQuery] int depth = 0)
        {
            var userId = GetUserId();
            if (depth > 10)
            {
                throw new ArgumentException("[depth] must be <= 10");
            }
            return positionRepo.GetById(positionId, userId, depth);
        }

        //[Authorize]
        [HttpGet("parentId/{positionId}")]
        public IList<Position> GetPositionByParentId(Guid positionId, [FromQuery] int depth = 0)
        {
            if (depth > 10)
            {
                throw new ArgumentException("[depth] must be <= 10");
            }
            var userId = GetUserId();
            return positionRepo.GetByParentId(positionId, userId, depth);
        }

        private Guid GetUserId()
        {
            return new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub));
        }
    }
}
