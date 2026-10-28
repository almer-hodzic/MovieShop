using Market.Shared.Constants;
using Microsoft.EntityFrameworkCore;
using Market.API;
using Market.API.Middleware;
using Market.Application;
using Market.Infrastructure;
using Market.Infrastructure.Common;
using Market.Shared.Options;
using Serilog;

public partial class Program
{
    private static async Task Main(string[] args)
    {
        //
        // 0) Bootstrap logger (very early, no full config yet)
        //
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console() // minimal sink so we see startup errors
            .CreateBootstrapLogger();

        try
        {
            Log.Information("Starting Market API...");

            //
            // 1) Standard builder (includes appsettings.json, appsettings.{ENV}.json,
            //    environment variables, user-secrets (Dev), and command-line args)
            //
            var initializeDatabase = args.Contains("--initialize-database");
            var builder = WebApplication.CreateBuilder(args.Where(arg => arg != "--initialize-database").ToArray());
            if (initializeDatabase)
                builder.Configuration["DatabaseStartup:ApplyMigrationsOnStartup"] = "true";

            var databaseConnection = builder.Configuration["ConnectionStrings:Main"];
            if (string.IsNullOrWhiteSpace(databaseConnection))
                databaseConnection = builder.Configuration[initializeDatabase ? "POSTGRES_URL_NON_POOLING" : "POSTGRES_URL"];
            if (!builder.Environment.IsTest() && !string.IsNullOrWhiteSpace(databaseConnection))
                builder.Configuration["ConnectionStrings:Main"] = PostgresConnection.Normalize(databaseConnection);

            // 2) Promote Serilog to full configuration from builder.Configuration
            //    (reads "Serilog" section from appsettings + ENV overrides)
            //
            builder.Host.UseSerilog((ctx, services, cfg) =>
            {
                if (ctx.HostingEnvironment.IsProduction())
                {
                    cfg.MinimumLevel.Information()
                       .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
                       .Enrich.FromLogContext()
                       .WriteTo.Console();
                    return;
                }
                cfg.ReadFrom.Configuration(ctx.Configuration)   // Serilog section in appsettings
                   .ReadFrom.Services(services)                 // DI enrichers if any
                   .Enrich.FromLogContext()
                   .Enrich.WithThreadId()
                   .Enrich.WithProcessId()
                   .Enrich.WithMachineName();
            });

            // Optional: remove default providers to have only Serilog
            builder.Logging.ClearProviders();

            // ---------------------------------------------------------
            // 3. Layer registrations
            // ---------------------------------------------------------
            builder.Services
                .AddAPI(builder.Configuration, builder.Environment)
                .AddInfrastructure(builder.Configuration, builder.Environment)
                .AddApplication();

            builder.Services.AddDeploymentProtection(builder.Configuration, builder.Environment);
            var port = Environment.GetEnvironmentVariable("PORT");
            if (!string.IsNullOrWhiteSpace(port))
                builder.WebHost.UseUrls($"http://0.0.0.0:{int.Parse(port)}");

            var app = builder.Build();

            // ---------------------------------------------------------
            // 4. Middleware pipeline
            // ---------------------------------------------------------
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();

                var developmentEmailOptions = app.Configuration
                    .GetSection(DevelopmentEmailOptions.SectionName)
                    .Get<DevelopmentEmailOptions>() ?? new DevelopmentEmailOptions();
                var emailOptions = app.Configuration
                    .GetSection(EmailOptions.SectionName)
                    .Get<EmailOptions>() ?? new EmailOptions();

                var emailMode = DevelopmentSmtpAuthEmailService.CanUse(developmentEmailOptions)
                    ? "SMTP"
                    : SendGridAuthEmailService.CanUse(emailOptions)
                        ? "SendGrid"
                        : "Console fallback";

                if (DevelopmentSmtpAuthEmailService.IsSandboxInterceptionHost(developmentEmailOptions.Host))
                {
                    Log.Warning(
                        "Development SMTP host is a sandbox interception host and will not be used for real recipient delivery.");
                }

                Log.Information("Development email delivery: {EmailMode}", emailMode);
            }

            // Global exception handler (IExceptionHandler)
            app.UseExceptionHandler();
            if (app.Environment.IsDevelopment())
                app.UseMiddleware<RequestResponseLoggingMiddleware>();

            // TLS terminates at the deployment proxy; the container listens on HTTP.
            if (app.Environment.IsDevelopment())
                app.UseHttpsRedirection();
            // UseCors must run before authentication and authorization.
            app.UseCors("Frontend");

            app.UseAuthentication();
            app.UseAuthorization();
            if (app.Environment.IsProduction())
                app.UseRateLimiter();

            app.MapControllers();
            app.MapGet("/health", async (Market.Infrastructure.Database.DatabaseContext db, CancellationToken ct) =>
            {
                using var timeout = CancellationTokenSource.CreateLinkedTokenSource(ct);
                timeout.CancelAfter(TimeSpan.FromSeconds(5));
                try
                {
                    var connectionString = new Npgsql.NpgsqlConnectionStringBuilder(db.Database.GetConnectionString())
                    {
                        Timeout = 3, CommandTimeout = 3, Pooling = false
                    };
                    await using var connection = new Npgsql.NpgsqlConnection(connectionString.ConnectionString);
                    await connection.OpenAsync(timeout.Token);
                    await using var command = connection.CreateCommand();
                    command.CommandText = "SELECT 1";
                    command.CommandTimeout = 3;
                    await command.ExecuteScalarAsync(timeout.Token);
                    return Results.Ok(new { status = "healthy" });
                }
                catch
                {
                    return Results.StatusCode(503);
                }
            }).AllowAnonymous();

            // Database migrations + seeding
            await app.Services.InitializeDatabaseAsync(app.Environment, app.Configuration);

            if (initializeDatabase)
                return;

            Log.Information("Market API started successfully.");
            app.Run();
        }
        catch (HostAbortedException)
        {
            // EF Core tools abort the host after resolving DbContext.
            // This is not a runtime failure, so exit quietly.
            Log.Information("Host aborted by EF Core tooling (design-time) - its ok.");
        }
        catch (Exception ex)
        {
            // Any startup failure will be logged here
            // Exception messages can contain provider configuration or personal data.
            Log.Fatal("Market API terminated unexpectedly ({ExceptionType}).", ex.GetType().Name);
            Environment.ExitCode = 1;
        }
        finally
        {
            // Ensure all logs are flushed before the app exits
            Log.CloseAndFlush();
        }
    }
}
