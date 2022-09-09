using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Projeto02.Services.Api.Security
{
    public static class JwtConfiguration
    {
        public static void AddJwtBearerConfiguration(WebApplicationBuilder builder)
        {
            var settingsSection = builder.Configuration.GetSection("JwtTokenSettings");
            builder.Services.Configure<JwtTokenSettings>(settingsSection);

            var appSettings = settingsSection.Get<JwtTokenSettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.SecretKey);

            builder.Services.AddAuthentication(
                auth =>
                {
                    auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(
                    bearer =>
                    {
                        bearer.RequireHttpsMetadata = false;
                        bearer.SaveToken = true;
                        bearer.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = new SymmetricSecurityKey(key),
                            ValidateIssuer = false,
                            ValidateAudience = false
                        };
                    }
                );
            builder.Services.AddTransient(map => new JwtTokenService(appSettings));
        }
    }
}
