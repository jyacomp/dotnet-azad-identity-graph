using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;
using Microsoft.AspNetCore.Builder;
using Infrastructure.Identity.Graph;

namespace Infrastructure.Identity
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureIdentity(this IServiceCollection services, IConfiguration Configuration)
        {
            services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApp(options =>
                {
                    Configuration.Bind("AzureAd", options);
                    options.Prompt = "select_account";
                    options.Events.OnTokenValidated = async context => await GraphEventService.OnTokenValidated(context);
                    options.Events.OnAuthenticationFailed = context => GraphEventService.OnAuthenticationFailed(context);
                    options.Events.OnRemoteFailure = context => GraphEventService.OnRemoteFailure(context);
                })
                .EnableTokenAcquisitionToCallDownstreamApi(options =>
                {
                    Configuration.Bind("GraphApi", options);
                }, GraphConstants.Scopes)
                .AddMicrosoftGraph(options => {
                    options.Scopes = string.Join(' ', GraphConstants.Scopes);
                })
                .AddInMemoryTokenCaches();

            return services;
        }
        public static IApplicationBuilder AddInfrastructureIdentity(this IApplicationBuilder app)
        {
            return app;
        }
    }
}
