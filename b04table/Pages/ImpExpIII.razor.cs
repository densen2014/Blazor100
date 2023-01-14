// ********************************** 
// Densen Informatica 中讯科技 
// 作者：Alex Chow
// e-mail:zhouchuanglin@gmail.com 
// **********************************

using AmeBlazor.Components;
using b14table.Data;
using Blazor100.Service;
using BootstrapBlazor.Components;
using Densen.DataAcces.FreeSql;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using System.Diagnostics.CodeAnalysis;
using static Blazor100.Service.ImportExportsService;

namespace b14table.Pages
{
    public partial class ImpExpIII
    {

        [Inject]
        IWebHostEnvironment? HostEnvironment { get; set; }

        [Inject]
        [NotNull]
        NavigationManager? NavigationManager { get; set; }

        [Inject]
        [NotNull]
        ImportExportsService? ImportExportsService { get; set; }

        [Inject]
        [NotNull]
        ToastService? ToastService { get; set; }

        [Inject]
        [NotNull]
        FreeSqlDataService<SalesChannels>? DataService { get; set; }

        [NotNull]
        Table<SalesChannels>? list1 { get; set; }

        [Parameter] public int Footercolspan1 { get; set; } = 3;

        [Parameter] public int Footercolspan2 { get; set; } = 2;

        [Parameter] public int Footercolspan3 { get; set; }

        [Parameter] public int FootercolspanTotal { get; set; } = 2;

        [Parameter] public string? FooterText { get; set; } = "合计：";

        [Parameter] public string? FooterText2 { get; set; }

        [Parameter] public string? FooterText3 { get; set; }

        [Parameter] public string? FooterTotal { get; set; }

        /// <summary>
        /// 获得/设置 IJSRuntime 实例
        /// </summary>
        [Inject]
        [NotNull]
        protected IJSRuntime? JsRuntime { get; set; }
        [Parameter] public string? 新窗口打开Url { get; set; } = "https://localhost:7292/";

        // 由于使用了FreeSql ORM 数据服务,可以直接取对象
        [Inject]
        [NotNull]
        IFreeSql? fsql { get; set; }

        [Inject] ToastService? toastService { get; set; }
        [Inject] SwalService? SwalService { get; set; }


        public bool IsExcel { get; set; }
        public bool DoubleClickToEdit { get; set; } = true;
        protected string UploadPath = "";
        protected string? uploadstatus;
        long maxFileSize = 1024 * 1024 * 15;
        string? tempfilename;

        private AggregateType Aggregate { get; set; }
        
        protected async Task GetDatasAsync()
        {
            var datas = GetDemoDatas();
            await fsql.Insert<SalesChannels>().AppendData(datas).ExecuteAffrowsAsync();
            await list1!.QueryAsync();
        }

