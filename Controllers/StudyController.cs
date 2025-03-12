
using chess.api.models;
using chess.api.repository;
using ChessApi.repository;
using Microsoft.AspNetCore.Mvc;

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