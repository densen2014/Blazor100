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

        ChartsBase? charts;
        decimal Total { get; set; } 
        private Task 数据生成(ChartDataSource ds)
        {
            var 起始日期 = (new DateTime(charts!.Year, charts.Month, 1)).FirstDay();

            var 结束日期 = 起始日期.LastDay();

            orders = fsql!.Select<OrderDetails>()
                .Where( a => a.Orders.OrderDate.Between(起始日期, 结束日期))
                .GroupBy(a =>  a.BarCode )
                .OrderByDescending(a => a.Sum(a.Value.Quantity))
                .ToList(a => new OrderDetails
                 {
                    BarCode = a.Key, 
                    Quantity = a.Sum(a.Value.Quantity)
                }
                );


            ds.Labels = orders.Select(a => $"{a.BarCode}");

            ds.Data.Add(new ChartDataset()
            {
                Label = $"销售量",
                Data = orders.Select(a => a.Quantity).Cast<object>()
            });

            Total = orders.Sum(a => a.Quantity);

            return Task.CompletedTask;
        }

        protected override void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                Orders.DemoDatas(fsql!);
            }
        }

    }
}
