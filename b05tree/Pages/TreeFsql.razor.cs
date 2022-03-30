using BootstrapBlazor.Components;
using Microsoft.AspNetCore.Components;

namespace b05tree.Pages;

/// <summary>
/// 
/// </summary>
public sealed partial class TreeFsql
{
    [Inject] IFreeSql? fsql { get; set; } 


    private List<TreeItem> GetLazyItems()
    {


        var ret = TreeDataFsqlFoo.GetTreeItems(fsql);

        ret[2].Text += "_懒加载";
        ret[2].HasChildNode = true;

        ret[3].Text += "_懒加载延时";
        ret[3].HasChildNode = true;
        ret[3].Key = "Delay";

        return ret;
    }



    private Task OnTreeItemClick(TreeItem item)
    {
        //Trace.Log($"TreeItem: {item.Text} clicked");
        return Task.CompletedTask;
    }

    private Task OnTreeItemChecked(TreeItem item)
    {
        var state = item.Checked ? "选中" : "未选中";
        //TraceChecked.Log($"TreeItem: {item.Text} {state}");
        return Task.CompletedTask;
    }

    private static async Task OnExpandNode(TreeItem item)
    {
        if (!item.Items.Any() && item.HasChildNode && !item.ShowLoading)
        {
            item.ShowLoading = true;
            if (item.Key?.ToString() == "Delay")
            {
                await Task.Delay(800);
            }
            item.Items.AddRange(new TreeItem[]
            {
                    new TreeItem()
                    {
                        Text = "懒加载子节点1",
                        HasChildNode = true
                    },
                    new TreeItem()
                    {
                        Text = "懒加载延时子节点2",
                        HasChildNode = true,
                        Key = "Delay"
                    },
                    new TreeItem() { Text = "懒加载子节点3" }
            });
            item.ShowLoading = false;
        }
    }

    private Task OnTreeItemChecked(List<TreeItem> items)
    {
        //TraceCheckedItems.Log($"当前共选中{items.Count}项");
        return Task.CompletedTask;
    }

}
