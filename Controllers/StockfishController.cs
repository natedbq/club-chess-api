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
    public class StockfishController : ControllerBase
    {
        private readonly ILogger<StockfishController> _logger;
        private StockfishWrapper _stockfishWrapper = new StockfishWrapper();

        public StockfishController(ILogger<StockfishController> logger)
        {
            _logger = logger;
        }


        //[Authorize]
        [HttpGet("eval")]
        public async Task<SinglePointEval> Eval([FromQuery] string fen, [FromQuery] int depth = 20)
        {
            _stockfishWrapper.Start();


            var result = await _stockfishWrapper.EvaluatePosition(fen, depth);

            _stockfishWrapper.Stop();

            return result;
        }

        //[Authorize]
        [HttpGet("engine")]
        public async Task<Evaluation> Engine([FromQuery] string fen, [FromQuery] int depth = 20)
        {
            _stockfishWrapper.Start();


            var result = await _stockfishWrapper.EngineMoves(fen, depth);

            _stockfishWrapper.Stop();

            return result;
        }
    }
}
