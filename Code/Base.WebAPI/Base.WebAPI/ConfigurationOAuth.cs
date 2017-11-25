using System;
using Microsoft.Owin;
using Owin;
using Microsoft.Owin.Security.OAuth;
using Microsoft.Owin.Security;
using Base.WebAPI.Provider;
using System.Security.Claims;
using System.Collections.Generic;

namespace Base.WebAPI
{
    public class ConfigurationOAuth
    {

        public static OAuthBearerAuthenticationOptions OAuthBearerOptions { get; private set; }


        public static void ConfigureOAuth(IAppBuilder app, IOAuthAuthorizationServerProvider provider, TimeSpan accessTokenExpireTimeSpan)
        {
            app.UseExternalSignInCookie(Microsoft.AspNet.Identity.DefaultAuthenticationTypes.ExternalCookie);
            OAuthBearerOptions = new OAuthBearerAuthenticationOptions();

            var OAutServerOption = new OAuthAuthorizationServerOptions
            {
                AuthenticationMode = AuthenticationMode.Active,
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/api/security/token"),
                AccessTokenExpireTimeSpan = accessTokenExpireTimeSpan,
                Provider = provider              
            };

            app.UseOAuthAuthorizationServer(OAutServerOption);
            app.UseOAuthBearerAuthentication(OAuthBearerOptions);
        }

    }
}
