// ********************************** 
// Densen Informatica 中讯科技 
// 作者：Alex Chow
// e-mail:zhouchuanglin@gmail.com 
// **********************************

using b16blazorIDS2.Areas.Identity;
using b16blazorIDS2.Data;
using b16blazorIDS2.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

//EF SqlServer 配置

// Add services to the container.
//var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

//builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

//EF Sqlite 配置
builder.Services.AddDbContext<ApplicationDbContext>(o => o.UseSqlite(builder.Configuration.GetConnectionString("IdsSQliteConnection")));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

//注入Identity依赖   WebAppIdentityUser => WebAppIdentityUser
builder.Services.AddDefaultIdentity<WebAppIdentityUser>(o =>
 {   // Password settings.
     o.Password.RequireDigit = false;
     o.Password.RequireLowercase = false;
     o.Password.RequireNonAlphanumeric = false;
     o.Password.RequireUppercase = false;
     o.Password.RequiredLength = 4;
     o.Password.RequiredUniqueChars = 1;
 }
)
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<WebAppIdentityUser>>();
builder.Services.AddSingleton<WeatherForecastService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
