// Startup activates the OAuth server only when NineStarKiOAuthEnabled=true.
// Before enabling it, configure the OpenIddict client and persistent signing keys.
// Preserve existing FormsAuthentication/WebSecurity.
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Autofac.Integration.Owin;
using K9.DataAccessLayer.Database;
using Microsoft.Extensions.DependencyInjection;
using OpenIddict.Server.Owin;
using Owin;
using OpenIddict.Abstractions;
using K9.WebApplication.Services;
using System.Threading.Tasks;
using System;
using System.Configuration;
using System.Security.Cryptography.X509Certificates;

namespace K9.WebApplication
{
    internal static class OAuthServerConfiguration
    {
        private const string IssuerSetting = "NineStarKiOAuthIssuer";
        private const string ClientIdSetting = "NineStarKiOAuthChatGptClientId";
        private const string RedirectUriSetting = "NineStarKiOAuthChatGptRedirectUri";
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
                    options.SetIssuer(LoadIssuer());
                    // Relative paths preserve the website's IIS application path.
                    options.SetAuthorizationEndpointUris("oauth/authorize");
                    options.SetTokenEndpointUris("oauth/token");
                    options.AllowAuthorizationCodeFlow();
                    options.RequireProofKeyForCodeExchange();
                    options.RegisterScopes(OAuthClaimsPrincipalFactory.ReadScope);
                    // This server issues delegated access tokens only. Do not
                    // advertise openid: the controller deliberately rejects it.
                    options.Configure(configuration =>
                        configuration.Scopes.Remove(OpenIddictConstants.Scopes.OpenId));

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

            var clientId = ConfigurationManager.AppSettings[ClientIdSetting];
            var redirectUri = ConfigurationManager.AppSettings[RedirectUriSetting];
            if (!string.IsNullOrWhiteSpace(clientId) ||
                !string.IsNullOrWhiteSpace(redirectUri))
            {
                // Validate both settings before accepting traffic. No guessed
                // callback or default client is registered.
                OAuthChatGptClientRegistration.CreateDescriptor(clientId, redirectUri);

                // Defer DB access until requests begin, after Application_Start
                // has run the existing DataAccess migrator. One task per worker
                // serialises first-request registration and caches any failure.
                // A failed registration requires a config fix and app recycle.
                var registration = new Lazy<Task>(async () =>
                {
                    using (var scope = container.BeginLifetimeScope())
                    {
                        var manager = scope.Resolve<IOpenIddictApplicationManager>();
                        await OAuthChatGptClientRegistration.RegisterAsync(
                            manager, clientId, redirectUri);
                    }
                }, true);

                app.Use(async (context, next) =>
                {
                    await registration.Value;
                    await next();
                });
            }

            app.UseMiddlewareFromContainer<OpenIddictServerOwinMiddleware>();
        }

        private static Uri LoadIssuer()
        {
            var supplied = ConfigurationManager.AppSettings[IssuerSetting];
            Uri issuer;
            if (string.IsNullOrWhiteSpace(supplied) ||
                !Uri.TryCreate(supplied.Trim(), UriKind.Absolute, out issuer) ||
                issuer.Scheme != Uri.UriSchemeHttps ||
                string.IsNullOrEmpty(issuer.Host) ||
                !string.IsNullOrEmpty(issuer.UserInfo) ||
                !string.IsNullOrEmpty(issuer.Query) ||
                !string.IsNullOrEmpty(issuer.Fragment) ||
                !issuer.AbsolutePath.EndsWith("/", StringComparison.Ordinal))
            {
                throw new ConfigurationErrorsException(
                    IssuerSetting + " must be an absolute HTTPS URL ending in '/', without credentials, a query or a fragment.");
            }

            return issuer;
        }

        private static X509Certificate2 LoadCertificate(string setting)
        {
            // Settings are environment-specific. No secrets in source control.
            var supplied = ConfigurationManager.AppSettings[setting];
            if (string.IsNullOrWhiteSpace(supplied))
            {
                throw new ConfigurationErrorsException("Missing OAuth certificate setting: " + setting);
            }

            var thumbprint = string.Concat(supplied.Split((char[])null, StringSplitOptions.RemoveEmptyEntries));
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
