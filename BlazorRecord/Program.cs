using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using BlazorRecord.Data;
using Microsoft.AspNetCore.Http.Features;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
//JavaScript 互操作调用的大小限制 64 KB [64 * 1024]
builder.Services.AddServerSideBlazor().AddHubOptions(o => 
{ 
    o.EnableDetailedErrors = true; 
    o.MaximumReceiveMessageSize = long.MaxValue;
});
//设置文件上传的大小限制 , 默认值128MB 
builder.Services.Configure<FormOptions>(o => o.MultipartBodyLengthLimit = long.MaxValue);
builder.Services.AddSingleton<WeatherForecastService>();
builder.Services.AddSignalR(o =>
{
    o.EnableDetailedErrors = true;
    o.MaximumReceiveMessageSize = long.MaxValue;
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

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
