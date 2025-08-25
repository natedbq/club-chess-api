using chess.api.dal;
using chess.api.models;
using chess.api.repository;
using ChessApi.repository;
using HealthTrackerApi.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

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

        [Authorize]
        [HttpPut("{positionId}/study/{userId}")]
        public async Task<HttpStatusCode> StudyPosition(Guid positionId, Guid userId)
        {
            positionRepo.StudyPosition(positionId, userId);

            return HttpStatusCode.NoContent;
        }

        [Authorize]
        [HttpPut("{positionId}/mistake/{userId}")]
        public HttpStatusCode MistakePosition(Guid positionId, Guid userId)
        {
            positionRepo.Mistake(positionId, userId);

            return HttpStatusCode.NoContent;
        }

        [Authorize]
        [HttpPut("{positionId}/correct/{userId}")]
        public HttpStatusCode CorrectPosition(Guid positionId, Guid userId)
        {
            positionRepo.Correct(positionId, userId);

            return HttpStatusCode.NoContent;
        }

        [Authorize]
        [HttpPost("delete/{id}")]
        public HttpStatusCode DeletePosition(Guid id)
        {
            positionRepo.Delete(id);
            return HttpStatusCode.NoContent;
        }

        [Authorize]
        [HttpPost]
        public HttpStatusCode SavePosition([FromBody] Position position, Guid userId)
        {
            positionRepo.Save(position, userId);
            return HttpStatusCode.NoContent;
        }


        [Authorize]
        [HttpGet("{id}")]
        public Position GetPosition(Guid id, [FromQuery] Guid userId, [FromQuery] int depth = 0)
        {
            if(depth > 10)
            {
                throw new ArgumentException("[depth] must be <= 10");
            }
            return positionRepo.GetById(id, userId, depth);
        }

        [Authorize]
        [HttpGet("parentId/{id}")]
        public IList<Position> GetPositionByParentId(Guid id, [FromQuery] Guid userId, [FromQuery] int depth = 0)
        {
            if (depth > 10)
            {
                throw new ArgumentException("[depth] must be <= 10");
            }
            return positionRepo.GetByParentId(id, userId, depth);
        }
    }
}
