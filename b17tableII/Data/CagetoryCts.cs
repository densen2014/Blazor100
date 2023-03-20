using BootstrapBlazor.Components;
using FreeSql.DataAnnotations;
using Magicodes.ExporterAndImporter.Excel;
using OfficeOpenXml.Table;
using System.ComponentModel;

namespace b17tableII.Data;

[AutoGenerateClass(Searchable = true, Filterable = true, Sortable = true)]
public class Remarks
{
    [DisplayName("序号")]
    public int ID { get; set; }

    public string? Remark { get; set; }
}

/// <summary>
/// OneToMany 一对多,CTS
/// </summary>
/// <param name="fsql"></param>
class Cagetory
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public Guid ParentId { get; set; }

    [Column(IsIgnore = true)]
    public string SubName
    {
        get => subName ?? (Childs?.FirstOrDefault()?.Name ?? "");
        set
        {
            subName = value;
            if (Childs != null && Childs?.FirstOrDefault() != null && Childs.Any())
            {
                Childs!.FirstOrDefault()!.Name = value;
            }
        }
    }
    string? subName;

    [Navigate(nameof(ParentId))]
    public List<Cagetory>? Childs { get; set; }


    public static void GenDemoDatas(IFreeSql fsql)
    {
        var repo = fsql.GetRepository<Cagetory>();
        if (repo.Select.Any()) return;
        repo.DbContextOptions.EnableCascadeSave = true;
        var cts = new[]
        {
            new Cagetory
            {
                Name = "分类1",
                Childs = new List<Cagetory>(new[]
                        {
                            new Cagetory { Name = "分类1_1" },
                            new Cagetory { Name = "分类1_2" },
                            new Cagetory { Name = "分类1_3" }
                        })
                    },
                    new Cagetory
                    {
                        Name = "分类2",
                        Childs = new List<Cagetory>(new[]
                        {
                            new Cagetory { Name = "分类2_1" },
                            new Cagetory { Name = "分类2_2" }
                        })
                    }
                };
        repo.Insert(cts);


    }
}


/// <summary>
/// OneToMany 子表
/// </summary>
/// <param name="fsql"></param>
class Cagetory2
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public Guid ParentId { get; set; }

    [Column(IsIgnore = true)]
    public string SubName
    {
        get => subName ?? (Childs?.FirstOrDefault()?.Name ?? "");
        set
        {
            subName = value;
            if (Childs != null && Childs?.FirstOrDefault() != null && Childs.Any())
            {
                Childs!.FirstOrDefault()!.Name = value;
            }
        }
    }
    string? subName;

    [Navigate(nameof(ParentId))]
    public List<SubCagetory>? Childs { get; set; }

    public static void GenDemoDatas(IFreeSql fsql)
    {
        var repo = fsql.GetRepository<Cagetory2>();
        if (repo.Select.Any()) return;
        repo.DbContextOptions.EnableCascadeSave = true;
        var cts = new[]
        {
            new Cagetory2
            {
                Name = "分类1",
                Childs = new List<SubCagetory>(new[]
            {
                            new SubCagetory { Name = "分类1_1" },
                            new SubCagetory { Name = "分类1_2" },
                            new SubCagetory { Name = "分类1_3" }
                        })
                    },
                    new Cagetory2
                    {
                        Name = "分类2",
                        Childs = new List<SubCagetory>(new[]
            {
                            new SubCagetory { Name = "分类2_1" },
                            new SubCagetory { Name = "分类2_2" }
                        })
                    }
                };
        repo.Insert(cts);


    }
}

class SubCagetory
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public Guid ParentId { get; set; }

}


/// <summary>
/// OneToOne, 两边都用 pk 作为关联，才是绝对的1v1, 否则是 ManyToOne
/// </summary>
class Cagetory3
{

    public Guid Id { get; set; }

    public string? Name { get; set; }

    [Column(IsIgnore = true)]
    public string SubName
    {
        get => subName ?? (Ext?.Remark ?? "");
        set
        {
            subName = value;
            Ext = Ext ?? new SubCagetory3PK() { Id = Id };
            Ext!.Remark = value;
        }
    }
    string? subName;

    /// <summary>
    /// 垂直分表,扩展字段
    /// </summary>
    [Navigate(nameof(SubCagetory3PK.Id))]
    public SubCagetory3PK? Ext { get; set; }


    public static void GenDemoDatas(IFreeSql fsql)
    {
        var repo = fsql.GetRepository<Cagetory3>();
        if (repo.Select.Any()) return;
        repo.DbContextOptions.EnableCascadeSave = true;

        //OneToOne 关键点
        var uid = Guid.NewGuid();
        var uid2 = Guid.NewGuid();
        var cts = new[]
        {
            new Cagetory3
            {
                Id=uid, //OneToOne 关键点
                Name = "oto分类1",
                Ext = new SubCagetory3PK {Id=uid, Remark = "扩展备注1" }
            } ,
            new Cagetory3
            {
                Id=uid2, //OneToOne 关键点
                Name = "oto分类2",
                Ext = new SubCagetory3PK {Id=uid2, Remark = "扩展备注2" }
            } ,                  
        };
        repo.Insert(cts);


    }
}

class SubCagetory3PK
{
    [Column(IsPrimary = true)]
    public Guid Id { get; set; }

    public virtual Cagetory3? Cagetory { get; set; }

    public string? Remark { get; set; }

}
