using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ZNetCS.AspNetCore.Authentication.Basic;
using ZNetCS.AspNetCore.Authentication.Basic.Events;

namespace TownSuite.ConversionServer.APISite
{
    public class AuthenticationEvents : BasicAuthenticationEvents
    {
        #region Public Methods

        readonly BasicAuthConfigs _config;
        public AuthenticationEvents(BasicAuthConfigs config) : base()
        {
            _config = config;
        }

        /// <inheritdoc/>
        public override Task ValidatePrincipalAsync(ValidatePrincipalContext context)
        {
            // keep the bots away
            if (string.Equals(context.UserName, _config.Username) && string.Equals(context.Password, _config.Password))
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, context.UserName, context.Options.ClaimsIssuer)
                };

                var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, BasicAuthenticationDefaults.AuthenticationScheme));
                context.Principal = principal;
            }

            return Task.CompletedTask;
        }

        #endregion
    }
}
