// Copyright (c) Argo Zhang (argo@163.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://www.blazor.zone or https://argozhang.github.io/

using BootstrapBlazor.Components;
using Densen.Models.ids;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// 演示网站示例数据注入服务实现类
/// </summary>
internal class IdsLookupService : ILookupService
{
    private IServiceProvider Provider { get; }
    private IFreeSql fsql { get; set; }

    public IdsLookupService(IServiceProvider provider, IFreeSql fsql)
    {
        Provider = provider;
        this.fsql = fsql;
    }

    public IEnumerable<SelectedItem>? GetItemsByKey(string? key)
    {
        IEnumerable<SelectedItem>? items = null;
        if (key == "Provideres")
        {
            items = new List<SelectedItem>()
            {
                new() { Value = "True", Text = "真真" },
                new() { Value = "False", Text = "假假" },
                new() { Value = "Google", Text = "谷歌" },
                new() { Value = "Google2", Text = "谷歌" },
                new() { Value = "Google3", Text = "谷歌" },
                new() { Value = "Google4", Text = "谷歌" },
                new() { Value = "Google5", Text = "谷歌" },
                new() { Value = "Google6", Text = "谷歌" },
                new() { Value = "Google7", Text = "谷歌" },
                new() { Value = "Google8", Text = "谷歌" },
                new() { Value = "Google9", Text = "谷歌" },
                new() { Value = "Googlea", Text = "谷歌" },
                new() { Value = "Googleb", Text = "谷歌" },
                new() { Value = "Googlec", Text = "谷歌" },
                new() { Value = "Googled", Text = "谷歌" },
                new() { Value = "Googlee", Text = "谷歌" },
                new() { Value = "Googlef", Text = "谷歌" },
                new() { Value = "Googlei", Text = "谷歌" },
                new() { Value = "Googlej", Text = "谷歌" },
                new() { Value = "Googlek", Text = "谷歌" },
            };
        }
        else if (key == nameof(AspNetUserRoles.UserId))
        {
            items = fsql.Select<AspNetUsers>().ToList().Select(a => new SelectedItem() { Value = a.Id ?? "", Text = a.UserName ?? "" }).ToList();
        }
        else if (key == nameof(AspNetUserRoles.RoleId))
        {
            items = fsql.Select<AspNetRoles>().ToList().Select(a => new SelectedItem() { Value = a.Id ?? "", Text = a.Name ?? "" }).ToList();
        }
        return items;
    }

    public IEnumerable<SelectedItem>? GetItemsByKey(string? key, object? data)
    {
        return GetItemsByKey(key);
    }
}
