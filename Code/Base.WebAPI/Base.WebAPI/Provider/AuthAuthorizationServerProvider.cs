using System.Threading.Tasks;
using Microsoft.Owin.Security.OAuth;
using System.Security.Claims;
using System.Collections.Generic;
using System.Security.Principal;
using System.Threading;
using System;

namespace Base.WebAPI.Provider
{
    public class AuthAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        private Func<string,string, List<Claim>>_validateUserPassword;

        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
        }

        public AuthAuthorizationServerProvider()
        {

        }


        public AuthAuthorizationServerProvider(Func<string,string, List<Claim>> validateUserPassword)
        {
            _validateUserPassword = validateUserPassword;
        }


        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });
            try
            {
                var user = context.UserName;
                var pass = context.Password;

                var claims = new List<Claim>();

                claims = _validateUserPassword?.Invoke(user, pass);

                var roles = new List<string> { "User" };

                var identity = new ClaimsIdentity(context.Options.AuthenticationType);
                identity.AddClaim(new Claim(ClaimTypes.Name, user));
                if (claims?.Count > 0)
                    foreach (var claim in claims)
                        identity.AddClaim(claim);
               

                var principal = new GenericPrincipal(identity, roles.ToArray());
                Thread.CurrentPrincipal = principal;
                context.Validated(identity);
            }
            catch (Exception ex)
            {
                context.SetError("invalid_grant", "Falha ao autenticar");
            }
        }

   

    }
}
