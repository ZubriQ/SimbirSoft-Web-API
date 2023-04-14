using Microsoft.AspNetCore.Authentication;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using Olymp_Project.Authentication;
using Olymp_Project.Services.Accounts;
using Olymp_Project.Services.Animals;
using Olymp_Project.Services.AnimalsKinds;
using Olymp_Project.Services.AreaAnalytics;
using Olymp_Project.Services.Areas;
using Olymp_Project.Services.Kinds;
using Olymp_Project.Services.Locations;
using Olymp_Project.Services.Registration;
using Olymp_Project.Services.VisitedLocations;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

#region Configurate SwaggerGen and Basic Authorization

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "webapi", Version = "v1" });
    options.AddSecurityDefinition("Basic", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Basic",
        In = ParameterLocation.Header
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Basic" }
            },
            new string[] { }
        }
    });
});

builder.Services.AddAuthentication(Constants.BasicAuthScheme)
    .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>(
    Constants.BasicAuthScheme, null);

#endregion

#region Configurate Database

//builder.Services
//    .AddHealthChecks()
//    .AddCheck("SQL Server Check", () =>
//    {
//        try
//        {
//            var connectionString = builder.Configuration.GetConnectionString("PostgreSQL");
//            using (var connection = new SqlConnection(connectionString))
//            {
//                connection.Open();
//            }
//            return HealthCheckResult.Healthy("SQL Server is healthy.");
//        }
//        catch (Exception)
//        {
//            return HealthCheckResult.Unhealthy("SQL Server is unhealthy.");
//        }
//    });

string connection = builder.Configuration.GetConnectionString("PostgreSQL");
builder.Services.AddSqlServer<ChipizationDbContext>(connection);

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

builder.WebHost.UseUrls("http://0.0.0.0:8080");

#endregion

var app = builder.Build();

//Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;

        var context = services.GetRequiredService<ChipizationDbContext>();
        if (context.Database.IsNpgsql())
        {
            if (context.Database.GetPendingMigrations().Any())
            {
                context.Database.EnsureCreated();
            }
        }
    }
}

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

//app.MapHealthChecks("/health");

app.Run();
