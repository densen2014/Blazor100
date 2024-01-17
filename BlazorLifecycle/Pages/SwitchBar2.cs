using Microsoft.AspNetCore.Components;



namespace BlazorLifecycle.Pages;


public class SwitchBar2 : SwitchBar  
{
    [Parameter]
    public int Value2 { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        ToolBar= RenderComponents();
    }

    private static RenderFragment RenderComponents() => builder =>
    {
        builder.OpenComponent<ToolBarBase>(0);
        builder.CloseComponent(); 
    };
}
