using Olymp_Project.Services.Accounts;
using Olymp_Project.Services.Animals;
using Olymp_Project.Services.Kinds;
using Olymp_Project.Services.Locations;
using Olymp_Project.Services.VisitedLocations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connection = builder.Configuration.GetConnectionString("animal-chipization");
builder.Services.AddSqlServer<ChipizationDbContext>(connection);
builder.Services.AddAutoMapper(typeof(Program).Assembly);
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<ILocationService, LocationService>();
builder.Services.AddScoped<IKindService, KindService>();
builder.Services.AddScoped<IAnimalService, AnimalService>();
builder.Services.AddScoped<IVisitedLocationService, VisitedLocationService>();

var app = builder.Build();

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