        protected override async void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                UploadPath = Path.Combine(HostEnvironment!.WebRootPath, "uploads");
                if (!Directory.Exists(UploadPath)) Directory.CreateDirectory(UploadPath);
                await list1!.QueryAsync();
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                //懒的人,直接初始化一些数据用用
                var res = fsql.Select<SalesChannels>().Count();
                if (res == 0)
                {
                    var datas = GetDemoDatas();
                    await fsql.Insert<SalesChannels>().AppendData(datas).ExecuteAffrowsAsync();
                    await list1!.QueryAsync();
                }
            }
        }

        public List<SalesChannels> GetDemoDatas()
        {

            var list = new List<SalesChannels>();
            for (int i = 0; i < 100; i++)
            {
                try
                {
                    var total = Random.Shared.Next(100, 3000);
                    list.Add(new SalesChannels()
                    {
                        ID = i,
                        Name = "渠道" + i,
                        Date = DateTime.Now,
                        Projects = Random.Shared.Next(10, 55),
                        Orders = Random.Shared.Next(3, 10),
                        Qualifieds = i,
                        Total = total,
                        Receivables = total - i,
                        Received = i,
                        Remark= $"{i} 明细行内嵌套另外一个 Table 组件，由于每行都要关联子表数据，出于性能的考虑，此功能采用 懒加载 模式，即点击展开按钮后，再对嵌套 Table 进行数据填充，通过 ShowDetailRow 回调委托可以控制每一行是否显示明细行，本例中通过 Complete 属性来控制是否显示明细行，可通过翻页来测试本功能"
                    });

                }
                catch (Exception e)
                {
                    System.Console.WriteLine(e.Message);
                }
            }
            return list;

        }

        private Task IsExcelToggle()
        {
            IsExcel = !IsExcel;
            DoubleClickToEdit = !IsExcel;
            StateHasChanged();
            return Task.CompletedTask;
        }

        public async Task<bool> Export模板Async()
        {
            await Export(exportTemplate:true);
            return true;
        }

        private async Task<bool> ExportExcelAsync(IEnumerable<SalesChannels> items) => await ExportAutoAsync(items, ExportType.Excel);
        private async Task<bool> ExportPDFAsync(IEnumerable<SalesChannels> items) => await ExportAutoAsync(items, ExportType.Pdf);
        private async Task<bool> ExportWordAsync(IEnumerable<SalesChannels> items) => await ExportAutoAsync(items, ExportType.Word);
        private async Task<bool> ExportHtmlAsync(IEnumerable<SalesChannels> items) => await ExportAutoAsync(items, ExportType.Html);

        private async Task<bool> ExportAutoAsync(IEnumerable<SalesChannels> items, ExportType exportType = ExportType.Excel)
        {
            if (items == null || !items.Any())
            {
                await ToastService.Error("提示", "无数据可导出");
                return false;
            }
            var option = new ToastOption()
            {
                Category = ToastCategory.Information,
                Title = "提示",
                Content = $"导出正在执行,请稍等片刻...",
                IsAutoHide = false
            };
            // 弹出 Toast
            await ToastService.Show(option);
            await Task.Delay(100);


            // 开启后台进程进行数据处理
            await Export(items?.ToList(), exportType);

            // 关闭 option 相关联的弹窗
            option.Close();

            // 弹窗告知下载完毕
            await ToastService.Show(new ToastOption()
            {
                Category = ToastCategory.Success,
                Title = "提示",
                Content = $"导出成功,请检查数据",
                IsAutoHide = false
            });
            return true;

        }

        private async Task Export(List<SalesChannels>? items = null, ExportType exportType = ExportType.Excel,bool exportTemplate = false)
        {
            try
            {
                if (!exportTemplate && (items == null || !items.Any()))
                {
                    ToastService?.Error($"导出", $"{exportType}出错,无数据可导出");
                    return;
                }
                var fileName = items == null ? "模板" : typeof(SalesChannels).Name;
                var fullName = Path.Combine(UploadPath, fileName);
                fullName = await ImportExportsService.Export(fullName, items, exportType);
                fileName = (new System.IO.FileInfo(fullName)).Name;
                ToastService?.Success("提示", fileName + "已生成");

                //下载后清除文件
                NavigationManager.NavigateTo($"uploads/{fileName}", true);
                _ = Task.Run(() =>
                {
                    Thread.Sleep(50000);
                    System.IO.File.Delete(fullName);
                });

            }
            catch (Exception e)
            {
                ToastService?.Error($"导出", $"{exportType}出错,请检查. {e.Message}");
            }
        }

        public async Task<bool> EmptyAll()
        {
            fsql.Delete<SalesChannels>().Where(a => 1 == 1).ExecuteAffrows();
            await ToastService!.Show(new ToastOption()
            {
                Category = ToastCategory.Success,
                Title = "提示",
                Content = "已清空数据",
            });

            await list1!.QueryAsync();
            return true;
        }
        private async Task ImportExcel()
        {
            if (string.IsNullOrEmpty(tempfilename))
            {
                ToastService?.Error("提示", "请正确选择文件上传");
                return;
            }
            var option = new ToastOption()
            {
                Category = ToastCategory.Information,
                Title = "提示",
                Content = "导入文件中,请稍等片刻...",
                IsAutoHide = false
            };
            // 弹出 Toast
            await ToastService!.Show(option);
            await Task.Delay(100);


            // 开启后台进程进行数据处理
            var isSuccess = await MockImportExcel();

            // 关闭 option 相关联的弹窗
            option.Close();

            // 弹窗告知下载完毕
            await ToastService.Show(new ToastOption()
            {
                Category = isSuccess ? ToastCategory.Success : ToastCategory.Error,
                Title = "提示",
                Content = isSuccess ? "操作成功,请检查数据" : "出现错误,请重试导入或者上传",
                IsAutoHide = false
            });

            await list1!.QueryAsync();
        }
        private async Task<bool> MockImportExcel()
        {
            var items_temp = await ImportExportsService!.ImportFormExcel<SalesChannels>(tempfilename!);
            if (items_temp.items == null)
            {
                ToastService?.Error("提示", "文件导入失败: " + items_temp.error);
                return false;
            }
            //items = SmartCombine(items_temp, items).ToList(); 新数据和老数据合并处理,略100字
            await fsql.Insert<SalesChannels>().AppendData(items_temp!.items.ToList()).ExecuteAffrowsAsync();
            return true;
        }

        protected async Task OnChange(InputFileChangeEventArgs e)
        {
            if (e.File == null) return;
            tempfilename = Path.Combine(UploadPath, e.File.Name);
            await using FileStream fs = new(tempfilename, FileMode.Create);
            using var stream = e.File.OpenReadStream(maxFileSize);
            await stream.CopyToAsync(fs);

            //正式工程此处是回调,简化版必须InvokeAsync一下,自由发挥
            _ = Task.Run(async () => await InvokeAsync(async () => await ImportExcel()));

        }

        /// <summary>
        /// 导出数据方法
        /// </summary>
        /// <param name="Items"></param>
        /// <param name="opt"></param>
        /// <returns></returns>
        protected async Task<bool> ExportAsync(IEnumerable<SalesChannels> Items, QueryPageOptions opt)
        {
            var ret = await ExportExcelAsync(Items);
            return ret;
        }

        public Task PrintPreview(IEnumerable<SalesChannels> item)
        {
            //实际工程自己完善js打印
            JsRuntime.InvokeVoidAsync("printDiv");
            return Task.CompletedTask;
        }

        private Task 新窗口打开()
        {
            if (string.IsNullOrEmpty(新窗口打开Url))
            {
                ToastService?.Error("提示", "Url为空!");
                return Task.CompletedTask;
            }
            JsRuntime.NavigateToNewTab(新窗口打开Url);
            return Task.CompletedTask;
        }

        public async Task 批量审批(IEnumerable<SalesChannels> items)
        {
            items.ToList().ForEach(a =>
            {
                a.Checkouts = a.Orders;
                a.Receivables = 0;
                a.Received = a.Total;
                a.ModifiedDate = DateTime.Now;
            });
            var res = await fsql.Update<SalesChannels>().SetSource(items).ExecuteAffrowsAsync();

            await SwalService!.Show(new SwalOption()
            {
                Title = res == 0 ? "提示: 操作失败" : "提示: 操作成功"

            });
           if (res != 0) await list1!.QueryAsync();

        }



    }
}
