using BootstrapBlazor.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;

namespace b07geo.Pages;

public partial class Index : IAsyncDisposable
{
    private JSInterop<Index>? Interop { get; set; }
    private string Trace;

    [Inject]
    private IJSRuntime? JSRuntime { get; set; }

    private GeolocationItem? Model { get; set; }

    /// <summary>
    /// 获得/设置 获取持续定位监听器ID
    /// </summary>
    private long WatchID { get; set; }

    private async Task GetLocation()
    {
        Interop ??= new JSInterop<Index>(JSRuntime);
        var ret = await Geolocation.GetLocaltion(Interop, this, nameof(GetLocationCallback));
        Trace += (ret ? "成功获取定位" : "获取定位失败");
    }
    private async Task WatchPosition()
    {
        try
        {
            Interop ??= new JSInterop<Index>(JSRuntime);
            WatchID = await Geolocation.WatchPosition(Interop, this, nameof(GetLocationCallback));
            Trace += WatchID != 0 ? "调用 WatchPosition 成功" : "调用 WatchPosition 失败";
            Trace += $"WatchID : {WatchID}";
        }
        catch (Exception)
        {
            Trace += "调用 WatchPosition 失败";
        }
    }

    private async Task ClearWatchPosition()
    {
        if (WatchID != 0)
        {
            Interop ??= new JSInterop<Index>(JSRuntime);
            var ret = await Geolocation.ClearWatchPosition(Interop, WatchID);
            if (ret)
            {
                WatchID = 0;
            }
            Trace += ret ? "停止追踪成功" : "停止追踪失败";
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="item"></param>
    [JSInvokable]
    public void GetLocationCallback(GeolocationItem item)
    {
        Model = item;
        StateHasChanged();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="disposing"></param>
    protected virtual async ValueTask DisposeAsync(bool disposing)
    {
        if (disposing)
        {
            if (Interop != null)
            {
                if (WatchID != 0)
                {
                    await Geolocation.ClearWatchPosition(Interop, WatchID);
                }

                Interop.Dispose();
                Interop = null;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        await DisposeAsync(true);
        GC.SuppressFinalize(this);
    }
}
