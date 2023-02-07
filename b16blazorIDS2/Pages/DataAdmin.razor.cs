// ********************************** 
// Densen Informatica 中讯科技 
// 作者：Alex Chow
// e-mail:zhouchuanglin@gmail.com 
// **********************************

using Blazor100.Service;
using BootstrapBlazor.Components;
using Microsoft.AspNetCore.Components;
using System.Diagnostics.CodeAnalysis;

namespace b16blazorIDS2.Pages
{
    public partial class DataAdmin
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
         

        private Task IsExcelToggle()
        {
            IsExcel = !IsExcel;
            DoubleClickToEdit = !IsExcel;
            StateHasChanged();
            return Task.CompletedTask;
        }
 


    }
}
