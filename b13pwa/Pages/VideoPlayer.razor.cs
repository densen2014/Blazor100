// ********************************** 
// Densen Informatica 中讯科技 
// 作者：Alex Chow
// e-mail:zhouchuanglin@gmail.com 
// **********************************

using b13video.Pages;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using System.Text.Json.Serialization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Blazor100.Components;

/// <summary>
/// 屏幕键盘 OnScreenKeyboard 组件基类
/// </summary>
public partial class VideoPlayer : IAsyncDisposable
{
    [Inject] IJSRuntime? JS { get; set; }
    private IJSObjectReference? module;
    private DotNetObjectReference<VideoPlayer>? instance { get; set; }
    protected ElementReference element { get; set; }
    private bool init;
    private string? info;

    private string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// 资源类型
    /// </summary>
    [Parameter]
    public string? SourcesType { get; set; }

    /// <summary>
    /// 资源地址
    /// </summary>
    [Parameter]
    public string? SourcesUrl { get; set; }

    [Parameter]
    public int Width { get; set; } = 300;

    [Parameter]
    public int Height { get; set; } = 200;

    [Parameter]
    public bool Controls { get; set; } = true;

    [Parameter]
    public bool Autoplay { get; set; } = true;

    [Parameter]
    public string Preload { get; set; } = "auto";

    /// <summary>
    /// 设置封面
    /// </summary>
    [Parameter]
    public string? Poster { get; set; }

    [Parameter]
    public VideoPlayerOption? Option { get; set; }

    [Parameter]
    public bool Debug { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        try
        {
            if (firstRender)
            {
                module = await JS!.InvokeAsync<IJSObjectReference>("import", "./app.js");
                instance = DotNetObjectReference.Create(this);

                Option = Option ?? new VideoPlayerOption()
                {
                    Width = Width,
                    Height = Height,
                    Controls = Controls,
                    Autoplay = Autoplay,
                    Preload = Preload,
                    Poster = Poster,
                    //EnableSourceset= true,
                    //TechOrder= "['fakeYoutube', 'html5']"
                };
                Option.Sources.Add(new VideoSources(SourcesType, SourcesUrl));

                try
                {
                    await module.InvokeVoidAsync("loadPlayer", instance, "video_" + Id, Option);
                }
                catch (Exception e)
                {
                    info = e.Message;
                    if (Debug) StateHasChanged();
                    Console.WriteLine(info);
                    if (OnError != null) await OnError.Invoke(info);
                }
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
            await module.InvokeVoidAsync("destroy", Id);
            await module.DisposeAsync();
        }
    }


    /// <summary>
    /// 获得/设置 错误回调方法
    /// </summary>
    [Parameter]
    public Func<string, Task>? OnError { get; set; }

    /// <summary>
    /// JS回调方法
    /// </summary>
    /// <param name="init"></param>
    /// <returns></returns>
    [JSInvokable]
    public void GetInit(bool init) => this.init = init;

    /// <summary>
    /// JS回调方法
    /// </summary>
    /// <param name="error"></param>
    /// <returns></returns>
    [JSInvokable]
    public async Task GetError(string error)
    {
        info = error;
        if (Debug) StateHasChanged();
        if (OnError != null) await OnError.Invoke(error);
    }

}
