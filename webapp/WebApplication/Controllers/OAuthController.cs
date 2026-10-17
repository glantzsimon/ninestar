// All OAuth endpoints are opt-in. Do not enable this symbol before a
// complete DataAccess migration, client registration, signing keys, and
// integration testing. Existing website authentication is unchanged.
#if NINESTAR_OAUTH
using K9.WebApplication.Services;
using Microsoft.Owin.Security;
using OpenIddict.Abstractions;
using OpenIddict.Server.Owin;
using Owin;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebMatrix.WebData;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace K9.WebApplication.Controllers
{
    [RoutePrefix("oauth")]
    public sealed class OAuthController : Controller
    {
        private readonly IOpenIddictApplicationManager _applications;
        private readonly IOAuthAccountIdentityService _accounts;
        private readonly IMembershipService _memberships;

        public OAuthController(
            IOpenIddictApplicationManager applications,
            IOAuthAccountIdentityService accounts,
            IMembershipService memberships)
        {
            _applications = applications ?? throw new ArgumentNullException(nameof(applications));
            _accounts = accounts ?? throw new ArgumentNullException(nameof(accounts));
            _memberships = memberships ?? throw new ArgumentNullException(nameof(memberships));
        }

        [HttpGet, AllowAnonymous, Route("authorize")]
        public async Task<ActionResult> Authorize()
        {
            var context = HttpContext.GetOwinContext();
            var request = context.GetOpenIddictServerRequest();
            if (request == null)
            {
                return new HttpStatusCodeResult(400);
            }

            // Only one read-only permission, code flow and S256 PKCE are
            // permitted for the ChatGPT client. The middleware also validates
            // registered redirect URIs and code challenges before pass-through.
            if (!IsExpectedRequest(request))
            {
                return OAuthError(Errors.InvalidRequest);
            }

            if (!WebSecurity.IsAuthenticated)
            {
                if (request.HasPromptValue(PromptValues.None))
                {
                    return OAuthError(Errors.LoginRequired);
                }

                // Login's returnUrl handler accepts only local paths. It must
                // NEVER redirect to the OAuth client's external callback.
                if (!Url.IsLocalUrl(Request.RawUrl))
                {
                    return new HttpStatusCodeResult(400);
                }

                return RedirectToAction("Login", "Account", new { returnUrl = Request.RawUrl });
            }

            // Re-authentication freshness cannot be established from the
            // legacy membership cookie alone. Fail closed rather than
            // falsely claiming to honour prompt=login or max_age.
            if (request.HasPromptValue(PromptValues.Login) || request.MaxAge != null)
            {
                return OAuthError(Errors.LoginRequired);
            }

            if (_accounts.ResolveCurrentUser() == null)
            {
                return OAuthError(Errors.AccessDenied);
            }

            // Always require interactive, explicit user approval. Do not
            // silently infer consent from the website's login cookie.
            if (request.HasPromptValue(PromptValues.None))
            {
                return OAuthError(Errors.ConsentRequired);
            }

            var application = await _applications.FindByClientIdAsync(request.ClientId);
            if (application == null)
            {
                return OAuthError(Errors.InvalidClient);
            }

            return View("~/Views/OAuth/Consent.cshtml", new OAuthConsentViewModel
            {
                ApplicationName = await _applications.GetDisplayNameAsync(application)
                    ?? request.ClientId,
                Scope = OAuthClaimsPrincipalFactory.ReadScope
            });
        }

        [HttpPost, AllowAnonymous, Route("authorize"), ValidateAntiForgeryToken]
        public ActionResult AuthorizePost(string decision)
        {
            var context = HttpContext.GetOwinContext();
            var request = context.GetOpenIddictServerRequest();
            if (request == null)
            {
                return new HttpStatusCodeResult(400);
            }

            if (!IsExpectedRequest(request))
            {
                return OAuthError(Errors.InvalidRequest);
            }

            if (!string.Equals(decision, "approve", StringComparison.Ordinal))
            {
                return OAuthError(Errors.AccessDenied);
            }

            // Re-check authentication and membership AFTER the consent form
            // is submitted. Neither ID nor account comes from request data.
            var account = _accounts.ResolveCurrentUser();
            if (account == null)
            {
                return OAuthError(Errors.AccessDenied);
            }

            var principal = OAuthClaimsPrincipalFactory.Create(
                account, request.GetScopes());

            context.Authentication.SignIn((ClaimsIdentity)principal.Identity);
            return new EmptyResult();
        }

        [HttpPost, AllowAnonymous, Route("token")]
        public async Task<ActionResult> Token()
        {
            var context = HttpContext.GetOwinContext();
            var request = context.GetOpenIddictServerRequest();
            if (request == null)
            {
                return new HttpStatusCodeResult(400);
            }

            // The token endpoint is a backchannel request from ChatGPT, not
            // a browser request with the website's authentication cookie.
            if (!request.IsAuthorizationCodeGrantType())
            {
                return OAuthError(Errors.UnsupportedGrantType);
            }

            var ticket = await context.Authentication.AuthenticateAsync(
                OpenIddictServerOwinDefaults.AuthenticationType);

            if (ticket?.Identity == null ||
                !int.TryParse(ticket.Identity.FindFirst(Claims.Subject)?.Value,
                    NumberStyles.None, CultureInfo.InvariantCulture, out var userId) ||
                userId <= 0)
            {
                return OAuthError(Errors.InvalidGrant);
            }

            // Re-evaluate the authenticated account's ACTIVE membership at
            // redemption time, to prevent issuing tokens for expired or
            // transferred memberships or blindly copying a stale account ID.
            var membership = _memberships.GetExistingActiveMembershipForOAuth(userId);
            if (membership == null || membership.UserId != userId ||
                !membership.IsActive ||
                string.IsNullOrWhiteSpace(membership.AccountNumber))
            {
                return OAuthError(Errors.InvalidGrant);
            }

            var principal = OAuthClaimsPrincipalFactory.Create(
                new OAuthAccountIdentity(userId, membership.AccountNumber),
                new ClaimsPrincipal(ticket.Identity).GetScopes());

            context.Authentication.SignIn((ClaimsIdentity)principal.Identity);
            return new EmptyResult();
        }

        private static bool IsExpectedRequest(OpenIddictRequest request)
        {
            var scopes = request.GetScopes().ToArray();
            return string.Equals(request.ResponseType, ResponseTypes.Code, StringComparison.Ordinal) &&
                   string.Equals(request.CodeChallengeMethod, CodeChallengeMethods.Sha256,
                       StringComparison.Ordinal) &&
                   scopes.Length == 1 &&
                   string.Equals(scopes[0], OAuthClaimsPrincipalFactory.ReadScope,
                       StringComparison.Ordinal);
        }

        private ActionResult OAuthError(string error)
        {
            HttpContext.GetOwinContext().Authentication.Challenge(
                authenticationTypes: OpenIddictServerOwinDefaults.AuthenticationType,
                properties: new AuthenticationProperties(
                    new Dictionary<string, string>
                    {
                        [OpenIddictServerOwinConstants.Properties.Error] = error
                    }));

            return new EmptyResult();
        }
    }

}
#endif

// The Razor view model stays compiled in ordinary builds, even while the
// endpoint itself remains disabled, so MVC view precompilation also succeeds.
namespace K9.WebApplication.Controllers
{
    public sealed class OAuthConsentViewModel
    {
        public string ApplicationName { get; set; }
        public string Scope { get; set; }
    }
}
