//CreateAsync       (Read/Write, Read Only URL, Saves Data, Puts into Account If Account Exist)
//UpdateAsync       (Update)
//LoadAsync         (Load Coaster readonly/edit)
//Publish           (Locks Coaster, Puts into public list)
//GetYourCoasters   (Maybe a query?, maybe so many at a time)
//QueryCoasters     (Many Query Options, Returns in chunks)
//CoasterEvent      (Loaded Coaster, Rode Coaster To compleateion)


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
//    public class CoastersController : ControllerBase
//    {
//        internal readonly ILoggingService<CoastersController> _logger;
//        internal readonly IAccountManager _accountManager;
//        public CoastersController
//        (
//            ILoggingService<CoastersController> logger,
//            IAccountManager accountManager
//        )
//        {
//            _logger = logger;
//            _accountManager = accountManager;
//        }

//        [HttpPost("Query")]
//        public async Task<ActionResult> CreateAsync([FromBody]CreateAccountRequest createAccountRequest)
//        {
//            var correlationId = HttpContext.Request.Headers["X-Correlation-ID"].First();

//            var createAccountDescriptor =
//                await _accountManager.CreateAsync
//                (
//                    createAccountRequest.Username,
//                    createAccountRequest.Password,
//                    createAccountRequest.Email,
//                    correlationId
//                );

//            if (createAccountDescriptor.Result == Logic.Models.CreateAccountResult.DatabaseError)
//            {
//                return StatusCode(500);
//            }

//            if (createAccountDescriptor.Result == Logic.Models.CreateAccountResult.DuplicateUser)
//            {
//                return StatusCode(409);
//            }

//            return StatusCode(200, createAccountDescriptor.AccountId);
//        }

//        [HttpPost("Publish")]
//        public async Task<ActionResult> PublishAsync([FromBody]CreateAccountRequest createAccountRequest)
//        {
//            var correlationId = HttpContext.Request.Headers["X-Correlation-ID"].First();

//            var createAccountDescriptor =
//                await _accountManager.CreateAsync
//                (
//                    createAccountRequest.Username,
//                    createAccountRequest.Password,
//                    createAccountRequest.Email,
//                    correlationId
//                );

//            if (createAccountDescriptor.Result == Logic.Models.CreateAccountResult.DatabaseError)
//            {
//                return StatusCode(500);
//            }

//            if (createAccountDescriptor.Result == Logic.Models.CreateAccountResult.DuplicateUser)
//            {
//                return StatusCode(409);
//            }

//            return StatusCode(200, createAccountDescriptor.AccountId);
//        }

//        [HttpGet("ReadOnlyURL")]
//        public async Task<ActionResult> ReadOnlyURLAsync([FromBody]CreateAccountRequest createAccountRequest)
//        {
//            var correlationId = HttpContext.Request.Headers["X-Correlation-ID"].First();

//            var createAccountDescriptor =
//                await _accountManager.CreateAsync
//                (
//                    createAccountRequest.Username,
//                    createAccountRequest.Password,
//                    createAccountRequest.Email,
//                    correlationId
//                );

//            if (createAccountDescriptor.Result == Logic.Models.CreateAccountResult.DatabaseError)
//            {
//                return StatusCode(500);
//            }

//            if (createAccountDescriptor.Result == Logic.Models.CreateAccountResult.DuplicateUser)
//            {
//                return StatusCode(409);
//            }

//            return StatusCode(200, createAccountDescriptor.AccountId);
//        }

//        [HttpPut("")]
//        public async Task<ActionResult> PutAsync([FromBody]LoginRequest createAccountRequest)
//        {
//            var correlationId = HttpContext.Request.Headers["X-Correlation-ID"].First();

//            var loginRequest =
//                await _accountManager.LoginAsync(createAccountRequest.Username, createAccountRequest.Password, correlationId);

//            if (loginRequest.Result == Logic.Models.LoginResult.AccountNotFound)
//            {
//                return StatusCode(404);
//            }

//            if (loginRequest.Result == Logic.Models.LoginResult.InvaildPassword)
//            {
//                return StatusCode(401);
//            }

//            if (loginRequest.Result == Logic.Models.LoginResult.DatabaseError)
//            {
//                return StatusCode(500);
//            }

//            return Ok(loginRequest.AccountId);
//        }

//        [HttpGet("")]
//        public async Task<ActionResult> SelectAsync([FromBody]RequestPasswordResetEmailRequest createAccountRequest)
//        {
//            var correlationId = HttpContext.Request.Headers["X-Correlation-ID"].First();

//            //var RequestPasswordResetEmailResult =
//            //await _accountManager.RequestPasswordResetEmailAsync(createAccountRequest.Email, correlationId);

//            //if (RequestPasswordResetEmailResult == Logic.Models.RequestPasswordResetEmailtResult.EmailNotFound)
//            //{
//            //    return NotFound();
//            //}

//            //if (RequestPasswordResetEmailResult == Logic.Models.RequestPasswordResetEmailtResult.EmailNotFound)
//            //{
//            //    return StatusCode(400, "Email Not Found");
//            //}

//            //if (RequestPasswordResetEmailResult == Logic.Models.RequestPasswordResetEmailtResult.EmailAlreadySentInLast24Hours)
//            //{
//            //    return StatusCode(400, "Email Already Sent In Last 24 Hours");
//            //}

//            //if (RequestPasswordResetEmailResult == Logic.Models.RequestPasswordResetEmailtResult.DatabaseError)
//            //{
//            //    return StatusCode(500);
//            //}

//            return Ok();
//        }

//        [HttpDelete("")]
//        public async Task<ActionResult> DeleteAsync([FromBody]RequestPasswordResetEmailRequest createAccountRequest)
//        {
//            var correlationId = HttpContext.Request.Headers["X-Correlation-ID"].First();

//            //var RequestPasswordResetEmailResult =
//            //await _accountManager.RequestPasswordResetEmailAsync(createAccountRequest.Email, correlationId);

//            //if (RequestPasswordResetEmailResult == Logic.Models.RequestPasswordResetEmailtResult.EmailNotFound)
//            //{
//            //    return NotFound();
//            //}

//            //if (RequestPasswordResetEmailResult == Logic.Models.RequestPasswordResetEmailtResult.EmailNotFound)
//            //{
//            //    return StatusCode(400, "Email Not Found");
//            //}

//            //if (RequestPasswordResetEmailResult == Logic.Models.RequestPasswordResetEmailtResult.EmailAlreadySentInLast24Hours)
//            //{
//            //    return StatusCode(400, "Email Already Sent In Last 24 Hours");
//            //}

//            //if (RequestPasswordResetEmailResult == Logic.Models.RequestPasswordResetEmailtResult.DatabaseError)
//            //{
//            //    return StatusCode(500);
//            //}

//            return Ok();
//        }

//        internal string TryGetHeaderValue(HttpRequest request, string key)
//        {
//            return request?.Headers?[key].FirstOrDefault()?.ToString();
//        }
//    }
//}
