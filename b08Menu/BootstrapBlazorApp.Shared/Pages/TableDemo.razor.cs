using BootstrapBlazor.Components;
using BootstrapBlazorApp.Shared.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace BootstrapBlazorApp.Shared.Pages;

/// <summary>
/// 
/// </summary>
public partial class TableDemo : ComponentBase
{
    [Inject]
    [NotNull]
    private IStringLocalizer<Foo>? Localizer { get; set; }

    private readonly ConcurrentDictionary<Foo, IEnumerable<SelectedItem>> _cache = new();

    private IEnumerable<SelectedItem> GetHobbys(Foo item) => _cache.GetOrAdd(item, f => Foo.GenerateHobbys(Localizer));

    /// <summary>
    /// 
    /// </summary>
    private static IEnumerable<int> PageItemsSource => new int[] { 20, 40 };
}
