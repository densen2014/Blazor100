// ********************************** 
// Densen Informatica 中讯科技 
// 作者：Alex Chow
// e-mail:zhouchuanglin@gmail.com 
// **********************************

using b16blazorIDS2.Areas.Identity;
using b16blazorIDS2.Data;
using b16blazorIDS2.Models;
using Blazor100.Service;
using Densen.DataAcces.FreeSql;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using System.IO.Compression;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddResponseCompression(options =>
{
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
    options.MimeTypes =
    ResponseCompressionDefaults.MimeTypes.Concat(
                    new[] { "image/svg+xml" });
});
builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.SmallestSize;
});

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
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<WebAppIdentityUser>>();
builder.Services.AddSingleton<WeatherForecastService>();
builder.Services.AddFreeSql(option =>
{
    option.UseConnectionString(FreeSql.DataType.Sqlite, "Data Source=ids.db;")  //也可以写到配置文件中
#if DEBUG
         //开发环境:自动同步实体
         .UseAutoSyncStructure(true)
         .UseNoneCommandParameter(true)
    //调试sql语句输出
         .UseMonitorCommand(cmd => System.Console.WriteLine(cmd.CommandText + Environment.NewLine))
#endif
    ;
});
builder.Services.AddSingleton(typeof(FreeSqlDataService<>));

builder.Services.AddTransient<ImportExportsService>();
builder.Services.AddDensenExtensions();
builder.Services.ConfigureJsonLocalizationOptions(op =>
{
    // 忽略文化信息丢失日志
    op.IgnoreLocalizerMissing = true;
});

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
if (Directory.Exists(Path.Combine(builder.Environment.WebRootPath, "uploads")) == false) Directory.CreateDirectory(Path.Combine(builder.Environment.WebRootPath, "uploads"));
IFileProvider fileProvider = new PhysicalFileProvider(Path.Combine(builder.Environment.WebRootPath, "uploads"));
app.UseStaticFiles(new StaticFileOptions()
{
    FileProvider = fileProvider,
    RequestPath = new PathString("/uploads"),
    OnPrepareResponse = ctx =>
    {
        const int durationInSeconds = 60 * 5;//5分钟 
        ctx.Context.Response.Headers[Microsoft.Net.Http.Headers.HeaderNames.CacheControl] =
           "public,max-age=" + durationInSeconds;
    }
});

app.UseDirectoryBrowser(new DirectoryBrowserOptions()
{
    FileProvider = fileProvider,
    RequestPath = new PathString("/uploads")
});

app.UseRouting();

app.UseAuthorization();

app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
