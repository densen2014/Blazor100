﻿// ********************************** 
// Densen Informatica 中讯科技 
// 作者：Alex Chow
// e-mail:zhouchuanglin@gmail.com 
// **********************************

using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.FileProviders;
using System.Text.Encodings.Web;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls(builder.Configuration["UseUrls"]?? "http://localhost:8000;http://0.0.0.0:8000;");
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
//设置文件上传的大小限制 , 默认值128MB 
builder.Services.Configure<FormOptions>(o =>o.MultipartBodyLengthLimit = long.MaxValue);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}


app.UseStaticFiles();

var dir = Path.Combine(app.Environment.WebRootPath, "Upload");
if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

var opt = new DirectoryBrowserOptions
{
    FileProvider = new PhysicalFileProvider(dir),
    Formatter = new AME.HtmlDirectoryFormatterChsSorted(HtmlEncoder.Default),
    RequestPath = new PathString("/Upload")
};
app.UseDirectoryBrowser(opt);

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
