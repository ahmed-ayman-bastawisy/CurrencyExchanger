using CurrencyExchanger.Models.APIModels;
using CurrencyExchanger.Models.Database;
using CurrencyExchanger.Repositories.ExchangeRepo;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Caching.Memory;
using Serilog;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CurrencyExchanger.Controllers
{
    [Route("api/client")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private readonly IExchangeServiceRepo _exchangeServiceRepo;
        public ClientController(IExchangeServiceRepo exchangeServiceRepo)
        {
            _exchangeServiceRepo = exchangeServiceRepo;
        }

        /// <summary>
        /// API to list Exchange trades made by a specific client
        /// </summary>
        /// <param name="clientId">client id who made exchange trades</param>
        /// <returns>a list of all trades made by this client</returns>
        [HttpGet("exchangetrades")]
        public IActionResult GetClientExchangeTrades([FromQuery][BindRequired] long clientId)
        {
            try
            {
                var clientTrades = _exchangeServiceRepo.GetClientExchangeTrades(clientId);
                if (clientTrades?.Any() == true)
                    return Ok(new ResponseGeneric<List<Exchange>>() { ResponseObj = clientTrades?.ToList() });
                else return Ok(new ResponseGeneric<List<Exchange>>() { Message = $"Client with id {clientId} does not has any exchange trades yet"});

            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
                return BadRequest(new ResponseModel() { Message = "Something went wrong while trying to get client exchange trades" });
            }
        }
    }
}
