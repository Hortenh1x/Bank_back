using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Bank_back.repositories;
using Bank_back.Services;
using Bank_back.services;
using Microsoft.OpenApi.Models;

namespace Bank_back
{
    public class Program
    {
        public static void Main(string[] args)
        {
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

            builder.Services.AddControllers();
            builder.Services.AddAuthorization();

            builder.Services.AddControllers();

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
            app.UseHttpsRedirection();

            // Authentication must come BEFORE Authorization
            app.UseAuthentication();




            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
