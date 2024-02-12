// ********************************** 
// Densen Informatica 中讯科技 
// 作者：Alex Chow
// e-mail:zhouchuanglin@gmail.com 
// **********************************
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Security.Claims;

namespace OidcClientShared;

public class OidcProfile
{ 

    public static void OidcDIY(OpenIdConnectOptions options)
    {
        var authority = "https://ids2.app1.es/";
        //authority = "https://localhost:5001/";
        var clientId = "Blazor5002";
        var callbackEndPoint = "http://localhost:5002";

        options.Authority = authority;
        options.ClientId = clientId;
        options.ResponseType = OpenIdConnectResponseType.Code;
        options.ResponseMode = OpenIdConnectResponseMode.Query;

        options.SignedOutRedirectUri = callbackEndPoint;
        options.CallbackPath = "/authentication/login-callback";
        options.SignedOutCallbackPath = "/authentication/logout-callback";
        options.Scope.Add("BlazorWasmIdentity.ServerAPI openid profile");

        options.GetClaimsFromUserInfoEndpoint = true;
        options.SaveTokens = true;
        options.MapInboundClaims = false;
        options.ClaimActions.MapAll();
        options.ClaimActions.MapJsonKey(ClaimTypes.Name, "name");
        options.ClaimActions.MapJsonKey(ClaimValueTypes.Email, "email", ClaimValueTypes.Email);
        options.ClaimActions.MapJsonKey(ClaimTypes.Role, "role");

        options.Events = new OpenIdConnectEvents
        {
            OnAccessDenied = context =>
            {
                context.HandleResponse();
                context.Response.Redirect("/");
                return Task.CompletedTask;
            },

            OnTokenValidated = context =>
            {
                var token = context.TokenEndpointResponse?.AccessToken;
                if (!string.IsNullOrEmpty(token))
                {
                    if (context.Principal?.Identity != null)
                    {
                        var identity = context.Principal!.Identity as ClaimsIdentity;
                        identity!.AddClaim(new Claim("AccessToken", token)); 
                    }

                }

                return Task.CompletedTask;
            }

        };

    }

}
