using BootstrapBlazor.Components;
using FreeSql.DataAnnotations;
using Magicodes.ExporterAndImporter.Excel;
using OfficeOpenXml.Table;
using System.ComponentModel;

namespace b03sqlite.Data;

[ExcelImporter(IsLabelingError = true)]
[ExcelExporter(Name = "导入商品中间表", TableStyle = TableStyles.Light10, AutoFitAllColumn = true)]
[AutoGenerateClass(Searchable = true, Filterable = true, Sortable = true)]
public class WeatherForecast: WeatherForecast1
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    [AutoGenerateColumn(Visible =false )]
    public int TemperatureC { get; set; }
}
public class WeatherForecast1: WeatherForecast0
{
    [AutoGenerateColumn(Visible =false )]
    public string? Summary { get; set; }
}
public class WeatherForecast0
{
    [Column(IsIdentity = true)]
    [DisplayName("序号")]
    public int ID { get; set; }

    [DisplayName("日期")]
    public DateTime Date { get; set; }



}
