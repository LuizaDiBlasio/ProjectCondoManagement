using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProjectCondoManagement.Data;
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

//builder.Services.AddIdentity<User, IdentityRole>()
//    .AddEntityFrameworkStores<DataContextUsers>()
//    .AddDefaultTokenProviders();

builder.Services.AddIdentity<User, IdentityRole>(cfg =>
{
    // Configurações de senha e usuário do seu projeto antigo
    cfg.Tokens.AuthenticatorTokenProvider = TokenOptions.DefaultAuthenticatorProvider;
    cfg.SignIn.RequireConfirmedEmail = true;
    cfg.User.RequireUniqueEmail = true;
    cfg.Password.RequireDigit = false;
    cfg.Password.RequiredUniqueChars = 0;
    cfg.Password.RequireLowercase = false;
    cfg.Password.RequireUppercase = false;
    cfg.Password.RequireNonAlphanumeric = false;
    cfg.Password.RequiredLength = 6;
}).AddEntityFrameworkStores<DataContextUsers>() // Usar o DataContextUsers para o Identity
  .AddDefaultTokenProviders();


builder.Services.AddScoped<IUserHelper, UserHelper>();

builder.Services.AddTransient<SeedDb>();

builder.Services.AddScoped<IMailHelper, MailHelper>();

builder.Services.AddScoped<ICondoMemberRepository, CondoMemberRepository>();

builder.Services.AddScoped<IConverterHelper, ConverterHelper>();

builder.Services.AddScoped<IMailHelper, MailHelper>();

var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var seeder = services.GetRequiredService<SeedDb>();
    await seeder.SeedAsync(); 
}

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.Run();
