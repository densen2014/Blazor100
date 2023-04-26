using BootstrapBlazor.Components;
using Microsoft.AspNetCore.Components.Routing;

namespace b18QuickStartv757.Shared;

public partial class NavMenu
{
    private IEnumerable<MenuItem> Menus { get; set; } = new List<MenuItem>
    {
            new MenuItem() { Text = "首页", Url = "/"  , Match = NavLinkMatch.All},
            new MenuItem() { Text = "Counter", Url = "/counter" },
            new MenuItem() { Text = "Fetch data", Url = "/fetchdata" },
            new MenuItem() { Text = "工具" ,Items= new List<MenuItem>
                {
                    new MenuItem() { Text = "Counter", Url = "/counter" },
                    new MenuItem() { Text = "Fetch data", Url = "/fetchdata" },
               }
            },
    };
}
