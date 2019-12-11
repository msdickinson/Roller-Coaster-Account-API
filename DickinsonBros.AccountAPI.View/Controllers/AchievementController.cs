//Select

//(Non controller)
//At least 1-100 User View your coaster
//Build x Tracks
//Ride x Coasters
//Coaster Stats (Long Coaster, So Many Tracks, Short Coaster, Avg Speed)



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
//    public class AchievementsController : ControllerBase
//    {
//        internal readonly ILoggingService<AchievementsController> _logger;
//        internal readonly IAccountManager _accountManager;
//        public AchievementsController
//        (
//            ILoggingService<AchievementsController> logger
//        )
//        {
//            _logger = logger;
//        }

//        [HttpGet("")]
//        public async Task<ActionResult> Select()
//        {
//            var correlationId = HttpContext.Request.Headers["X-Correlation-ID"].First();

//            //Check Auth
//            // if (createAccountDescriptor.Result == Logic.Models.CreateAccountResult.DatabaseError)
//            {
//                return StatusCode(401);
//            }


//            //Call Modal Select with AccountId

//            //var createAccountDescriptor =
//            //    await _accountManager.CreateAsync
//            //    (
//            //        accountId,
//            //        correlationId
//            //    );

//          //  if (createAccountDescriptor.Result == Logic.Models.CreateAccountResult.DatabaseError)
//           // {
//           //     return StatusCode(500);
//           // }

//            //return StatusCode(200, createAccountDescriptor.AccountId);
//        }
       
//    }
//}
