using K9.DataAccessLayer.Models;
using System;
using WebMatrix.WebData;

namespace K9.WebApplication.Services
{
    /// <summary>
    /// Server-verified identity to be used by the future OpenIddict authorisation
    /// endpoint. Both values come from the current website login and its
    /// existing UserMembership records, never from an OAuth request parameter.
    /// This class does not issue tokens or modify website membership.
    /// </summary>
    public sealed class OAuthAccountIdentity
    {
        public OAuthAccountIdentity(int userId, string membershipAccountNumber)
        {
            if (userId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(userId));
            }

            if (string.IsNullOrWhiteSpace(membershipAccountNumber))
            {
                throw new ArgumentException("An active membership is required.", nameof(membershipAccountNumber));
            }

            UserId = userId;
            MembershipAccountNumber = membershipAccountNumber;
        }

        public int UserId { get; }

        public string MembershipAccountNumber { get; }
    }

    public interface IOAuthAccountIdentityService
    {
        /// <summary>
        /// Resolve only the current authenticated MVC Membership user and their
        /// existing active subscription. Null means authentication/membership
        /// is unavailable and token issuance must stop.
        /// </summary>
        OAuthAccountIdentity ResolveCurrentUser();
    }

    public sealed class OAuthAccountIdentityService : IOAuthAccountIdentityService
    {
        private readonly IMembershipService _membershipService;

        public OAuthAccountIdentityService(IMembershipService membershipService)
        {
            _membershipService = membershipService ??
                throw new ArgumentNullException(nameof(membershipService));
        }

        public OAuthAccountIdentity ResolveCurrentUser()
        {
            // Reuse the original WebSecurity/FormsAuthentication login. Do not
            // accept a user ID or account number supplied by a ChatGPT client.
            if (!WebSecurity.IsAuthenticated)
            {
                return null;
            }

            var authenticatedUserId = WebSecurity.CurrentUserId;
            if (authenticatedUserId <= 0)
            {
                return null;
            }

            UserMembership membership =
                _membershipService.GetExistingActiveMembershipForOAuth(authenticatedUserId);
            if (membership == null || membership.UserId != authenticatedUserId ||
                !membership.IsActive || string.IsNullOrWhiteSpace(membership.AccountNumber))
            {
                return null;
            }

            return new OAuthAccountIdentity(authenticatedUserId, membership.AccountNumber);
        }
    }
}
