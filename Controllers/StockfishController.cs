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
    public class StockfishController : ControllerBase
    {
        private readonly ILogger<StockfishController> _logger;
        private StockfishWrapper _stockfishWrapper = new StockfishWrapper();

        public StockfishController(ILogger<StockfishController> logger)
        {
            _logger = logger;
        }


        [HttpGet("eval")]
        public async Task<string> Eval([FromQuery] string fen, [FromQuery] int depth = 20)
        {
            _stockfishWrapper.Start();


            string result = await _stockfishWrapper.EvaluatePosition(fen, depth);

            _stockfishWrapper.Stop();

            return result;
        }

    }
}
