using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Bank_back.repositories;
using Bank_back.Services;

namespace Bank_back
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //use 'Scoped' so a new instance is created per HTTP request
            builder.Services.AddScoped<UserRepository>();
            builder.Services.AddScoped<AuthService>();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

            var jwtSettings = builder.Configuration.GetSection("ApplicationSettings");
            var secretKey = jwtSettings.GetValue<string>("JWT_Secret");

            if (string.IsNullOrEmpty(secretKey))
            {
                throw new Exception("JWT Secret is missing from appsettings.json");
            }

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = "Bank",        // Must match AuthService
                    ValidAudience = "Clients",    // Must match AuthService
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
                };
            });

            builder.Services.AddControllers();
            builder.Services.AddAuthorization();

            builder.Services.AddControllers();

            builder.Services.AddOpenApi();

            var app = builder.Build();

            // 3. MIDDLEWARE ORDER
            app.UseHttpsRedirection();

            // Authentication must come BEFORE Authorization
            app.UseAuthentication();


            

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
