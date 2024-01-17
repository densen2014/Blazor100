// ********************************** 
// Densen Informatica 中讯科技 
// 作者：Alex Chow
// e-mail:zhouchuanglin@gmail.com 
// **********************************

using Microsoft.AspNetCore.Components;

namespace BlazorLifecycle.Pages;

public class SwitchBar2 : SwitchBar
{
    [Parameter]
    public int Value2 { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        ToolBar = RenderComponents();
    }

    private static RenderFragment RenderComponents() => builder =>
    {
        builder.OpenComponent<ToolBarBase>(0);
        builder.AddAttribute(1, nameof(ToolBarBase.Title), "工程文件操作");
        builder.CloseComponent();
    };
}
