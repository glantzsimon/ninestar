// Stage only. Do not enable NINESTAR_OAUTH until the DataAccess LocalDb
// migration has been generated and integration-tested, the OWIN middleware
// adapter has been installed, and persistent signing keys are provisioned.
// The existing MVC FormsAuthentication/WebSecurity login remains authoritative.
#if NINESTAR_OAUTH
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Autofac.Integration.Owin;
using K9.DataAccessLayer.Database;
using Microsoft.Extensions.DependencyInjection;
using OpenIddict.Server.Owin;
using Owin;
using System;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace K9.WebApplication
{
    internal static class OAuthServerConfiguration
    {
        private const string IssuerUrl = "https://9starkiastrology.com/";
        private const string SigningThumbprintSetting = "NineStarKiOAuthSigningCertificateThumbprint";
        private const string EncryptionThumbprintSetting = "NineStarKiOAuthEncryptionCertificateThumbprint";

        public static void RegisterServices(ContainerBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            // OpenIddict uses the Microsoft DI registration API internally.
            // Import its registrations into the website's EXISTING Autofac
            // container. Never construct a second container for OAuth.
            var services = new ServiceCollection();

            services.AddOpenIddict()
                .AddCore(options =>
                {
                    // Use the single existing website DataAccess context.
                    // LocalDb is registered AsSelf and As<DbContext> in Startup.
                    options.UseEntityFramework().UseDbContext<LocalDb>();
                })
                .AddServer(options =>
                {
                    options.SetIssuer(new Uri(IssuerUrl));
                    options.SetAuthorizationEndpointUris("/oauth/authorize");
                    options.SetTokenEndpointUris("/oauth/token");
                    options.AllowAuthorizationCodeFlow();
                    options.RequireProofKeyForCodeExchange();
                    options.RegisterScopes(Services.OAuthClaimsPrincipalFactory.ReadScope);

                    options.SetAccessTokenLifetime(TimeSpan.FromMinutes(15));

                    // The separate MCP server validates the JWT against the
                    // website's published signing key/JWKS. Access tokens
                    // must therefore be signed but not JWE-encrypted.
                    options.DisableAccessTokenEncryption();

                    // Never generate development certificates or commit keys.
                    // Distinct persistent X.509 keys must be provisioned in
                    // the local-machine certificate store for each environment.
                    options.AddSigningCertificate(LoadCertificate(SigningThumbprintSetting));
                    options.AddEncryptionCertificate(LoadCertificate(EncryptionThumbprintSetting));

                    options.UseOwin()
                        .EnableAuthorizationEndpointPassthrough()
                        .EnableTokenEndpointPassthrough();
                });

            builder.Populate(services);
        }

        public static void RegisterMiddleware(IAppBuilder app, IContainer container)
        {
            if (app == null) throw new ArgumentNullException(nameof(app));
            if (container == null) throw new ArgumentNullException(nameof(container));

            // Per-request scope for OpenIddict stores and protocol handlers,
            // without replacing the site's existing MVC Autofac integration.
            app.UseAutofacLifetimeScopeInjector(container);
            app.UseMiddlewareFromContainer<OpenIddictServerOwinMiddleware>();
        }

        private static X509Certificate2 LoadCertificate(string setting)
        {
            // Settings are environment-specific. No secrets in source control.
            var supplied = ConfigurationManager.AppSettings[setting];
            if (string.IsNullOrWhiteSpace(supplied))
            {
                throw new ConfigurationErrorsException("Missing OAuth certificate setting: " + setting);
            }

            var thumbprint = new string(supplied.Where(c => !char.IsWhiteSpace(c)).ToArray());
            using (var store = new X509Store(StoreName.My, StoreLocation.LocalMachine))
            {
                store.Open(OpenFlags.ReadOnly);
                var matches = store.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, false);
                if (matches.Count != 1 || !matches[0].HasPrivateKey ||
                    DateTime.UtcNow < matches[0].NotBefore.ToUniversalTime() ||
                    DateTime.UtcNow >= matches[0].NotAfter.ToUniversalTime())
                {
                    throw new ConfigurationErrorsException(
                        "OAuth certificate missing, expired or without a private key: " + setting);
                }

                return matches[0];
            }
        }
    }
}
#endif
