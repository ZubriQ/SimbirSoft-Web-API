using Microsoft.AspNetCore.Authentication;
using Microsoft.OpenApi.Models;
using Olymp_Project.Authentication;
using Olymp_Project.Services.Accounts;
using Olymp_Project.Services.Animals;
using Olymp_Project.Services.AnimalsKinds;
using Olymp_Project.Services.Kinds;
using Olymp_Project.Services.Locations;
using Olymp_Project.Services.Registration;
using Olymp_Project.Services.VisitedLocations;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

#region Configurating SwaggerGen and Basic authentication

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "webapi", Version = "v1" });
    options.AddSecurityDefinition("Basic", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Basic",
        In = ParameterLocation.Header,
        Description = "Basic Authorization header using the Bearer scheme."
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

builder.Services.AddAuthentication(ApiAuthenticationScheme.Name)
    .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>(
    ApiAuthenticationScheme.Name, null);

#endregion

#region Configurating Database, AutoMapper, Services and WebHost port

string connection;
#if DEBUG
connection = builder.Configuration.GetConnectionString("Development");
#else
connection = builder.Configuration.GetConnectionString("Testing");
#endif

builder.Services.AddSqlServer<ChipizationDbContext>(connection);

builder.Services.AddAutoMapper(typeof(Program).Assembly);

builder.Services.AddScoped<IApiAuthenticationService, ApiAuthenticationService>();
builder.Services.AddScoped<IRegistrationService, RegistrationService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<ILocationService, LocationService>();
builder.Services.AddScoped<IKindService, KindService>();
builder.Services.AddScoped<IAnimalService, AnimalService>();
builder.Services.AddScoped<IAnimalKindService, AnimalKindService>();
builder.Services.AddScoped<IVisitedLocationService, VisitedLocationService>();

#if !DEBUG
builder.WebHost.UseUrls("http://0.0.0.0:8080");
#endif

#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    // Create a database for testing.
    using (var scope = app.Services.CreateScope())
    {
        using (var context = scope.ServiceProvider.GetRequiredService<ChipizationDbContext>())
        {
            // TODO: Add the deletion?
            //context.Database.EnsureDeleted();
            context.Database.Migrate();
        }
    }
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
