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
using ProjectCondoManagement.Data.Repositories.Finances.Interfaces;
using ProjectCondoManagement.Data.Repositories.Finances;
using ProjectCondoManagement.Helpers;
using System.Text;
using ProjectCondoManagement.Data.Repositories.Users;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.

builder.Services.AddControllers();

// Configura��o para CondoDbContext
builder.Services.AddDbContext<DataContextCondos>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("CondominiumConnection")));

// Configura��o para FinanceDbContext
builder.Services.AddDbContext<DataContextFinances>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("FinanceConnection")));

// Configura��o para UserDbContext
builder.Services.AddDbContext<DataContextUsers>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("UserConnection")));


// Configurar o ASP.NET Core Identity
builder.Services.AddIdentity<User, IdentityRole>(cfg =>
{
    // Configura��es de senha e usu�rio do seu projeto antigo
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

//Autentica��o Jwt
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

builder.Services.AddScoped<IExpenseRepository, ExpenseRepository>();

builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();

builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();    

builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();    

builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();

builder.Services.AddScoped<ICondoMemberRepository, CondoMemberRepository>();

builder.Services.AddScoped<ICondominiumRepository, CondominiumRepository>();    

builder.Services.AddScoped<ICompanyRepository,  CompanyRepository>();   

builder.Services.AddScoped<IFinancialAccountRepository, FinancialAccountReposirory>();

builder.Services.AddScoped<IMailHelper, MailHelper>();

builder.Services.AddScoped<ICondominiumHelper, CondominiumHelper>();

builder.Services.AddScoped<IMessageRepository, MessageRepository>();

builder.Services.AddScoped<ICondoMemberRepository, CondoMemberRepository>();

builder.Services.AddScoped<ICondominiumRepository, CondominiumRepository>();

builder.Services.AddScoped<IUnitRepository, UnitRepository>();


builder.Services.AddHttpClient();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("JwtApiAthentication", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
    });
});

// Configura��es do Swagger/OpenAPI para testar a API (opcional, mas �til)
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

// Configura��o do pipeline de requisi��es 
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

