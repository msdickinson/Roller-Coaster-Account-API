//Coasters
//Account
//Site
//System








//using System.Linq;
//using System.Threading.Tasks;
//using DickinsonBros.AccountAPI.Logic;
//using DickinsonBros.AccountAPI.View.Models;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using DickinsonBros.AccountAPI.Infrastructure.Logging;

//namespace DickinsonBros.AccountAPI.View.Controllers
//{
//    [ApiController]
//    [Route("[controller]")]
//    public class AdministrationController : ControllerBase
//    {
//        internal readonly ILoggingService<AdministrationController> _logger;
//        internal readonly IAccountManager _accountManager;
//        public AdministrationController
//        (
//            ILoggingService<AdministrationController> logger,
//            IAccountManager accountManager
//        )
//        {
//            _logger = logger;
//            _accountManager = accountManager;
//        }


//        [HttpGet("Achievements")]
//        public async Task<ActionResult> ActivityAsync()
//        {
//            var correlationId = HttpContext.Request.Headers["X-Correlation-ID"].First();

//            //Achievements Total
//            //Achievements Last 30 Days
//            //Achievements New Users 30 Days

//            return Ok();
//        }

//        [HttpGet("Accounts")]
//        public async Task<ActionResult> AccountsAsync()
//        {
//            var correlationId = HttpContext.Request.Headers["X-Correlation-ID"].First();

//            //Total Accounts
//            //New Accounts (30 days)
//            //Account Acitcity (Duration)
//            //Account Logins
//            //Account PasswordHash Reset
//            //Account PasswordHash Reset Complete Process

//            //if (RequestPasswordResetEmailResult == Logic.Models.RequestPasswordResetEmailtResult.DatabaseError)
//            //{
//            //    return StatusCode(500);
//            //}

//            return Ok();
//        }

//        [HttpGet("Account")]
//        public async Task<ActionResult> AccountAsync([FromBody]AccountRequest accountRequest)
//        {
//            var correlationId = HttpContext.Request.Headers["X-Correlation-ID"].First();

//            //if (RequestPasswordResetEmailResult == Logic.Models.RequestPasswordResetEmailtResult.DatabaseError)
//            //{
//            //    return StatusCode(500);
//            //}

//            return Ok();
//        }

//        [HttpPost("Coasters")]
//        public async Task<ActionResult> CoastersAsync()
//        {
//            var correlationId = HttpContext.Request.Headers["X-Correlation-ID"].First();

//            //Last 30 Days
//            //Coaster Tracks Distribution
//            //Total Coasters
//            //Cosaters Per User
//            //Tracks Per User User
 
//            return Ok();
//        }

//        [HttpPost("Que")]
//        public async Task<ActionResult> QueAsync()
//        {
//            var correlationId = HttpContext.Request.Headers["X-Correlation-ID"].First();

//            //Que Status

//            return Ok();
//        }

//        [HttpPost("System")]
//        public async Task<ActionResult> SystemAsync()
//        {
//            var correlationId = HttpContext.Request.Headers["X-Correlation-ID"].First();

//            //CPU, Ram, Network, Seilog Pull Log Reports
            

//            return Ok();
//        }

//        internal string TryGetHeaderValue(HttpRequest request, string key)
//        {
//            return request?.Headers?[key].FirstOrDefault()?.ToString();
//        }
//    }
//}
