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

#region Configurating Database, AutoMapper and Services

var connection = builder.Configuration.GetConnectionString("animal-chipization");
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

#endregion

var app = builder.Build();

// Creating the db for testing.
using (var scope = app.Services.CreateScope())
{
    using (var context = scope.ServiceProvider.GetRequiredService<ChipizationDbContext>())
    {
        context.Database.Migrate();
    }
}

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
