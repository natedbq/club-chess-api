
using chess.api.models;
using chess.api.repository;
using ChessApi.repository;
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

        public StudyController(ILogger<StudyController> logger)
        {
            _logger = logger;
            studyRepo = new StudyRepository();
            simpleStudyRepo = new SimpleStudyRepository();
        }

        [HttpPost]
        public async Task<HttpStatusCode> SaveStudy([FromBody] Study study)
        {
            await studyRepo.Save(study);
            return HttpStatusCode.NoContent;
        }

        [HttpPut("{id}/study")]
        public HttpStatusCode Study(Guid id)
        {
            var study = studyRepo.GetStudyById(id);
            study.Position.LastStudied = DateTime.Now;
            studyRepo.Save(study);
            
            return HttpStatusCode.NoContent;
        }

        [HttpPost("delete/{id}")]
        public HttpStatusCode DeleteStudy(Guid id)
        {
            studyRepo.Delete(id);
            return HttpStatusCode.NoContent;
        }

        [HttpGet("SimpleStudies/{userId}")]
        public IList<SimpleStudy> GetSimpleStudies(Guid userId)
        {
            try
            {
                return simpleStudyRepo.GetStudies(userId);
            }
            catch(Exception ex)
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


        [HttpGet("Studies/{id}")]
        public Study GetStudies(Guid id)
        {
            return studyRepo.GetStudyById(id);
        }
    }
}