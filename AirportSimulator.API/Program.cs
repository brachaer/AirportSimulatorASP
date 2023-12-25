using AirportSimulator.API.Data;
using AirportSimulator.API.Data.Repositories;
using AirportSimulator.API.Hubs;
using AirportSimulator.API.Logic;
using AirportSimulator.API.Logic.Interfaces;
using AirportSimulator.API.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AirportDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("AirportDb")));
builder.Services.AddScoped<IRepository<Station>, StationRepository>();
builder.Services.AddScoped<IRepository<Plane>, PlaneRepository>();
builder.Services.AddScoped<ILogic<Plane>, PlaneLogic>();
builder.Services.AddScoped<ILogic<Station>, StationLogic>();
builder.Services.AddScoped<IAirportLogic, AirportLogic>();
builder.Services.AddScoped<ITimeLogic, TimeLogic>();
builder.Services.AddSignalR();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(p => p.AddPolicy("allow", builder =>
{
    builder.WithOrigins("https://localhost:7287").AllowAnyHeader().AllowAnyMethod().AllowCredentials();
}));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("allow");
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<AirportHub>("/airporthub");
    endpoints.MapDefaultControllerRoute();
});
app.MapControllers();
app.Run();