using CondoManagementWebApp.Helpers;
using Vereyon.Web;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddFlashMessage();

builder.Services.AddHttpClient();

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<IConverterHelper, ConverterHelper>();

builder.Services.AddScoped<IApiCallService, ApiCallService>();

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


builder.Services.AddScoped<CloudinaryService>();

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
