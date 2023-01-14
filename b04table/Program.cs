// ********************************** 
// Densen Informatica 中讯科技 
// 作者：Alex Chow
// e-mail:zhouchuanglin@gmail.com 
// **********************************

using b14table.Data;
using Blazor100.Service;
using Densen.DataAcces.FreeSql;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<WeatherForecastService>();
builder.Services.AddFreeSql(option =>
{
    option.UseConnectionString(FreeSql.DataType.Sqlite, "Data Source=test.db;")  //也可以写到配置文件中
#if DEBUG
         //开发环境:自动同步实体
         .UseAutoSyncStructure(true)
         .UseNoneCommandParameter(true)
    //调试sql语句输出
         .UseMonitorCommand(cmd => System.Console.WriteLine(cmd.CommandText))
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
if (!app.Environment.IsDevelopment())
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

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
