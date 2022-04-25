using BootstrapBlazorApp.Shared.Data;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.AddFreeSql(option =>
{
    //demo演示的是Sqlite驱动,FreeSql支持多种数据库，MySql/SqlServer/PostgreSQL/Oracle/Sqlite/Firebird/达梦/神通/人大金仓/翰高/华为GaussDB/MsAccess
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
builder.Services.AddBootstrapBlazor();

builder.Services.AddSingleton<WeatherForecastService>();

// 增加 Table 数据服务操作类
//builder.Services.AddTableDemoDataService();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();

app.UseRouting();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
