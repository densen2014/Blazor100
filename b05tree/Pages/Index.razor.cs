using BootstrapBlazor.Components;

namespace b05tree.Pages;

/// <summary>
/// 
/// </summary>
public sealed partial class Index
{

    private static List<TreeItem> GetLazyItems()
    {
        var ret = TreeDataFoo.GetTreeItems();
        ret[0].Items[0].Items[0].Text += "_懒加载延时";
        ret[0].Items[0].Items[0].HasChildNode = true;
        ret[0].Items[0].Items[0].Key = "Delay";

        ret[0].Items[0].Items[1].Text += "_懒加载";
        ret[0].Items[0].Items[1].HasChildNode = true;

        ret[0].Items[0].Items[2].Text += "_默认打开";
        ret[0].Items[0].Items[2].IsCollapsed = false;

        for (int i = 0; i < ret[0].Items[0].Items[0].Items.Count; i++)
        {
            ret[0].Items[0].Items[0].Items[i].Checked = true;
            ret[0].Items[0].Items[1].Items[i].Checked = true;
            ret[0].Items[0].Items[2].Items[i].Checked = true;
        }
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
                    new TreeItem() { Text = "懒加载子节点2" }
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
