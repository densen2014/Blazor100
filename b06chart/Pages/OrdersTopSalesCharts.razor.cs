using Blazor100.Data;
using BootstrapBlazor.Components;
using Microsoft.AspNetCore.Components;

namespace b06chart
{
    public partial class OrdersTopSalesCharts
    {
        [Inject] IFreeSql? fsql { get; set; }
        [Inject] ToastService? toastService { get; set; }
        List<OrderDetails> orders { get; set; } = new List<OrderDetails>();

        ChartsBase charts;
        decimal Total { get; set; }
        string TotalString2 { get; set; }
        private Task 数据生成(ChartDataSource ds)
        {
            //var 起始日期  = (new DateTime(charts.Year, charts.Month,1)).FirstDay(); 

            //var 结束日期 = 起始日期.LastDay();

            //orders = reportService.销售排行榜(起始日期, 结束日期, "", 取记录数:30);

            //结束日期 = 结束日期.Date.AddDays(1).AddSeconds(-1);
            //var selector = fsql.Select<OrderDetailsTopSaleDto>()
            //    .Where(a => a.OrdersLites.Status == "已结账")
            //    .WhereIf(!全部日期, a => a.OrdersLites.OrderDate.Between(起始日期, 结束日期))
            //    .WhereIf(!string.IsNullOrEmpty(搜索) && 精确, a => a.BarCode == 搜索 || a.UserCode == 搜索)
            //    .WhereIf(!string.IsNullOrEmpty(搜索) && !精确, a => a.BarCode.Contains(搜索) || a.UserCode.Contains(搜索) || a.ProductName.Contains(搜索))
            //    .Include(a => a.ProductsLites.Suppliers)
            //    .GroupBy(a => new { a.BarCode, a.UserCode, a.ProductName, a.ProductsLites })
            //    .OrderByDescending(a => a.Sum(a.Value.Quantity));
            //if (取记录数 != null) selector = selector.Take(取记录数.Value);

            //var selectDto = selector.ToList(a => new ProductsStock销售排行榜
            //{
            //    SupplierName = a.Key.ProductsLites == null ? "" : a.Key.ProductsLites.Suppliers == null ? "" : $"[{a.Key.ProductsLites.Suppliers.SupplierID}]{a.Key.ProductsLites.Suppliers.CompanyName}",
            //    BarCode = a.Value.BarCode,
            //    UserCode = a.Value.UserCode,
            //    ProductName = a.Value.ProductName,
            //    Quantity = a.Sum(a.Value.Quantity)
            //}
            //    );


            //ds.Labels = orders.Select(a => $"{a.ProductName}");

            //ds.Data.Add(new ChartDataset()
            //{
            //    Label = $"销售量",
            //    Data = orders.Select(a => a.Quantity).Cast<object>()
            //});

            //Total = orders.Sum(a => a.Quantity);
 
            return Task.CompletedTask;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                Orders.DemoDatas(fsql);
            }
        }

    }
}
