using BootstrapBlazor.Components;

namespace b05tree;
public static class Utility
{
    /// <summary>
    /// 树状组件数据层次化方法
    /// </summary>
    /// <param name="items">数据集合</param>
    /// <param name="parentId">父级节点</param>
    public static IEnumerable<TreeItem> CascadingTree(this IEnumerable<TreeItem> items, string? parentId = null) => items.Where(i => i.ParentId == parentId).Select(i =>
    {
        i.Items = CascadingTree(items, i.Id).ToList();
        return i;
    });
}
class TreeDataFoo
{
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public static List<TreeItem> GetTreeItems0()
    {
        var items = new List<TreeItem>
        {
            new TreeItem() { Text = "001_系统管理", Id = "001" },
            new TreeItem() { Text = "001_01_基础数据管理", Id = "001_01", ParentId = "001" },
            new TreeItem() { Text = "001_01_01_教师", Id = "001_01_01", ParentId = "001_01" },
            new TreeItem() { Text = "001_01_02_职工", Id = "001_01_02", ParentId = "001_01" },

            new TreeItem() { Text = "001_02_餐厅数据管理", Id = "001_02", ParentId = "001" },
            new TreeItem() { Text = "001_02_01_厨师", Id = "001_02_01", ParentId = "001_02" },
            new TreeItem() { Text = "001_02_02_服务员", Id = "001_02_02", ParentId = "001_02" },

        }; 
        // 算法获取属性结构数据
        return items.CascadingTree().ToList();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public static List<TreeItem> GetTreeItems(int count=30)
    {
        var items = new List<TreeItem>
        {
            new TreeItem() { Text = "系统管理", Id = "1010" },
            new TreeItem() { Text = "基础数据管理", Id = "1040", ParentId = "1010" },
            new TreeItem() { Text = "基础管理", Id = "1070", ParentId = "1040" },
            new TreeItem() { Text = "基础2管理", Id = "1080", ParentId = "1040" },
            new TreeItem() { Text = "基础3管理", Id = "1090", ParentId = "1040" },
            new TreeItem() { Text = "基础4管理", Id = "1100", ParentId = "1040" },

            new TreeItem() { Text = "系统管理2", Id = "1011" },
            new TreeItem() { Text = "基础数据管理2", Id = "1210", ParentId = "1011" },


            new TreeItem() { Text = "系统管理3", Id = "1012" }, //_懒加载演示

            new TreeItem() { Text = "系统管理4", Id = "1014" },//_懒加载延时演示 

        };

        var items1000 = new List<TreeItem>();
        for (int i = 0; i < count; i++)
        {
            items1000.Add(new TreeItem() { Text = $"教师{-i}信息", Id = $"112{i}0", ParentId = "1070", IsActive = true });
        }
        for (int i = 0; i < count; i++)
        {
            items1000.Add(new TreeItem() { Text = $"教师基础2-{i}信息", Id = $"113{i}0", ParentId = "1080", IsActive = true });
        }
        for (int i = 0; i < count; i++)
        {
            items1000.Add(new TreeItem() { Text = $"教师基础3-{i}信息", Id = $"114{i}0", ParentId = "1090", IsActive = true });
        }
        for (int i = 0; i < count; i++)
        {
            items1000.Add(new TreeItem() { Text = $"教师基础4-{i}信息", Id = $"115{i}0", ParentId = "1100", IsActive = true });
        }

        for (int i = 0; i < count; i++)
        {
            items1000.Add(new TreeItem() { Text = $"教师II{-i}信息", Id = $"116{i}0", ParentId = "1210", IsActive = true });
        }

        items.AddRange(items1000);
        // 算法获取属性结构数据
        return items.CascadingTree().ToList();
    }
}
