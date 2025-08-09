using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ProjectCondoManagement.Data;
using ProjectCondoManagement.Data.Entites.CondosDb;
using ProjectCondoManagement.Data.Entites.FinancesDb;
using ProjectCondoManagement.Data.Entites.UsersDb;
using ProjectCondoManagement.Data.Repositories.Condos;
using ProjectCondoManagement.Data.Repositories.Condos.Interfaces;
using ProjectCondoManagement.Helpers;
using System.Text;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.

builder.Services.AddControllers();

// Configuração para CondoDbContext
builder.Services.AddDbContext<DataContextCondos>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("CondominiumConnection")));

// Configuração para FinanceDbContext
builder.Services.AddDbContext<DataContextFinances>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("FinanceConnection")));

// Configuração para UserDbContext
builder.Services.AddDbContext<DataContextUsers>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("UserConnection")));


// Configurar o ASP.NET Core Identity
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

//Autenticação Jwt
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(cfg =>
{
    cfg.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = builder.Configuration["Tokens:Issuer"],
        ValidAudience = builder.Configuration["Tokens:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Tokens:Key"]))
    };
});



builder.Services.AddTransient<SeedDb>();

builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

builder.Services.AddScoped<IUserHelper, UserHelper>();

builder.Services.AddScoped<IConverterHelper, ConverterHelper>();

builder.Services.AddScoped<ISmsHelper, SmsHelper>();

builder.Services.AddScoped<ICondoMemberRepository, CondoMemberRepository>();

builder.Services.AddScoped<IMailHelper, MailHelper>();

builder.Services.AddScoped<ICondoMemberRepository, CondoMemberRepository>();

builder.Services.AddHttpClient();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("JwtApiAthentication", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
    });
});

// Configurações do Swagger/OpenAPI para testar a API (opcional, mas útil)
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

// Configuração do pipeline de requisições 
var app = builder.Build();

// Configurar o pipeline HTTP.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}


// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<SeedDb>();
    await seeder.SeedAsync();
}

app.Run();

