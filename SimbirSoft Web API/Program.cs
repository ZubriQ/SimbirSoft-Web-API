#region Usings

using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using Npgsql;
using SimbirSoft_Web_API.Authentication;
using SimbirSoft_Web_API.Services.Accounts;
using SimbirSoft_Web_API.Services.Animals;
using SimbirSoft_Web_API.Services.AnimalsKinds;
using SimbirSoft_Web_API.Services.AreaAnalytics;
using SimbirSoft_Web_API.Services.Areas;
using SimbirSoft_Web_API.Services.Kinds;
using SimbirSoft_Web_API.Services.Locations;
using SimbirSoft_Web_API.Services.Registration;
using SimbirSoft_Web_API.Services.VisitedLocations;

#endregion

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

#region Configurate SwaggerGen and Basic Authentication

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "SimbirSoft - Animal Chipization Web API", Version = "v1" });
    options.AddSecurityDefinition("Basic", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Scheme = "Basic",
        Type = SecuritySchemeType.Http,
        In = ParameterLocation.Header
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Basic" }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services
    .AddAuthentication(Constants.BasicAuthScheme)
    .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>(Constants.BasicAuthScheme, null);

#endregion

#region Configurate Database

builder.Services
    .AddHealthChecks()
    .AddCheck("PostgreSQL Check", () =>
    {
        try
        {
            var connectionString = builder.Configuration.GetConnectionString("PostgreSQL");
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
            }
            return HealthCheckResult.Healthy("PostgreSQL is healthy.");
        }
        catch (Exception)
        {
            return HealthCheckResult.Unhealthy("PostgreSQL is unhealthy.");
        }
    });

string connection = builder.Configuration.GetConnectionString("PostgreSQL");
builder.Services.AddNpgsql<ChipizationDbContext>(connection);

#endregion

#region Configurate AutoMapper, Services and 8080 Port

builder.Services.AddAutoMapper(typeof(Program).Assembly);

builder.Services.AddScoped<IApiAuthenticationService, ApiAuthenticationService>();
builder.Services.AddScoped<IRegistrationService, RegistrationService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<ILocationService, LocationService>();
builder.Services.AddScoped<IKindService, KindService>();
builder.Services.AddScoped<IAnimalService, AnimalService>();
builder.Services.AddScoped<IAnimalKindService, AnimalKindService>();
builder.Services.AddScoped<IVisitedLocationService, VisitedLocationService>();
builder.Services.AddScoped<IAreaService, AreaService>();
builder.Services.AddScoped<IAreaAnalyticsService, AreaAnalyticsService>();

builder.WebHost.UseUrls("http://*:8080");

#endregion

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

#region Migrate

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ChipizationDbContext>();
    if (context.Database.IsNpgsql() && context.Database.GetPendingMigrations().Any())
    {
        context.Database.EnsureCreated();
    }
}

#endregion

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/health");

app.Run();
