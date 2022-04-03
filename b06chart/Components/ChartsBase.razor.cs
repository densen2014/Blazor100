using BootstrapBlazor.Components;
using Microsoft.AspNetCore.Components;
using System.Diagnostics.CodeAnalysis;

namespace b06chart
{
    public partial class ChartsBase
    {
        [NotNull] private Chart? LineChart { get; set; }
        [NotNull] private Chart? BarChart { get; set; }

        /// <summary>
        /// 设定当前年份
        /// </summary>
        [Parameter] public int Year { get; set; } = DateTime.Now.Year;

        /// <summary>
        /// 设定当前月份
        /// </summary>
        [Parameter] public int Month { get; set; } = DateTime.Now.Month;

        /// <summary>
        /// 设定图表抬头
        /// </summary>
        [Parameter] public string TitleCharts { get; set; } = "日报表";

        /// <summary>
        /// 设定X轴文本
        /// </summary>
        [Parameter] public string XAxesText { get; set; } = "天数";

        /// <summary>
        /// 设定Y轴文本
        /// </summary>
        [Parameter] public string YAxesText { get; set; } = "数值";

        /// <summary>
        /// 图表类型:是=LineChart,否=BarChart
        /// </summary>
        [Parameter] public bool IsLineChart { get; set; }

        /// <summary>
        /// 使用默认数据
        /// </summary>
        [Parameter] public bool IsDemo { get; set; }

        /// <summary>
        /// 显示月份选择器
        /// </summary>
        [Parameter] public bool IsShowMonthSelector { get; set; } = true;
        [Parameter] public EventCallback<ChartDataSource> OnInitCallback { get; set; }
        [Parameter] public EventCallback<ChartDataSource> 数据生成Callback { get; set; }
        [Parameter] public decimal Total { get; set; }
        [Parameter] public string? TotalString2 { get; set; }
        [Parameter] public string? TotalString3 { get; set; }

        /// <summary>
        /// 隐藏选择器
        /// </summary>
        [Parameter] public bool IsHideSelectores { get; set; }

        /// <summary>
        /// 使用/初始化日期选择控件日期
        /// </summary>
        [Parameter] public bool UseDateTimeRangeValue { get; set; }

        /// <summary>
        /// 是否合并Bar显示 默认false
        /// </summary>
        public bool IsStacked { get; set; } 

        /// <summary>
        /// 强刷显示控件控制,Hack一下
        /// </summary>
        private bool Show { get; set; } = true;
        public int LastCount { get; set; }
        public bool FirstLoad { get; set; } = true;
        public bool ForceRefresh { get; set; }
        private string? ClickItemID { get; set; }

        private IEnumerable<string> Colors { get; set; } = new List<string>() { "Blue", "Green", "Red",  "Orange", "Yellow", "Tomato", "Pink", "Violet" };

        #region 日期选择控件
        private DateTimeRangeValue DateTimeRangeValue1 { get; set; } = new DateTimeRangeValue();
        DateTime 起始日期 = DateTime.Today.FirstDay();
        DateTime 结束日期 = DateTime.Today.LastDay();
        private Task OnConfirm(DateTimeRangeValue value)
        {
            起始日期 = value.Start.FirstSecond();
            结束日期 = value.End.Year == 1 ? value.Start.LastSecond() : value.End.LastSecond();

            Chart chart = IsLineChart ? LineChart : BarChart;
            chart?.Update(ChartAction.Update);
            //StateHasChanged();
            return Task.CompletedTask;
        }
        private Task OnClear(DateTimeRangeValue value)
        {
            起始日期 = DateTime.Today.FirstDay();
            结束日期 = DateTime.Today.LastDay();
            Chart chart = IsLineChart ? LineChart : BarChart;
            chart?.Update(ChartAction.Update);
            //StateHasChanged();
            return Task.CompletedTask;
        }

        /// <summary>
        /// 设置日期选择控件日期
        /// </summary>
        /// <param name="_起始日期"></param>
        /// <param name="_结束日期"></param>
        /// <returns></returns>
        protected Task SetDates(DateTime _起始日期, DateTime _结束日期)
        {
            起始日期 = _起始日期;
            结束日期 = _结束日期;
            DateTimeRangeValue1.Start = 起始日期;
            DateTimeRangeValue1.End = 结束日期;
            return Task.CompletedTask;
        }
        #endregion

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (UseDateTimeRangeValue && firstRender) { 
                DateTimeRangeValue1.Start = 起始日期;
                DateTimeRangeValue1.End = 结束日期;
            }
        }
        private Task OnAfterInit()
        {
            System.Console.WriteLine("Bar 初始化完毕");
            return Task.CompletedTask;
        }

