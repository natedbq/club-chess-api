
using chess.api.common;
using chess.api.Exceptions;
using chess.api.models;
using chess.api.repository;
using chess.api.Validations;
using ChessApi.repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;

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

        //[Authorize]
        [HttpPost("{studyId}/import/me")]
        public HttpStatusCode Import(Guid studyId)
        {
            var userId = GetUserId();
            BusinessValidation.Study.UserCanViewStudy(studyId, userId);

            portStudyTo.PortToUser(studyId, userId);
            return HttpStatusCode.NoContent;
        }

        //[Authorize]
        [HttpPost]
        public async Task<HttpStatusCode> SaveStudy([FromBody] Study study)
        {
            var userId = GetUserId();
            study.Owner = new SimpleUser()
            {
                Id = userId
            };

            await studyRepo.Save(study);
            return HttpStatusCode.NoContent;
        }

        //[Authorize]
        [HttpPut("{id}/study/me")]
        public HttpStatusCode Study(Guid id)
        {
            var userId = GetUserId();
            BusinessValidation.Study.UserCanViewStudy(userId, id);

            studyRepo.Study(id, userId);

            return HttpStatusCode.NoContent;
        }

        //[Authorize]
        [HttpPost("delete/{id}")]
        public HttpStatusCode DeleteStudy(Guid id)
        {
            var userId = GetUserId();
            BusinessValidation.Study.UserCanEditStudy(userId, id);

            studyRepo.Delete(id);
            return HttpStatusCode.NoContent;
        }

        //[Authorize]
        [HttpGet("SimpleStudies/me")]
        public async Task<IList<SimpleStudy>> GetSimpleStudies()
        {
            try
            {
                var userId = GetUserId();
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


        //[Authorize]
        [HttpGet("{studyId}/me")]
        public async Task<Study> GetStudy(Guid studyId)
        {
            var userId = GetUserId();
            BusinessValidation.Study.UserCanViewStudy(userId, studyId);

            StudyAccuracyCache.Invalidate(studyId, userId);
            return await studyRepo.GetStudyById(studyId,userId);
        }

        private Guid GetUserId()
        {
            return new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub));
        }
    }
}