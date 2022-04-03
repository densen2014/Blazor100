using BootstrapBlazor.Components;
using FreeSql.DataAnnotations;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Linq;

namespace Blazor100.Data;


public partial class Orders
{
    /// <summary>
    /// 流水号
    /// </summary>

    [AutoGenerateColumn(Editable = false, DefaultSort = true, DefaultSortOrder = SortOrder.Desc, Order = 1)]
    [JsonProperty, Column(IsIdentity = true)]
    [DisplayName("流水号")]
    public int OrderID { get; set; }

    /// <summary>
    /// 单据日期
    /// </summary>
    [AutoGenerateColumn(FormatString = "yyyy-MM-dd", ComponentType = typeof(DatePickerBody))]
    [JsonProperty]
    [DisplayName("日期")]
    public DateTime OrderDate { get; set; }

    /// <summary>
    /// 合计金额
    /// </summary>
    [AutoGenerateColumn(FormatString = "N2", Align = Alignment.Right)]
    [JsonProperty, Column(DbType = "decimal(19,4)")]
    [DisplayName("合计")]
    public decimal SubTotal { get; set; }

    [AutoGenerateColumn(Ignore = true)]
    [Navigate(nameof(OrderID))]
    public virtual List<OrderDetails>? OrderDetailss { get; set; }


    public static void DemoDatas(IFreeSql fsql)
    {
        var Randomer = new Random();
        var res = fsql!.Select<Orders>().Count();
        if (res == 0)
        {
            var items = new List<Orders>();
            for (int i = 1; i < 100; i++)
            {
                var demo = Enumerable.Range(1, Randomer.Next(5, 12)).Select((j, index) =>
                      new Orders
                      {
                          //OrderID = i,
                          OrderDate = DateTime.Now.Date.AddDays(-(100 - i)),
                          SubTotal = Randomer.Next(3, 45), 
                          OrderDetailss = Enumerable.Range(1, Randomer.Next(3, 10)).Select((j, index) => 
                                              new OrderDetails
                                              {
                                                  OrderID = i,
                                                  BarCode = Randomer.Next(100000, 9000000).ToString(),
                                                  Quantity = Randomer.Next(10, 30)
                                              }).ToList()
                      });  
                items.AddRange(demo.ToList());
            } 

        var repo = fsql.GetRepository<Orders>();//仓库类
        repo.DbContextOptions.EnableAddOrUpdateNavigateList = true; //开启一对多，多对多级联保存功能 
        repo.Insert(items);
    }

}

}
/// <summary>
/// 订单详单
/// </summary>
public partial class OrderDetails
{

    [JsonProperty, Column(IsIdentity = true)]
    public int ID { get; set; }

    [JsonProperty]
    public int OrderID { get; set; }

    [JsonProperty, Column(StringLength = -1)]
    [DisplayName("条码")]
    public string? BarCode { get; set; }

    [AutoGenerateColumn(FormatString = "N0", Align = Alignment.Center)]
    [JsonProperty, Column(DbType = "numeric(18,3)")]
    [DisplayName("数量")]
    public decimal Quantity { get; set; }

    [AutoGenerateColumn(Ignore = true)]
    [Navigate(nameof(OrderID))]
    public virtual Orders Orders { get; set; }

}