        /// <summary>
        /// 初始化 ChartDataSource
        /// </summary>
        /// <returns></returns>
        protected Task<ChartDataSource> OnInit()
        {
            var ds = new ChartDataSource();
            if (!OnInitCallback.HasDelegate)
            {
                ds.Options.Title = TitleCharts;
                ds.Options.X.Title = XAxesText;
                ds.Options.X.Stacked = IsStacked;
                ds.Options.Y.Title = YAxesText;
                ds.Options.Y.Stacked = IsStacked;
            }
            else
            {
                OnInitCallback.InvokeAsync(ds);
            }


            //设置自定义颜色
            ds.Options.Colors = new Dictionary<string, string>() {
                                    { "blue:", "rgb(54, 162, 235)" },
                                    { "green:", "rgb(75, 192, 192)" },
                                    { "red:", "rgb(255, 99, 132)" },
                                    { "orange:", "rgb(255, 159, 64)" },
                                    { "yellow:", "rgb(255, 205, 86)" },
                                    { "tomato:", "rgb(255, 99, 71)" },
                                    { "pink:", "rgb(255, 192, 203)" },
                                    { "violet:", "rgb(238, 130, 238)" },
                                };

            if (!数据生成Callback.HasDelegate)
                数据生成(ds);
            else
                数据生成Callback.InvokeAsync(ds);

            ForceRefresh = LastCount < (ds.Labels?.Count()??0);
            LastCount = ds.Labels?.Count()??0;

            if (!FirstLoad && ForceRefresh)
            {
                ReloadChart();
                ForceRefresh = false;
            }
            FirstLoad = false;

            return Task.FromResult(ds);
        }


        /// <summary>
        /// 数据生成,添加Labels和ChartDataset
        /// </summary>
        /// <param name="ds"></param>
        protected virtual void 数据生成(ChartDataSource ds)
        { 
        }

        private Task SetYear(int year)
        {
            Chart chart = IsLineChart ? LineChart : BarChart;
            Year = year;
            chart?.Update(ChartAction.Update);
            return Task.CompletedTask;
        }
        private Task SetMonth(int month)
        {
            Chart chart = IsLineChart ? LineChart : BarChart;
            Month = month;
            chart?.Update(ChartAction.Update);
            return Task.CompletedTask;
        }
        private Task PreMonth()
        {
            Chart chart = IsLineChart ? LineChart : BarChart;
            Year = Month - 1 >= 1 ? Year : Year - 1;
            Month = Month - 1 >= 1 ? Month - 1 : 12;
            chart?.Update(ChartAction.Update);
            return Task.CompletedTask;
        }
        private Task NextMonth()
        {
            Chart chart = IsLineChart ? LineChart : BarChart;
            Year = Month + 1 <= 12 ? Year : Year + 1;
            Month = Month + 1 <= 12 ? Month + 1 : 1;
            chart?.Update(ChartAction.Update);
            return Task.CompletedTask;
        }
        private Task SetNow()
        {
            Chart chart = IsLineChart ? LineChart : BarChart;
            Year = DateTime.Now.Year;
            Month = DateTime.Now.Month;
            chart?.Update(ChartAction.Update);
            return Task.CompletedTask;
        }

        private Task RandomData()
        {
            Chart chart = IsLineChart ? LineChart : BarChart;
            chart?.Update(ChartAction.Update);
            return Task.CompletedTask;
        }
        private Task SwitchChart()
        {
            IsLineChart = !IsLineChart;
            return Task.CompletedTask;
        }

        /// <summary>
        /// 切换合并显示
        /// </summary>
        private void SwitchStacked()
        {
            IsStacked = !IsStacked;
            ReloadChart();
        }

        /// <summary>
        /// 强刷控件,重新初始化控件外观
        /// </summary>
        private async void ReloadChart(bool reloadData=false)
        {
            Chart chart = IsLineChart ? LineChart : BarChart;
            if (reloadData) chart?.Update(ChartAction.Update);
            Show = false;
            await InvokeAsync(StateHasChanged);
            await Task.Delay(1);
            Show = true;
            await InvokeAsync(StateHasChanged);
        }
  

    }


    public static class DateTimeExtensions
    {
        public static DateTime FirstDay(this DateTime obj) => new DateTime(obj.Year, obj.Month, 1, 0, 0, 0);
        public static DateTime LastDay(this DateTime obj) => obj.FirstDay().AddMonths(1).AddDays(-1).LastSecond();
        public static DateTime FirstSecond(this DateTime obj) => new DateTime(obj.Year, obj.Month, obj.Day, 0, 0, 0);
        public static DateTime LastSecond(this DateTime obj) => new DateTime(obj.Year, obj.Month, obj.Day, 23, 59, 59);

    }
}
