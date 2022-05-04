using BootstrapBlazor.Components;
using BootstrapBlazorApp.Shared.Data;
using Microsoft.AspNetCore.Components;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace BootstrapBlazorApp.Shared.Pages;

/// <summary>
/// 
/// </summary>
public partial class Htmls
{
    [Parameter]
    public string? PageName { get; set; }

    [Parameter]
    public string? Language { get; set; }

    [Inject]
    [NotNull]
    private ToastService? ToastService { get; set; }

    [Inject]
    [NotNull]
    IFreeSql? fsql { get; set; }

    [NotNull]
    WebPages? TpvPage { get; set; }

    string? pageName;

    protected override void OnInitialized()
    {
        Language = CultureInfo.CurrentUICulture.Name;
    }

    protected override void OnAfterRender(bool firstRender)
    {
        PageName ??= "Index";
        if (firstRender || pageName != PageName)
        {
            TpvPage = fsql.Select<WebPages>().Where(a => a.Url == PageName).First();
            pageName = PageName;
            StateHasChanged();
        }
    }

}
