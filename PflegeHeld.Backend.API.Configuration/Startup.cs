namespace PflegeHeld.Backend.API.Configuration;

public class Startup(string name)
{
    private readonly string _name = name;
    private string accessKey = string.Empty;
    public int APIVersion { get; } = 1;

    public WebApplicationBuilder Build(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.AddServiceDefaults();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();


        builder.Services.AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService($"API.Service.{_name}"))
            .WithTracing(tracing =>
            {
                tracing
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddEntityFrameworkCoreInstrumentation();
            });


        var sqlUrl = Environment.GetEnvironmentVariable("SQL_URL") ?? "localhost";
        var sqlAdminUsername = Environment.GetEnvironmentVariable("SQL_ADMIN_USERNAME") ?? "sa";
        var sqlAdminPassword = Environment.GetEnvironmentVariable("SQL_ADMIN_PASSWORD") ?? "P@ssword1";
        accessKey = Environment.GetEnvironmentVariable("X-Access-Key") ?? "";

        //builder.Services.AddDbContext<GDCDbContext>(options => { options.UseSqlServer($"Server={sqlUrl};Database=GDC;User ID={sqlAdminUsername};Password={sqlAdminPassword};TrustServerCertificate=True"); });
        builder.Services.AddHealthChecks();
        builder.Services.AddResponseCaching();

        builder.Host.UseSerilog((context, configuration) =>
            configuration
                .ReadFrom.Configuration(context.Configuration)
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Backend", $"API.Service.{_name}")
                .MinimumLevel.Information()
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"));

        builder.Services.AddApiVersioning(o =>
        {
            o.DefaultApiVersion = new ApiVersion(APIVersion);
            o.ApiVersionReader = new UrlSegmentApiVersionReader();
        })
        .AddApiExplorer(o =>
        {
            o.GroupNameFormat = "'v'V";
            o.SubstituteApiVersionInUrl = true;
        });

        return builder;
    }

    public WebApplication Create(WebApplicationBuilder build)
    {
        var app = build.Build();
        app.MapDefaultEndpoints();

        if(app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseResponseCaching();

        app.Use(async (context, next) =>
        {
            if(app.Environment.IsDevelopment())
            {
                await next();
                return;
            }

            if (context.Request.Headers.TryGetValue("X-Access-Key", out var value) && value.Equals(accessKey))
            {
                context.Request.Headers["X-Access-Key"] = "-";
                context.Response.GetTypedHeaders().CacheControl = new CacheControlHeaderValue
                {
                    Public = true,
                    MaxAge = TimeSpan.FromSeconds(30)
                };

                context.Response.Headers[HeaderNames.Vary] = "User-Agent";

                await next();
                return;
            }

            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsync("Access denied.");
            return;
        });

        app.MapGet("", () =>
        {
            return $"API: {_name}";
        });

        return app;
    }

}
