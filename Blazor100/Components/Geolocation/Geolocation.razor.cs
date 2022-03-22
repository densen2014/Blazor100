// ********************************** 
// Densen Informatica 中讯科技 
// 作者：Alex Chow
// e-mail:zhouchuanglin@gmail.com 
// **********************************

using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Diagnostics.CodeAnalysis;

namespace Blazor100.Components;

/// <summary>
/// Geolocation 组件基类
/// <para></para>
/// 扩展阅读:Chrome中模拟定位信息，清除定位信息<para></para>
/// https://blog.csdn.net/u010844189/article/details/81163438
/// </summary>
public partial class Geolocation
{
    [Inject] IJSRuntime? JS { get; set; }

    /// <summary>
    /// 获得/设置 定位
    /// </summary>
    [Parameter]
    [NotNull]
    public string? GeolocationInfo { get; set; }

    /// <summary>
    /// 获得/设置 获取位置按钮文字 默认为 获取位置
    /// </summary>
    [Parameter]
    [NotNull]
    public string? GetLocationButtonText { get; set; } = "获取位置";

    /// <summary>
    /// 获得/设置 获取持续定位监听器ID
    /// </summary>
    [Parameter]
    public long? WatchID { get; set; }

    /// <summary>
    /// 获得/设置 获取移动距离追踪按钮文字 默认为 移动距离追踪
    /// </summary>
    [Parameter]
    [NotNull]
    public string? WatchPositionButtonText { get; set; } = "移动距离追踪";

    /// <summary>
    /// 获得/设置 获取停止追踪按钮文字 默认为 停止追踪
    /// </summary>
    [Parameter]
    [NotNull]
    public string? ClearWatchPositionButtonText { get; set; } = "停止追踪";

    /// <summary>
    /// 获得/设置 是否显示默认按钮界面
    /// </summary>
    [Parameter]
    public bool ShowButtons { get; set; } = true;

    /// <summary>
    /// 
    /// </summary>
    protected ElementReference GeolocationElement { get; set; }

    /// <summary>
    /// 获得/设置 定位结果回调方法
    /// </summary>
    [Parameter]
    public Func<Geolocationitem, Task>? OnResult { get; set; }

    /// <summary>
    /// 获得/设置 状态更新回调方法
    /// </summary>
    [Parameter]
    public Func<string, Task>? OnUpdateStatus { get; set; }

    private IJSObjectReference? module;
    private DotNetObjectReference<Geolocation>? InstanceGeo { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        try
        {
            if (firstRender)
            {
                module = await JS!.InvokeAsync<IJSObjectReference>("import", "./lib/geolocation/geolocation.js");
                InstanceGeo = DotNetObjectReference.Create(this);
            }
        }
        catch (Exception e)
        {
            if (OnError != null) await OnError.Invoke(e.Message);
        }
    }

    async ValueTask IAsyncDisposable.DisposeAsync()
    {
        if (module is not null)
        {
            //await module.InvokeVoidAsync("destroy");
            InstanceGeo!.Dispose();
            await module.DisposeAsync();
        }
    }


    /// <summary>
    /// 获取定位
    /// </summary>
    public virtual async Task GetLocation()
    {
        try
        {
            await module!.InvokeVoidAsync("getLocation", InstanceGeo);
        }
        catch (Exception e)
        {
            if (OnError != null) await OnError.Invoke(e.Message);
        }
    }

    /// <summary>
    /// 持续定位
    /// </summary>
    public virtual async Task WatchPosition()
    {
        try
        {
            await module!.InvokeVoidAsync("getLocation", InstanceGeo, false);
        }
        catch (Exception e)
        {
            if (OnError != null) await OnError.Invoke(e.Message);
        }
    }

    /// <summary>
    /// 持续定位
    /// </summary>
    public virtual async Task ClearWatch()
    {
        await module!.InvokeVoidAsync("clearWatchLocation", InstanceGeo, WatchID);
        WatchID = null;
    }

    /// <summary>
    /// 定位完成回调方法
    /// </summary>
    /// <param name="geolocations"></param>
    /// <returns></returns>
    [JSInvokable]
    public async Task GetResult(Geolocationitem geolocations)
    {
        try
        {
            if (OnResult != null) await OnResult.Invoke(geolocations);
        }
        catch (Exception e)
        {
            if (OnError != null) await OnError.Invoke(e.Message);
        }
    }

    /// <summary>
    /// 获得/设置 错误回调方法
    /// </summary>
    [Parameter]
    public Func<string, Task>? OnError { get; set; }

    /// <summary>
    /// 状态更新回调方法
    /// </summary>
    /// <param name="status"></param>
    /// <returns></returns>
    [JSInvokable]
    public async Task UpdateStatus(string status)
    {
        if (OnUpdateStatus != null) await OnUpdateStatus.Invoke(status);
    }

    /// <summary>
    /// 监听器ID回调方法
    /// </summary>
    /// <param name="watchID"></param>
    /// <returns></returns>
    [JSInvokable]
    public Task UpdateWatchID(long watchID)
    {
        this.WatchID = watchID;
        return Task.CompletedTask;
    }

}
