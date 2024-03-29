﻿using Blazor100.Data;
using BootstrapBlazor.Components;
using Microsoft.AspNetCore.Components;
using System.Diagnostics.CodeAnalysis;

namespace b06chart
{
    public partial class YearsCharts
    {
        [Inject]
        [NotNull]
        IFreeSql? fsql { get; set; }
        [Inject] ToastService? toastService { get; set; }
        List<Orders> orders { get; set; } = new List<Orders>();

        ChartsBase? charts;
        decimal Total { get; set; }
        string? TotalString2 { get; set; }
        private Task 数据生成(ChartDataSource ds)
        {
             var orders = fsql.Select<Orders>()
                                .Where(a =>  a.OrderDate.Year == charts!.Year)
                                .GroupBy(a => new
                                {
                                     a.OrderDate.Month
                                })
                                .ToList(a => new
                                {
                                    cou1 = a.Count(),
                                    OrderDate = a.Key.Month,
                                    Total = a.Sum(a.Value.SubTotal)
                                });

            orders = orders.OrderBy(a => a.OrderDate).ToList();

            ds.Labels = orders.Select(a => a.OrderDate.ToString());

            ds.Data.Add(new ChartDataset()
            {
                Label = $"单据数",
                Data = orders.Select(a => a.cou1).Cast<object>()
            });
            ds.Data.Add(new ChartDataset()
            {
                Label = $"金额",
                Data = orders.Select(a => a.Total).Cast<object>()
            });
            Total = orders.Select(a => a.Total).Sum();
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
