using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using MamboBank.Gateway.WebApi.Data;
using StateManagement.Database.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

// Add Entity Framework DbContext
// In Aspire, connection string is automatically provided through database reference
builder.Services.AddDbContext<MamboBankDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MamboBankDb")));

builder.Services.AddOrchestration(builder.Configuration.GetConnectionString("MamboBankDb")!);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapDefaultEndpoints();

app.MapOpenApi();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();