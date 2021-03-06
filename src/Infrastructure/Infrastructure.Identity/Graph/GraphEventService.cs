using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Graph;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Net;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Infrastructure.Identity.Graph
{
    public class GraphEventService
    {
        public static async Task<TokenValidatedContext> OnTokenValidated(TokenValidatedContext context)
        {
            var tokenAcquisition = context.HttpContext.RequestServices.GetRequiredService<ITokenAcquisition>();
            var graphClient = new GraphServiceClient(
                new DelegateAuthenticationProvider(async (request) =>
                {
                    var token = await tokenAcquisition.GetAccessTokenForUserAsync(GraphConstants.Scopes, user: context.Principal);
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                })
            );
            var user = await graphClient.Me.Request()
                .Select(u => new
                {
                    u.DisplayName,
                    u.Mail,
                    u.UserPrincipalName,
                    //u.MailboxSettings
                })
                .GetAsync();
            context.Principal.AddUserGraphInfo(user);
            try
            {
                var photo = await graphClient.Me
                    .Photos["48x48"]
                    .Content
                    .Request()
                    .GetAsync();
                context.Principal.AddUserGraphPhoto(photo);
            }
            catch (ServiceException ex)
            {
                if (ex.IsMatch("ErrorItemNotFound") || ex.IsMatch("ConsumerPhotoIsNotSupported"))
                {
                    context.Principal.AddUserGraphPhoto(null);
                }
                else
                {
                    throw;
                }
            }
            return context;
        }

        public static Task<int> OnAuthenticationFailed(AuthenticationFailedContext context)
        {
            var error = WebUtility.UrlEncode(context.Exception.Message);
            context.Response.Redirect($"/Home/ErrorWithMessage?message=Authentication+error&debug={error}");
            context.HandleResponse();

            return Task.FromResult(0);
        }
        public static Task<int> OnRemoteFailure(RemoteFailureContext context)
        {
            if (context.Failure is OpenIdConnectProtocolException)
            {
                var error = WebUtility.UrlEncode(context.Failure.Message);
                context.Response.Redirect($"/Home/ErrorWithMessage?message=Sign+in+error&debug={error}");
                context.HandleResponse();
            }
            return Task.FromResult(0);
        }
    }

}
