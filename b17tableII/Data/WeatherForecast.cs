using BootstrapBlazor.Components;
using DocumentFormat.OpenXml.Office2010.Excel;
using FreeSql.DataAnnotations;
using Magicodes.ExporterAndImporter.Excel;
using OfficeOpenXml.Table;
using System.ComponentModel;

namespace b17tableII.Data;

[ExcelImporter(IsLabelingError = true)]
[ExcelExporter(Name = "导入商品中间表", TableStyle = TableStyles.Light10, AutoFitAllColumn = true)]
[AutoGenerateClass(Searchable = true, Filterable = true, Sortable = true)]
public class WeatherForecast
{
    [Column(IsPrimary = true)]
    [DisplayName("序号")]
    public int ID { get; set; }

    [DisplayName("日期")]
    public DateTime Date { get; set; }

    public int TemperatureC { get; set; }

    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

    public string? Summary { get; set; }

    [DisplayName("备注")]
    [Column(IsIgnore = true)]
    public string Remark1
    {
        get => remark1 ?? (Ext?.Remark ?? "");
        set
        {
            remark1 = value;
            Ext = Ext ?? new Remarks() { ID = ID };
            Ext!.Remark = value;
        }
    }
    string? remark1;

    [DisplayName("扩展备注")]
    [Navigate (nameof(Remarks.ID))]
    public virtual  Remarks? Ext { get; set; }
 
}
