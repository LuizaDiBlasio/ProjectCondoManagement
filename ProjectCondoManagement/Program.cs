using Microsoft.EntityFrameworkCore;
using ProjectCondoManagement.Data.Entites.CondosDb;
using ProjectCondoManagement.Data.Entites.FinancesDb;
using ProjectCondoManagement.Data.Entites.UsersDb;
using ProjectCondoManagement.Data.Repositories.Condos;
using ProjectCondoManagement.Data.Repositories.Condos.Interfaces;
using ProjectCondoManagement.Helpers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DataContextCondos>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("CondominiumConnection")));

// Configuração para FinanceDbContext
builder.Services.AddDbContext<DataContextFinances>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("FinanceConnection")));

// Configuração para UserDbContext
builder.Services.AddDbContext<DataContextUsers>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("UserConnection")));

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddScoped<ICondoMemberRepository, CondoMemberRepository>();

builder.Services.AddScoped<IConverterHelper, ConverterHelper>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.Run();
