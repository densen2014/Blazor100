using BootstrapBlazor.Components;

namespace b05tree;

class TreeDataFoo
{
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public static List<TreeItem> GetTreeItems()
    {
        var items = new List<TreeItem>
        {
            new TreeItem() { Text = "系统管理", Id = "1010" },
            new TreeItem() { Text = "系统管理2", Id = "1011" },
            new TreeItem() { Text = "系统管理3", Id = "1012" },
            new TreeItem() { Text = "基础数据管理", Id = "1040", ParentId = "1010" },
            new TreeItem() { Text = "基础管理", Id = "1070", ParentId = "1040" },
            new TreeItem() { Text = "基础2管理", Id = "1080", ParentId = "1040" },
            new TreeItem() { Text = "基础3管理", Id = "1090", ParentId = "1040" },
            new TreeItem() { Text = "基础4管理", Id = "1100", ParentId = "1040" },
            new TreeItem() { Text = "基础数据管理2", Id = "1210", ParentId = "1011" },
            new TreeItem() { Text = "基础数据管理2", Id = "1211", ParentId = "1012" },

        };

        var items1000 = new List<TreeItem>();
        for (int i = 0; i < 300; i++)
        {
            items1000.Add(new TreeItem() { Text = $"教师{-i}信息", Id = $"112{i}0", ParentId = "1070", IsActive = true });
        }
        for (int i = 0; i < 300; i++)
        {
            items1000.Add(new TreeItem() { Text = $"教师基础2-{i}信息", Id = $"113{i}0", ParentId = "1080", IsActive = true });
        }
        for (int i = 0; i < 300; i++)
        {
            items1000.Add(new TreeItem() { Text = $"教师基础3-{i}信息", Id = $"114{i}0", ParentId = "1090", IsActive = true });
        }
        for (int i = 0; i < 300; i++)
        {
            items1000.Add(new TreeItem() { Text = $"教师基础4-{i}信息", Id = $"115{i}0", ParentId = "1100", IsActive = true });
        }

        for (int i = 0; i < 300; i++)
        {
            items1000.Add(new TreeItem() { Text = $"教师II{-i}信息", Id = $"116{i}0", ParentId = "1210", IsActive = true });
        }

        items.AddRange(items1000);
        // 算法获取属性结构数据
        return items.CascadingTree().ToList();
    }
}
