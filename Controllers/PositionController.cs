using chess.api.dal;
using chess.api.models;
using chess.api.repository;
using ChessApi.repository;
using HealthTrackerApi.Controllers;
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

        [HttpPut("{id}/study")]
        public HttpStatusCode StudyPosition(Guid id)
        {
            var position = positionRepo.GetById(id);
            position.LastStudied = DateTime.Now;
            positionRepo.Save(position);

            return HttpStatusCode.NoContent;
        }

        [HttpPut("{id}/mistake")]
        public HttpStatusCode MistakePosition(Guid id)
        {
            var position = positionRepo.GetById(id);
            position.Mistakes += 1;
            positionRepo.Save(position);

            return HttpStatusCode.NoContent;
        }
        [HttpPut("{id}/correct")]
        public HttpStatusCode CorrectPosition(Guid id)
        {
            var position = positionRepo.GetById(id);
            position.Mistakes = Math.Max(position.Mistakes - 1, 0);
            positionRepo.Save(position);

            return HttpStatusCode.NoContent;
        }

        [HttpPost("delete/{id}")]
        public HttpStatusCode DeletePosition(Guid id)
        {
            positionRepo.Delete(id);
            return HttpStatusCode.NoContent;
        }

        [HttpPost]
        public HttpStatusCode SavePosition([FromBody] Position position)
        {
            positionRepo.Save(position);
            return HttpStatusCode.NoContent;
        }


        [HttpGet("{id}")]
        public Position GetPosition(Guid id, [FromQuery] int depth = 0)
        {
            if(depth > 10)
            {
                throw new ArgumentException("[depth] must be <= 10");
            }
            return positionRepo.GetById(id, depth);
        }

        [HttpGet("parentId/{id}")]
        public IList<Position> GetPositionByParentId(Guid id, [FromQuery] int depth = 0)
        {
            if (depth > 10)
            {
                throw new ArgumentException("[depth] must be <= 10");
            }
            return positionRepo.GetByParentId(id, depth);
        }
    }
}
