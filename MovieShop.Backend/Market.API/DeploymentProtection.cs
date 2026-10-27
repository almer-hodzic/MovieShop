using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

namespace Market.API;

public static class DeploymentProtection
{
    public static IServiceCollection AddDeploymentProtection(this IServiceCollection services,
        IConfiguration configuration, IHostEnvironment environment)
    {
        var origins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
            ?? (environment.IsDevelopment() ? ["http://localhost:4200"] : []);
        if (environment.IsProduction() && (origins.Length == 0 || origins.Any(origin =>
            !Uri.TryCreate(origin, UriKind.Absolute, out var uri) || uri.Scheme != "https" ||
            uri.GetLeftPart(UriPartial.Authority) != origin || origin.Contains('*'))))
            throw new InvalidOperationException("Configure exact HTTPS frontend origins in Cors:AllowedOrigins.");

        services.AddCors(options => options.AddPolicy("Frontend", policy =>
        {
            if (origins.Length > 0)
                policy.WithOrigins(origins).AllowAnyHeader().AllowAnyMethod().AllowCredentials();
        }));
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            options.OnRejected = async (context, ct) =>
            {
                context.HttpContext.Response.Headers.RetryAfter = "60";
                await context.HttpContext.Response.WriteAsJsonAsync(
                    new { code = "rate_limit", message = "Too many requests. Please try again later." }, ct);
            };
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
            {
                var auth = context.Request.Path.StartsWithSegments("/api/auth");
                var payment = context.Request.Path.StartsWithSegments("/PayPal") &&
                    HttpMethods.IsPost(context.Request.Method);
                if (!auth && !payment)
                    return RateLimitPartition.GetNoLimiter("unlimited");
                var identity = payment && context.User.Identity?.IsAuthenticated == true
                    ? context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                    : context.Connection.RemoteIpAddress?.ToString();
                return RateLimitPartition.GetFixedWindowLimiter($"{(payment ? "payment" : "auth")}:{identity}",
                    _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = payment ? 10 : 30,
                        Window = TimeSpan.FromMinutes(1),
                        QueueLimit = 0,
                        AutoReplenishment = true
                    });
            });
        });
        return services;
    }
}
