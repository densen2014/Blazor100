using BootstrapBlazor.Components;
using FreeSql.DataAnnotations;
using System.ComponentModel;

namespace b05tree.Data;

[AutoGenerateClass(Searchable = true, Filterable = true, Sortable = true)] //BB自动建列特性,可搜索,可过滤,可排序
public class WeatherForecast
{
    [Column(IsIdentity = true)] //FreeSql ORM特性: 自增,第一个名称ID的自动为主键
    [DisplayName("序号")] //BB使用DisplayName渲染列名
    public int ID { get; set; }

    [DisplayName("日期")]
    public DateTime Date { get; set; }

    public int TemperatureC { get; set; }

    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

    public string? Summary { get; set; }
}