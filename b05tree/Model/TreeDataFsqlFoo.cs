using BootstrapBlazor.Components;
using FreeSql.DataAnnotations;

namespace b05tree;

/// <summary>
/// 无限级分类（父子）是一种比较常用的表设计,表设计中只有 parent_id 字段
/// </summary>
public class TreeDataFsqlFoo
{
    //https://github.com/dotnetcore/FreeSql/wiki/%E6%9F%A5%E8%AF%A2%E7%88%B6%E5%AD%90%E5%85%B3%E7%B3%BB

    [Column(IsPrimary = true, StringLength = 6)]
    public string Code { get; set; }

    [Column(StringLength = 20, IsNullable = false)]
    public string Name { get; set; }

    [Column(StringLength = 6)]
    public string ParentCode { get; set; }

    [Navigate(nameof(ParentCode))]
    public TreeDataFsqlFoo Parent { get; set; }

    [Navigate(nameof(ParentCode))]
    public List<TreeDataFsqlFoo> Childs { get; set; }



    public static void DemoDatas(IFreeSql fsql)
    {
        var res = fsql!.Select<TreeDataFsqlFoo>().Count();
        if (res == 0)
        {
            var districts = new TreeDataFsqlFoo
            {
                Code = "001",
                Name = "001_系统管理",
                Childs = new List<TreeDataFsqlFoo>(new[] {
                    new TreeDataFsqlFoo{
                        Code = "001_01",
                        Name = "001_01_基础数据管理",
                        Childs = new List<TreeDataFsqlFoo>(new[] {
                            new TreeDataFsqlFoo{
                                Code = "001_01_01",
                                Name = "001_01_01_教师"
                            },
                            new TreeDataFsqlFoo{
                                Code = "001_01_02",
                                Name = "001_01_02_职工"
                            }
                        })
                    },
                    new TreeDataFsqlFoo{
                        Code = "001_02",
                        Name = "001_02_餐厅数据管理",
                        Childs = new List<TreeDataFsqlFoo>(new[] {
                            new TreeDataFsqlFoo{
                                Code = "001_02_01",
                                Name = "001_02_01_厨师"
                            },
                            new TreeDataFsqlFoo{
                                Code = "001_02_02",
                                Name = "001_02_02_服务员"
                            }
                        })
                    },
                    new TreeDataFsqlFoo{
                        Code = "001_03",
                        Name = "001_03_懒加载演示",
                        Childs = new List<TreeDataFsqlFoo>(new[] {
                            new TreeDataFsqlFoo{
                                Code = "001_03_01",
                                Name = "001_03_01_子节点1"
                            },
                            new TreeDataFsqlFoo{
                                Code = "001_03_02",
                                Name = "001_03_02_子节点2"
                            }
                        })
                    },
                    new TreeDataFsqlFoo{
                        Code = "001_04",
                        Name = "001_04_懒加载延时演示",
                        Childs = new List<TreeDataFsqlFoo>(new[] {
                            new TreeDataFsqlFoo{
                                Code = "001_04_01",
                                Name = "001_04_01_子节点1"
                            },
                            new TreeDataFsqlFoo{
                                Code = "001_04_02",
                                Name = "001_04_02_子节点2"
                            }
                        })
                    }
                })
            };
            var repo = fsql.GetRepository<TreeDataFsqlFoo>();//仓库类
            repo.DbContextOptions.EnableAddOrUpdateNavigateList = true; //开启一对多，多对多级联保存功能
            repo.Insert(districts);
        }

    }
    public static List<TreeDataFsqlFoo> GetTreeList(IFreeSql fsql)
    {
        DemoDatas(fsql);
        return fsql.Select<TreeDataFsqlFoo>().LeftJoin(d => d.ParentCode == d.Parent.Code).ToList();
    }

    public static List<TreeItem> GetTreeItems(IFreeSql fsql)
    {
        DemoDatas(fsql);
        var items = fsql.Select<TreeDataFsqlFoo>().LeftJoin(d => d.ParentCode == d.Parent.Code)
            .ToList(a => new TreeItem()
            {
                Text = a.Name,
                Id = a.Code,
                ParentId = a.ParentCode
            });
        // 算法获取属性结构数据
        return items.CascadingTree().ToList();
    }
}
