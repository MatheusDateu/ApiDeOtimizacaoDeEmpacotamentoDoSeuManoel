using ManoelsPackaging.Api.Core.Interfaces;
using ManoelsPackaging.Api.Core.Services;
using ManoelsPackaging.Api.Infrastructure.Persistence;
using ManoelsPackaging.Api.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ManoelsPackaging.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            #region Database
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");
            }

            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(connectionString));
            #endregion

            builder.Services.AddControllers();

            #region Swagger Settings
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options => { 
               options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "Manoel's Packaging API",
                    Version = "v1",
                    Description = "API for managing packaging operations including products and boxes."
                });
            });
            #endregion

            #region Dependency Injection
            builder.Services.AddScoped<IBoxRepository, SqlBoxRepository>();
            builder.Services.AddScoped<IPackagingOptimisationService, PackagingOptimisationService>();
            #endregion

            var app = builder.Build();

            // app.UseAuthentication();
            // app.UseAuthorization();

            //app.UseHttpsRedirection();

            #region Apply Migrations Automatically
            /// <summary>
            /// This block applies pending EF Core migrations at application startup.
            /// It includes a retry mechanism to handle cases where the database
            /// might not be immediately available (e.g., in containerized environments).
            /// </summary>
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var logger = services.GetRequiredService<ILogger<Program>>();
                var context = services.GetRequiredService<AppDbContext>();

                try
                {
                    const int maxRetryAttempts = 3;
                    const int retryDelaySeconds = 3;
                    int currentRetry = 0;

                    while (currentRetry < maxRetryAttempts)
                    {
                        try
                        {
                            logger.LogInformation("Attempting to apply database migrations (Attempt: {AttemptNumber})...", currentRetry + 1);
                            context.Database.Migrate();
                            logger.LogInformation("Database migrations applied successfully at startup.");
                            break;
                        }
                        catch (Exception ex)
                        {
                            logger.LogError(ex, "Error applying migrations on attempt {AttemptNumber}. Retrying in {DelaySeconds} seconds...", currentRetry + 1, retryDelaySeconds);
                            currentRetry++;
                            if (currentRetry >= maxRetryAttempts)
                            {
                                logger.LogError(ex, "Failed to apply database migrations after {MaxAttempts} attempts. Application startup might be affected.");
                                throw;
                            }
                            System.Threading.Thread.Sleep(TimeSpan.FromSeconds(retryDelaySeconds));
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "A final error occurred after attempting to migrate the database at startup.");
                }
            }
            #endregion

            #region Swagger Usages
            app.UseSwagger();
            app.UseSwaggerUI();
            #endregion

            app.MapControllers();
            app.MapGet("/", () => "Bem-vindo à API Otimização de Empacotamento - Loja do seu Manoel, desenvolvida por mdelmondes5@outlook.com. Acesse '{esta_url}/swagger' para a documentação da API.");

            app.Run();
        }
    }
}