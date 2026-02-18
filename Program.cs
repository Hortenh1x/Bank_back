using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Bank_business.repositories;
using Bank_business.Services;

namespace Bank_back
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 1. REGISTER YOUR SERVICES (Dependency Injection)
            // We use 'Scoped' so a new instance is created per HTTP request
            builder.Services.AddScoped<UserRepository>();
            builder.Services.AddScoped<AuthService>();

            // 2. CONFIGURE JWT AUTHENTICATION
            var jwtSettings = builder.Configuration.GetSection("ApplicationSettings");
            var secretKey = jwtSettings.GetValue<string>("JWT_Secret");

            if (string.IsNullOrEmpty(secretKey))
            {
                throw new Exception("JWT Secret is missing from appsettings.json!");
            }

            // Add services to the container.
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
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            var app = builder.Build();

            // 3. MIDDLEWARE ORDER (This part is critical!)
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
