using CondoManagementWebApp.Helpers;
using Vereyon.Web;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Authentication.Cookies;
using Syncfusion.Licensing;
using Microsoft.AspNetCore.Server.Kestrel.Core;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddFlashMessage();

builder.Services.AddHttpClient();

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<IConverterHelper, ConverterHelper>();

builder.Services.AddScoped<IApiCallService, ApiCallService>();

builder.Services.AddScoped<IPaymentHelper, PaymentHelper>();

builder.Services.AddScoped<CloudinaryService>();

SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1JEaF5cXmRCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdmWXded3VWR2VcVEZxWUZWYEk=");

builder.Services.Configure<CloudinarySettings>(
    builder.Configuration.GetSection("CloudinarySettings"));


builder.Services.AddSingleton(x => {
    var config = builder.Configuration.GetSection("CloudinarySettings").Get<CloudinarySettings>();
    var account = new Account(config.CloudName, config.ApiKey, config.ApiSecret);
    return new Cloudinary(account);
});

//configurar sessão 
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(120);
    options.Cookie.HttpOnly = true; 
    options.Cookie.IsEssential = true; // Indica que o cookie de sessão é essencial para o funcionamento do site
});

//nomear a sessão para que ela seja unica
var cookieName = $"CondoAuth_Session_{Guid.NewGuid().ToString("N")}";

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/NotAuthorized";

        //atribuir o nome, assim a cada sessão o nome terá que ser unico e os cookies nunca serão guardados
        options.Cookie.Name = cookieName;

        options.Events = new CookieAuthenticationEvents
        {
            OnSigningIn = context =>
            {
                context.Properties.IsPersistent = false;
                return Task.CompletedTask;
            }
        };
    });

builder.WebHost.ConfigureKestrel(options =>
{
    // Aumenta o limite de tamanho do cabeçalho para todos os endpoints
    options.Limits.MaxRequestHeadersTotalSize = 32768;
});

builder.Services.AddScoped<IApiCallService, ApiCallService>();


SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1JEaF5cXmRCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdmWXded3VWR2VcVEZxWUZWYEk=");


builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
