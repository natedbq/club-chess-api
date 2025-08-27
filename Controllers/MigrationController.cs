
using chess.api.Exceptions;
using chess.api.models;
using chess.api.repository;
using chess.api.services;
using ChessApi.repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace HealthTrackerApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MigrationController : ControllerBase
    {
        private readonly ILogger<MigrationController> _logger;

        public MigrationController(ILogger<MigrationController> logger)
        {
            _logger = logger;
        }

        [Authorize]
        [HttpPost("uci")]
        public async Task<HttpStatusCode> Uci()
        {
            var uciMigration = new MigrationUCI();
            uciMigration.Execute();
            return HttpStatusCode.NoContent;
        }

        [Authorize]
        [HttpPost("error")]
        public async Task<HttpStatusCode> FakeError(bool fail)
        {
            throw new BusinessRuleException("An error was thrown! How could this happen!?");
        }
    }
}