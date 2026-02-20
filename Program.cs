using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Bank_back.repositories;
using Bank_back.Services;
using Bank_back.services;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;
using Bank_back.utils;


namespace Bank_back
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // string myNewPepper = CryptoUtility.GenerateSecureString(32);
            // Console.WriteLine(myNewPepper);

            // string myNewSalt = CryptoUtility.GenerateSecureString(16);
            // Console.WriteLine(myNewSalt);

            var builder = WebApplication.CreateBuilder(args);

            //use 'Scoped' so a new instance is created per HTTP request
            builder.Services.AddScoped<UserRepository>();
            builder.Services.AddScoped<AccountRepository>();
            builder.Services.AddScoped<TransactionRepository>();
            builder.Services.AddScoped<AuthService>();
            builder.Services.AddScoped<AccountService>();
            builder.Services.AddScoped<TransactionService>();
            builder.Services.AddScoped<UserService>();
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

            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });
            builder.Services.AddAuthorization();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "Bank API", Version = "v1" });

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter your JWT token as: Bearer {token}"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            var app = builder.Build();

            // 3. MIDDLEWARE ORDER
            // Authentication must come BEFORE Authorization
            app.UseAuthentication();

            // Avoid warning "Failed to determine the https port for redirect" when app runs on HTTP-only profile.
            if (ShouldUseHttpsRedirection(app.Configuration))
            {
                app.UseHttpsRedirection();
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }

        private static bool ShouldUseHttpsRedirection(IConfiguration configuration)
        {
            string? urls = configuration["ASPNETCORE_URLS"] ?? configuration["urls"];
            if (!string.IsNullOrWhiteSpace(urls) &&
                urls.Contains("https://", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            string? httpsPorts =
                configuration["ASPNETCORE_HTTPS_PORTS"] ??
                configuration["HTTPS_PORT"] ??
                configuration["https_port"];

            return !string.IsNullOrWhiteSpace(httpsPorts);
        }
    }
}
