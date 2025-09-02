using CloudinaryDotNet;
using CondoManagementWebApp.Helpers;
using Microsoft.AspNetCore.Authentication.Cookies;
using ProjectCondoManagement.Data.Entites.CondosDb;
using Syncfusion.Licensing;
using Vereyon.Web;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddFlashMessage();

builder.Services.AddHttpClient();

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<IConverterHelper, ConverterHelper>();

builder.Services.AddScoped<IFeeHelper, FeeHelper>();

builder.Services.AddScoped<ICondominiumHelper, CondominiumHelper>();

builder.Services.AddScoped<IApiCallService, ApiCallService>();

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
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true; 
    options.Cookie.IsEssential = true; // Indica que o cookie de sessão é essencial para o funcionamento do site
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
    });




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

app.MapControllers();

app.Run();
