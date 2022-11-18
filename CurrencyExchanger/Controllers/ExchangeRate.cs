using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyExchanger.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExchangeRate : ControllerBase
    {

        [HttpGet]
        [Route("convert")]
        public ActionResult Convert(decimal amount, string from, string to, DateOnly? date = null)
        {

        }
    }
}
