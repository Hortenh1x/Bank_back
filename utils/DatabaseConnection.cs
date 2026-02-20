using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace Bank_back.utils
{
    public static class DatabaseConnection
    {
        public static string ResolveConnectionString(IConfiguration configuration)
        {
            string? configured = configuration.GetConnectionString("Default");
            if (!string.IsNullOrWhiteSpace(configured))
            {
                return configured;
            }

            string? envValue = Environment.GetEnvironmentVariable("BANK_DB_PATH");
            if (!string.IsNullOrWhiteSpace(envValue))
            {
                envValue = envValue.Trim();
                if (envValue.Contains('='))
                {
                    return envValue;
                }

                return $"Data Source={envValue}";
            }

            string fallbackPath = Path.Combine(AppContext.BaseDirectory, "bank_db.db");
            return $"Data Source={fallbackPath}";
        }
    }
}
