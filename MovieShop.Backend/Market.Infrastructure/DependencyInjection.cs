using Market.Application.Abstractions;
using Market.Infrastructure.Common;
using Market.Infrastructure.Database;
using Market.Shared.Constants;
using Market.Shared.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Market.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment env)
    {
        // Typed ConnectionStrings + validation
        services.AddOptions<ConnectionStringsOptions>()
            .Bind(configuration.GetSection(ConnectionStringsOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<PayPalOptions>()
            .Bind(configuration.GetSection(PayPalOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<EmailOptions>()
            .Bind(configuration.GetSection(EmailOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.Configure<DevelopmentEmailOptions>(
            configuration.GetSection(DevelopmentEmailOptions.SectionName));

        // DbContext: InMemory for test environments; SQL Server otherwise
        services.AddDbContext<DatabaseContext>((sp, options) =>
        {
            if (env.IsTest())
            {
                options.UseInMemoryDatabase("IntegrationTestsDb");

                return;
            }

            var cs = sp.GetRequiredService<IOptions<ConnectionStringsOptions>>().Value.Main;
            options.UseSqlServer(cs);
        });

        // IAppDbContext mapping
        services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<DatabaseContext>());

        // Identity hasher
        services.AddScoped<IPasswordHasher<MarketUserEntity>, PasswordHasher<MarketUserEntity>>();

        // Token service (reads JwtOptions via IOptions<JwtOptions>)
        services.AddTransient<IJwtTokenService, JwtTokenService>();
        services.AddMemoryCache();
        services.AddSingleton<IPayPalCheckoutSessionStore, MemoryPayPalCheckoutSessionStore>();
        services.AddHttpClient<IPayPalService, PayPalService>();
        services.AddHttpClient<SendGridAuthEmailService>(client =>
        {
            client.BaseAddress = new Uri("https://api.sendgrid.com/");
        });

        if (env.IsDevelopment())
        {
            services.AddScoped<IAuthEmailService>(sp =>
            {
                var developmentEmailOptions = sp.GetRequiredService<IOptions<DevelopmentEmailOptions>>().Value;
                var emailOptions = sp.GetRequiredService<IOptions<EmailOptions>>().Value;
                var logger = sp.GetRequiredService<ILoggerFactory>()
                    .CreateLogger(nameof(DependencyInjection));

                if (DevelopmentSmtpAuthEmailService.IsSandboxInterceptionHost(developmentEmailOptions.Host))
                {
                    logger.LogWarning(
                        "Development SMTP host is a sandbox interception host and will not be used for real recipient delivery.");
                }

                if (DevelopmentSmtpAuthEmailService.CanUse(developmentEmailOptions))
                {
                    logger.LogInformation("Development email delivery: SMTP");
                    return ActivatorUtilities.CreateInstance<DevelopmentSmtpAuthEmailService>(sp);
                }

                if (SendGridAuthEmailService.CanUse(emailOptions))
                {
                    logger.LogInformation("Development email delivery: SendGrid");
                    return sp.GetRequiredService<SendGridAuthEmailService>();
                }

                logger.LogInformation("Development email delivery: Console fallback");
                return ActivatorUtilities.CreateInstance<DevelopmentAuthEmailService>(sp);
            });
        }
        else
        {
            services.AddHttpClient<IAuthEmailService, SendGridAuthEmailService>(client =>
            {
                client.BaseAddress = new Uri("https://api.sendgrid.com/");
            });
        }

        // HttpContext accessor + current user
        services.AddHttpContextAccessor();
        services.AddScoped<IAppCurrentUser, AppCurrentUser>();

        // TimeProvider (if used in handlers/services)
        services.AddSingleton<TimeProvider>(TimeProvider.System);

        return services;
    }
}
