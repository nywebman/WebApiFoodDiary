using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Net.Http;
using System.Text;
using WebMatrix.WebData;
using System.Security.Principal;
using System.Threading;
using CountingKs.Data;
using Ninject;

namespace CountingKs.Filters
{
    public class CountingKsAuthorizeAttribute :AuthorizationFilterAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if(Thread.CurrentPrincipal.Identity.IsAuthenticated)
            {
                return;
                //if soeone has gotten here with auth user, dont go through this again
            }

            var authHeader = actionContext.Request.Headers.Authorization;
            if (authHeader != null)
            {
                if (authHeader.Scheme.Equals("basic", StringComparison.OrdinalIgnoreCase) &&
                    !string.IsNullOrWhiteSpace(authHeader.Parameter))
                {
                    var rawCredentials = authHeader.Parameter;
                    var encoding = Encoding.GetEncoding("iso-8859-1");
                    var credentials = encoding.GetString(Convert.FromBase64String(rawCredentials));
                    var split = credentials.Split(':');
                    var username = split[0];
                    var password = split[1];

                    if (!WebSecurity.Initialized)
                    {
                        WebSecurity.InitializeDatabaseConnection("DefaultConnection", "UserProfile", "UserId", "UserName", autoCreateTables: true); //need to make changes to InitializeSimpleMembershipAttribute class
                    }

                    if (WebSecurity.Login(username,password))
                    {
                        var principal = new GenericPrincipal(new GenericIdentity(username), null);
                        Thread.CurrentPrincipal = principal;
                        return;
                    }
                }
            }

            HandleUnauthorized(actionContext);
        }

        private void HandleUnauthorized(HttpActionContext actionContext)
        {
            //createresponse method needs System.Net.Http 
            actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);

            if (_perUser)
            {
                actionContext.Response.Headers.Add("WWW-Authenticate",
                    "Basic Scheme='CountingKs' location='http://localhost:8901/account/login'");
            }
        }
        
    }
}