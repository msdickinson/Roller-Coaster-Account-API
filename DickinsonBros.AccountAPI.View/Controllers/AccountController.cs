﻿using System.Linq;
using System.Threading.Tasks;
using DickinsonBros.AccountAPI.View.Models;
using Microsoft.AspNetCore.Mvc;
using DickinsonBros.AccountAPI.Infrastructure.Logging;
using DickinsonBros.AccountAPI.Logic.Account;
using DickinsonBros.AccountAPI.Logic.Account.Models;
using DickinsonBros.AccountAPI.Infrastructure.Encryption;
using DickinsonBros.AccountAPI.Infrastructure.DateTime;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace DickinsonBros.AccountAPI.View.Controllers
{
    [Authorize]
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    public class AccountController : ControllerBase
    {
        internal readonly ILoggingService<AccountController> _logger;
        internal readonly IAccountManager _accountManager;
        internal readonly IEncryptionService _encryptionService;
        internal readonly IDateTimeService _dateTimeService;

        internal const int FIFTEEN_MIN_IN_SECONDS = 900;
        internal const int TWO_HOURS_IN_SECONDS = 7200;
        public AccountController
        (
            ILoggingService<AccountController> logger,
            IAccountManager accountManager,
            IEncryptionService encryptionService,
            IDateTimeService dateTimeService
        )
        {
            _logger = logger;
            _accountManager = accountManager;
            _encryptionService = encryptionService;
            _dateTimeService = dateTimeService;
        }
 
        [AllowAnonymous]
        [HttpPost("Create")]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(JWTResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult> CreateAsync([FromBody]CreateAccountRequest createAccountRequest)
        {
            var createAccountDescriptor =
                await _accountManager.CreateAsync
                (
                    createAccountRequest.Username,
                    createAccountRequest.Password,
                    createAccountRequest.Email
                );

            if (createAccountDescriptor.Result == CreateAccountResult.DuplicateUser)
            {
                return StatusCode(409);
            }

            string accountId = Convert.ToString(createAccountDescriptor.AccountId);

            return new ObjectResult
            (
                GetNewJWTTokens(accountId)
            );
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        [ProducesResponseType(typeof(JWTResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> LoginAsync([FromBody]LoginRequest createAccountRequest)
        {
            var loginRequest =
                await _accountManager.LoginAsync(createAccountRequest.Username, createAccountRequest.Password);

            if (loginRequest.Result == LoginResult.InvaildPassword)
            {
                return StatusCode(401);
            }

            if (loginRequest.Result == LoginResult.AccountLocked)
            {
                return StatusCode(403);
            }

            if (loginRequest.Result == LoginResult.AccountNotFound)
            {
                return StatusCode(404);
            }

            return new ObjectResult
            (
                GetNewJWTTokens((Convert.ToString(loginRequest.AccountId)))
            );
        }

        [AllowAnonymous]
        [HttpPost("RefreshToken")]
        [ProducesResponseType(typeof(JWTResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult RefreshToken([FromBody]RefreshTokenRequest refreshTokenRequest)
        {
            var accessTokenClaims = _encryptionService.GetPrincipal(refreshTokenRequest.access_token, false, false);
            var refreshTokenClaims = _encryptionService.GetPrincipal(refreshTokenRequest.refresh_token, true);

            if (accessTokenClaims == null ||
                refreshTokenClaims == null ||
                !accessTokenClaims.Identity.IsAuthenticated ||
                !refreshTokenClaims.Identity.IsAuthenticated ||
                accessTokenClaims.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value !=
                refreshTokenClaims.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value)
            {
                return StatusCode(401);
            }

            var accessToken = _encryptionService.GenerateJWT(accessTokenClaims.Claims, _dateTimeService.GetDateTimeUTC().AddSeconds(FIFTEEN_MIN_IN_SECONDS));
            var refreshToken = _encryptionService.GenerateJWT(accessTokenClaims.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value, _dateTimeService.GetDateTimeUTC().AddSeconds(TWO_HOURS_IN_SECONDS), true);

            return new ObjectResult
            (
               new JWTResponse
               {
                   AccessToken = accessToken,
                   AccessTokenExpiresIn = FIFTEEN_MIN_IN_SECONDS,
                   RefreshToken = refreshToken,
                   RefreshTokenExpiresIn = TWO_HOURS_IN_SECONDS,
                   TokenType = "Bearer"
               }
            );
        }

        /// <response code="403">Email Not Activated</response>
        [AllowAnonymous]
        [HttpPost("RequestPasswordResetEmail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> RequestPasswordResetEmailAsync([FromBody]RequestPasswordResetEmailRequest requestPasswordResetEmailRequest)
        {
            var requestPasswordResetEmailResult =
                await _accountManager.RequestPasswordResetEmailAsync(requestPasswordResetEmailRequest.Email);

            if (requestPasswordResetEmailResult == RequestPasswordResetEmailResult.EmailNotFound)
            {
                return StatusCode(404);
            }

            if (requestPasswordResetEmailResult == RequestPasswordResetEmailResult.EmailNotActivated)
            {
                return StatusCode(403, "Email Not Activated");
            }

            if (requestPasswordResetEmailResult == RequestPasswordResetEmailResult.NoEmailSentDueToEmailPreference)
            {
                return StatusCode(403, "No Email Sent Due To Email Preference");
            }

            return StatusCode(200);
        }

        [AllowAnonymous]
        [HttpPost("ResetPassword")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> ResetPasswordAsync([FromBody]ResetPasswordRequest resetPasswordRequest)
        {
            var resetPasswordResult =
                await _accountManager.ResetPasswordAsync(resetPasswordRequest.Token, resetPasswordRequest.NewPassword);

            if (resetPasswordResult == ResetPasswordResult.TokenInvaild)
            {
                return StatusCode(401);
            }

            return StatusCode(200);
        }

        [AllowAnonymous]
        [HttpPost("UpdateEmailPreferenceWithToken")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateEmailPreferenceWithTokenAsync([FromBody]UpdateEmailPreferenceWithTokenRequest updateEmailPreferenceWithTokenRequest)
        {
            var updateEmailPreferenceWithTokenResult =
                await _accountManager.UpdateEmailPreferenceWithTokenAsync(updateEmailPreferenceWithTokenRequest.Token, updateEmailPreferenceWithTokenRequest.EmailPreference);

            if (updateEmailPreferenceWithTokenResult == UpdateEmailPreferenceWithTokenResult.InvaildToken)
            {
                return StatusCode(401);
            }

            return StatusCode(200);
        }

        [HttpPost("UpdateEmailPreference")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateEmailPreferenceAsync([FromBody]UpdateEmailPreferenceRequest updateEmailPreferenceRequest)
        {
            int accountId = Convert.ToInt32(User.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value);

            await _accountManager.UpdateEmailPreferenceAsync(accountId, updateEmailPreferenceRequest.EmailPreference);

            return StatusCode(200);
        }

        [AllowAnonymous]
        [HttpPost("ActivateEmail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> ActivateEmailAsync([FromBody]ActivateEmailRequest activateEmailRequest)
        {
            var activateAccountResult =
                await _accountManager.ActivateEmailAsync(activateEmailRequest.Token);

            if (activateAccountResult == ActivateEmailResult.InvaildToken)
            {
                return StatusCode(401);
            }

            if (activateAccountResult == ActivateEmailResult.EmailWasAlreadyActivated)
            {
                return StatusCode(400, "Email Was Already Activated");
            }
            return StatusCode(200);
        }

        [HttpPost("UpdatePassword")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdatePasswordAsync([FromBody]UpdatePasswordRequest updatePasswordRequest)
        {
            int accountId = Convert.ToInt32(User.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value);

            var updatePasswordResult =
                await _accountManager.UpdatePasswordAsync(accountId, updatePasswordRequest.ExistingPassword, updatePasswordRequest.NewPassword);

            if (updatePasswordResult == UpdatePasswordResult.AccountLocked)
            {
                return StatusCode(403);
            }

            if (updatePasswordResult == UpdatePasswordResult.InvaildExistingPassword)
            {
                return StatusCode(400, "Invaild Existing Password");
            }

            return StatusCode(200);
        }

        internal JWTResponse GetNewJWTTokens(string accountId)
        {
            var accessToken = _encryptionService.GenerateJWT(accountId, _dateTimeService.GetDateTimeUTC().AddSeconds(FIFTEEN_MIN_IN_SECONDS));
            var refreshToken = _encryptionService.GenerateJWT(accountId, _dateTimeService.GetDateTimeUTC().AddSeconds(TWO_HOURS_IN_SECONDS), true);

            return new JWTResponse
            {
                AccessToken = accessToken,
                AccessTokenExpiresIn = FIFTEEN_MIN_IN_SECONDS,
                RefreshToken = refreshToken,
                RefreshTokenExpiresIn = TWO_HOURS_IN_SECONDS,
                TokenType = "Bearer"
            };
        }
    }
}
