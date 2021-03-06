using Microsoft.Graph;
using System;
using System.IO;
using System.Security.Claims;

namespace Infrastructure.Identity.Graph
{
    public static class GraphConstants
    {
        public readonly static string[] Scopes =
        {
            "User.Read"
        };
        public static string NoImage { get { return "data:image/svg+xml;charset=utf-8,%3Csvg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 64 64' stroke='%23707070'%3E%3Cg fill='none'%3E%3Ccircle cx='32' cy='32' r='30.25' stroke-width='1.5'/%3E%3Cg transform='matrix(.9 0 0 .9 10.431 10.431)' stroke-width='2'%3E%3Ccircle cx='24.25' cy='18' r='9'/%3E%3Cpath d='M11.2 40a1 1 0 1126.1 0'/%3E%3C/g%3E%3C/g%3E%3C/svg%3E"; } }
    }
    public static class GraphClaimTypes
    {
        public const string DisplayName = "graph_name";
        public const string Email = "graph_email";
        public const string Photo = "graph_photo";
        public const string TimeZone = "graph_timezone";
        public const string TimeFormat = "graph_timeformat";
        public const string GivenName = "graph_givenname";
        public const string Surname = "graph_surname";
    }
    public static class GraphClaimsPrincipalExtensions
    {
        public static string GetUserGraphDisplayName(this ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.FindFirstValue(GraphClaimTypes.DisplayName);
        }

        public static string GetUserGraphEmail(this ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.FindFirstValue(GraphClaimTypes.Email);
        }

        public static string GetUserGraphPhoto(this ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.FindFirstValue(GraphClaimTypes.Photo);
        }

        //public static string GetUserGraphTimeZone(this ClaimsPrincipal claimsPrincipal)
        //{
        //    return claimsPrincipal.FindFirstValue(GraphClaimTypes.TimeZone);
        //}

        //public static string GetUserGraphTimeFormat(this ClaimsPrincipal claimsPrincipal)
        //{
        //    return claimsPrincipal.FindFirstValue(GraphClaimTypes.TimeFormat);
        //}

        public static void AddUserGraphInfo(this ClaimsPrincipal claimsPrincipal, User user)
        {
            var identity = claimsPrincipal.Identity as ClaimsIdentity;

            identity.AddClaim(new Claim(GraphClaimTypes.DisplayName, user.DisplayName));
            identity.AddClaim(new Claim(GraphClaimTypes.Email, user.Mail ?? user.UserPrincipalName));
            identity.AddClaim(new Claim(GraphClaimTypes.GivenName, user.GivenName ?? ""));
            identity.AddClaim(new Claim(GraphClaimTypes.Surname, user.Surname ?? ""));
            //identity.AddClaim(new Claim(GraphClaimTypes.TimeZone,user.MailboxSettings.TimeZone));
            //identity.AddClaim(new Claim(GraphClaimTypes.TimeFormat, user.MailboxSettings.TimeFormat));
        }

        public static void AddUserGraphPhoto(this ClaimsPrincipal claimsPrincipal, Stream photoStream)
        {
            var identity = claimsPrincipal.Identity as ClaimsIdentity;

            if (photoStream == null)
            {
                identity.AddClaim(new Claim(GraphClaimTypes.Photo, GraphConstants.NoImage));
                return;
            }

            var memoryStream = new MemoryStream();
            photoStream.CopyTo(memoryStream);
            var photoBytes = memoryStream.ToArray();

            var photoUrl = $"data:image/png;base64,{Convert.ToBase64String(photoBytes)}";

            identity.AddClaim(new Claim(GraphClaimTypes.Photo, photoUrl));
        }
    }
}