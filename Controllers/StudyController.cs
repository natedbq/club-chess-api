
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
        public HttpStatusCode SaveStudy([FromBody] Study study)
        {
            studyRepo.Save(study);
            return HttpStatusCode.NoContent;
        }

        [HttpGet("SimpleStudies")]
        public IList<SimpleStudy> GetSimpleStudies()
        {
            return simpleStudyRepo.GetStub();
        }

        [HttpGet("Studies")]
        public IList<Study> GetStudies()
        {
            return studyRepo.GetStub();
        }


        [HttpGet("Studies/{id}")]
        public Study GetStudies(Guid id)
        {
            return studyRepo.GetStudyById(id);
        }
    }
}