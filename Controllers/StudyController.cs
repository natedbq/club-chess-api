
using chess.api.common;
using chess.api.models;
using chess.api.repository;
using ChessApi.repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace HealthTrackerApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StudyController : ControllerBase
    {
        private readonly ILogger<StudyController> _logger;
        private readonly StudyRepository studyRepo;
        private readonly SimpleStudyRepository simpleStudyRepo;
        private readonly PortStudyToUser portStudyTo = new PortStudyToUser();

        public StudyController(ILogger<StudyController> logger)
        {
            _logger = logger;
            studyRepo = new StudyRepository();
            simpleStudyRepo = new SimpleStudyRepository();
        }

        [Authorize]
        [HttpPost("{studyId}/import/{userId}")]
        public HttpStatusCode Import(Guid userId, Guid studyId)
        {
            portStudyTo.PortToUser(studyId, userId);
            return HttpStatusCode.NoContent;
        }

        [Authorize]
        [HttpPost]
        public async Task<HttpStatusCode> SaveStudy([FromBody] Study study)
        {
            await studyRepo.Save(study);
            return HttpStatusCode.NoContent;
        }

        [Authorize]
        [HttpPut("{id}/study/{userId}")]
        public HttpStatusCode Study(Guid id, Guid userId)
        {
            studyRepo.Study(id, userId);

            return HttpStatusCode.NoContent;
        }

        [Authorize]
        [HttpPost("delete/{id}")]
        public HttpStatusCode DeleteStudy(Guid id)
        {
            studyRepo.Delete(id);
            return HttpStatusCode.NoContent;
        }

        [Authorize]
        [HttpGet("SimpleStudies/{userId}")]
        public async Task<IList<SimpleStudy>> GetSimpleStudies(Guid userId)
        {
            try
            {
                return await simpleStudyRepo.GetStudies(userId);
            }
            catch (Exception ex)
            {
                return new List<SimpleStudy>()
                {
                    new SimpleStudy()
                    {
                        Description = ex.Message
                    }
                };
            }
            return new List<SimpleStudy>();
        }


        [Authorize]
        [HttpGet("{studyId}")]
        public async Task<Study> GetStudies(Guid studyId, [FromQuery]  Guid userId = default(Guid))
        {
            StudyAccuracyCache.Invalidate(studyId, userId);
            return await studyRepo.GetStudyById(studyId,userId);
        }
    }
}