using BootstrapBlazor.Components;
using DocumentFormat.OpenXml.Wordprocessing;
using FreeSql.DataAnnotations;
using Magicodes.ExporterAndImporter.Excel;
using OfficeOpenXml.Table;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace b14table.Data;

[ExcelImporter(IsLabelingError = true)]
[ExcelExporter(Name = "导入商品中间表", TableStyle = TableStyles.Light10, AutoFitAllColumn = true)]
[AutoGenerateClass(Searchable = true, Filterable = true, Sortable = true, ShowTips = true)]

public class SalesChannels
{
    [AutoGenerateColumn(Ignore = true)]
    [Column(IsIdentity = true)]
    [DisplayName("序号")]
    public int ID { get; set; }

    [AutoGenerateColumn(ComponentType = typeof(ColorPicker), Width = 30)]
    [DisplayName("级别")]
    public string? Background { get; set; }

    [AutoGenerateColumn(FormatString = "yyyy-MM-dd")]
    [DisplayName("日期")]
    public DateTime Date { get; set; }

    [Required(ErrorMessage = "{0}不能为空")]
    [DisplayName("名称")]
    public string? Name { get; set; }

    [DisplayName("项目数量")]
    public int Projects { get; set; }

    [DisplayName("交单数量")]
    public int Orders { get; set; }

    [DisplayName("结单数量")]
    public int Checkouts { get; set; }

    // 编辑界面无法显示小数, 以后再思考
    [DisplayName("结单率")]
    [AutoGenerateColumn(Readonly = true)]
    public string? CheckoutRates { get => GetCheckoutRates(Checkouts, Orders); set => checkoutRates = value; }
    string? checkoutRates;


    [DisplayName("合格数量")]
    public int Qualifieds { get; set; }

    [DisplayName("合格率")]
    [AutoGenerateColumn(Readonly = true)]
    public string? QualifiedRates { get => GetQualifiedRates(Qualifieds, Checkouts); set => qualifiedRates = value; }
    string? qualifiedRates;

    [DisplayName("总价值")]
    public int Total { get; set; }

    [DisplayName("应收款")]
    public int Receivables { get; set; }

    [DisplayName("已收款")]
    public int Received { get; set; }

    [AutoGenerateColumn(FormatString = "HH:mm:ss")]
    [DisplayName("修改日期")]
    public DateTime ModifiedDate { get; set; } = DateTime.Now;

    [AutoGenerateColumn(TextEllipsis = true, Visible = false, ShowTips = true, ComponentType = typeof(Textarea))]
    [DisplayName("备注")]
    public string? Remark { get; set; }

    [AutoGenerateColumn(Visible = false, ComponentType = typeof(BootstrapInput<decimal>), Width = 80)]
    [DisplayName("Test1")]
    public decimal Test1 { get; set; }

    private string GetCheckoutRates(int checkouts, int orders) => orders > 0 ? (checkouts /(double) orders).ToString("P2") : "0%";

    private string GetQualifiedRates(int qualifieds, int checkouts) => checkouts > 0 ? (qualifieds / (double)checkouts).ToString("P2") : "0%";


}
