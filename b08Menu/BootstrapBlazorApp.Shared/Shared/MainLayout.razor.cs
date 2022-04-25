using BootstrapBlazor.Components;
using BootstrapBlazorApp.Shared.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using System.Diagnostics.CodeAnalysis;

namespace BootstrapBlazorApp.Shared.Shared;

/// <summary>
/// 
/// </summary>
public sealed partial class MainLayout
{
    private bool UseTabSet { get; set; } = true;

    private string Theme { get; set; } = "";

    private bool IsOpen { get; set; }

    private bool IsFixedHeader { get; set; } = true;

    private bool IsFixedFooter { get; set; } = true;

    private bool IsFullSide { get; set; } = true;

    private bool ShowFooter { get; set; } = true;

    private List<MenuItem>? Menus { get; set; } = new List<MenuItem>();

    [Inject]
    [NotNull]
    IFreeSql? fsql { get; set; }

    /// <summary>
    /// OnInitialized 方法
    /// </summary>
    protected override void OnInitialized()
    {
        base.OnInitialized();

        //Menus = GetIconSideMenuItems();
    }

    protected override void OnAfterRender(bool firstRender)
    {
        if (firstRender)
        {
            if (fsql.Select<WebPages>().Count() == 0)
            {
                var pages = new List<WebPages>(){
                    new WebPages("首页","/","fa fa-home","001") ,
                    new WebPages("数据","","fa fa-fw fa-database","002",
                        new List<WebPages>(new[] {
                            new WebPages("FetchData","fetchdata","fa fa-fw fa-database","002_001") ,
                            new WebPages( "Counter","counter","fa fa-fw fa-check-square-o","002_002")  ,
                            new WebPages("后台管理","admins","fa fa-gears","002_003") ,
                        })) ,
                    new WebPages("Table","table","fa fa-fw fa-table","004")  ,
                    new WebPages("花名册","users","fa fa-fw fa-users","005")
                };

                var repo = fsql.GetRepository<WebPages>();//仓库类
                repo.DbContextOptions.EnableAddOrUpdateNavigateList = true; //开启一对多，多对多级联保存功能
                repo.Insert(pages); 
            }
            Menus =  fsql.Select<WebPages>().OrderBy(a => a.Code)
                        .LeftJoin(d => d.ParentCode == d.Parent!.Code)
                        .ToList(a => new MenuItem()
                        {
                            Text = a.PageName,
                            Id = a.Code,
                            Url = a.Url,
                            ParentId = a.ParentCode,
                            Icon = a.Icon
                        }).CascadingMenu().ToList();
            // 算法获取属性结构数据 .CascadingMenu().ToList()
            StateHasChanged();
        }
    }
 
}
