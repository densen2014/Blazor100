using BootstrapBlazor.Components;
using Microsoft.Extensions.Localization;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace b18ValidateForm.Pages;

/// <summary>
/// 
/// </summary>
public sealed partial class Index
{
    [NotNull]
    private Foo? Model { get; set; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    protected override void OnInitialized()
    {
        Model = new Foo()
        {
            Name = "",
            Count = 1,
            Address = "TestAddress",
            DateTime = new DateTime(1997, 12, 05),
            Education = EnumEducation.Middle
        };
    }

}

/// <summary>
/// Demo示例数据
/// Demo sample data
/// </summary>
public class Foo
{
    // 列头信息支持 Display DisplayName 两种标签

    /// <summary>
    ///
    /// </summary>
    [Display(Name = "主键")]
    [AutoGenerateColumn(Ignore = true)]
    public int Id { get; set; }

    /// <summary>
    ///
    /// </summary>
    [Required(ErrorMessage = "{0}不能为空")]
    [AutoGenerateColumn(Order = 10, Filterable = true, Searchable = true)]
    [Display(Name = "姓名")]
    public string? Name { get; set; }

    /// <summary>
    ///
    /// </summary>
    [AutoGenerateColumn(Order = 1, FormatString = "yyyy-MM-dd", Width = 180)]
    [Display(Name = "日期")]
    public DateTime? DateTime { get; set; }

    /// <summary>
    ///
    /// </summary>
    [Display(Name = "地址")]
    [Required(ErrorMessage = "{0}不能为空")]
    [AutoGenerateColumn(Order = 20, Filterable = true, Searchable = true)]
    public string? Address { get; set; }

    /// <summary>
    ///
    /// </summary>
    [Display(Name = "数量")]
    [Required]
    [AutoGenerateColumn(Order = 40, Sortable = true)]
    public int Count { get; set; }

    /// <summary>
    ///
    /// </summary>
    [Display(Name = "是/否")]
    [AutoGenerateColumn(Order = 50)]
    public bool Complete { get; set; }

    /// <summary>
    ///
    /// </summary>
    [Required(ErrorMessage = "请选择学历")]
    [Display(Name = "学历")]
    [AutoGenerateColumn(Order = 60)]
    public EnumEducation? Education { get; set; }

    /// <summary>
    ///
    /// </summary>
    [Required(ErrorMessage = "请选择一种{0}")]
    [Display(Name = "爱好")]
    [AutoGenerateColumn(Order = 70, Editable = false)]
    public IEnumerable<string> Hobby { get; set; } = new List<string>();

    #region Static methods
    /// <summary>
    /// 
    /// </summary>
    protected static readonly Random Random = new();

    /// <summary>
    /// 生成Foo类,随机数据
    /// Generate Foo class, random data
    /// </summary>
    /// <param name="localizer"></param>
    /// <returns></returns>
    public static Foo Generate(IStringLocalizer<Foo> localizer) => new()
    {
        Id = 1,
        Name = localizer["Foo.Name", "1000"],
        DateTime = System.DateTime.Now,
        Address = localizer["Foo.Address", $"{Random.Next(1000, 2000)}"],
        Count = Random.Next(1, 100),
        Complete = Random.Next(1, 100) > 50,
        Education = Random.Next(1, 100) > 50 ? EnumEducation.Primary : EnumEducation.Middle
    };

    /// <summary>
    /// 生成 Foo 类,随机数据
    /// Generate Foo class, random data
    /// </summary>
    /// <returns>返回一个Foo类的List，Return a List of Foo class</returns>
    public static List<Foo> GenerateFoo(int count = 80) => Enumerable.Range(1, count).Select(i => new Foo()
    {
        Id = i,
        Name = "Foo.Name"+ $"{i:d4}",
        DateTime = System.DateTime.Now.AddDays(i - 1),
        Address = "Foo.Address"+$"{Random.Next(1000, 2000)}",
        Count = Random.Next(1, 100),
        Complete = Random.Next(1, 100) > 50,
        Education = Random.Next(1, 100) > 50 ? EnumEducation.Primary : EnumEducation.Middle
    }).ToList();

  
    /// <summary>
    /// 通过 Count 获得颜色
    /// </summary>
    /// <param name="count"></param>
    /// <returns></returns>
    public static Color GetProgressColor(int count) => count switch
    {
        >= 0 and < 10 => Color.Secondary,
        >= 10 and < 20 => Color.Danger,
        >= 20 and < 40 => Color.Warning,
        >= 40 and < 50 => Color.Info,
        >= 50 and < 70 => Color.Primary,
        _ => Color.Success
    };

    /// <summary>
    /// 通过 Id 获取 Title
    /// </summary>
    /// <returns></returns>
    private static string GetTitle() => Random.Next(1, 80) switch
    {
        >= 1 and < 10 => "Clerk",
        >= 10 and < 50 => "Engineer",
        >= 50 and < 60 => "Manager",
        >= 60 and < 70 => "Chief",
        _ => "General Manager"
    };

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static string GetTitle(int id) => Cache.GetOrAdd(id, key => GetTitle());

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public static Func<IEnumerable<Foo>, string, SortOrder, IEnumerable<Foo>> GetNameSortFunc() => Utility.GetSortFunc<Foo>();

    private static ConcurrentDictionary<int, string> Cache { get; } = new();
    #endregion
}

/// <summary>
///
/// </summary>
public enum EnumEducation
{
    /// <summary>
    ///
    /// </summary>
    [Display(Name = "小学")]
    Primary,

    /// <summary>
    ///
    /// </summary>
    [Display(Name = "中学")]
    Middle
}
