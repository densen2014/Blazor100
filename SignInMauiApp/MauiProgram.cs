// ********************************** 
// Densen Informatica 中讯科技 
// 作者：Alex Chow
// e-mail:zhouchuanglin@gmail.com 
// **********************************

using Microsoft.Extensions.Logging;
using FreeSql;

namespace SignInMauiApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // 注册 FreeSql ORM，使用 SQLite 数据库
        var fsql = new FreeSqlBuilder()
            .UseAutoSyncStructure(true)
            .UseConnectionString(DataType.Sqlite, "Data Source=signindb.db")
            .Build();
        builder.Services.AddSingleton<IFreeSql>(fsql);

#if DEBUG
		builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
