using Microsoft.Maui.Controls;
using SignInMauiApp.Models;
using FreeSql;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Maui.Storage;
using MiniExcelLibs;

namespace SignInMauiApp;

public partial class SignInReportPage : ContentPage
{
    private readonly IFreeSql? _fsql;
    private List<SignInReportItem> _report = new();

    public SignInReportPage()
    {
        InitializeComponent();
        _fsql = IPlatformApplication.Current?.Services.GetService<IFreeSql>();
        StartDatePicker.Date = DateTime.Today.AddDays(-7);
        EndDatePicker.Date = DateTime.Today;
        LoadReport();
    } 

    private void LoadReport()
    {
        var start = StartDatePicker.Date;
        var end = EndDatePicker.Date!.Value.AddDays(1);
        _report = _fsql!.Select<SignInRecord, User, Tenant>()
            .LeftJoin((r, u, t) => r.UserId == u.Id)
            .LeftJoin((r, u, t) => r.TenantId == t.Id)
            .Where((r, u, t) => r.SignInTime >= start && r.SignInTime < end)
            .ToList((r, u, t) => new SignInReportItem()
            {
                Username = u.Username,
                TenantName = t.Name,
                SignInTime = r.SignInTime
            });
        ReportCollectionView.ItemsSource = _report;
    }

    private void OnQueryClicked(object sender, EventArgs e)
    {
        LoadReport();
    }

    private async void OnExportClicked(object sender, EventArgs e)
    {
        if (_report.Count == 0)
        {
            await DisplayAlertAsync("提示", "无数据可导出", "确定");
            return;
        }
        var fileName = $"签到报表_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
        var filePath = Path.Combine(FileSystem.CacheDirectory, fileName);
        await MiniExcel.SaveAsAsync(filePath, _report);
        if (!File.Exists(filePath))
        {
            await DisplayAlertAsync("错误", "文件未生成，无法分享", "确定");
            return;
        }
        try
        {
            await Share.RequestAsync(new ShareFileRequest
            {
                Title = "导出签到报表",
                File = new ShareFile(filePath),
            });
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("错误", ex.Message, "确定");
        }
        //await ShareText("签到报表已导出，点击查看。");
        //await ShareUri("file://" + filePath, Share.Default);
        //await ShareFile(filePath);
    }
    public async Task ShareText(string text)
    {
        await Share.Default.RequestAsync(new ShareTextRequest
        {
            Text = text,
            Title = "Share Text"
        });
    }

    public async Task ShareUri(string uri, IShare share)
    {
        await share.RequestAsync(new ShareTextRequest
        {
            Uri = uri,
            Title = "Share Web Link"
        });
    }
    public async Task ShareFile(string filename)
    {
        string fn = "Attachment.txt";
        string file = Path.Combine(FileSystem.CacheDirectory, filename);

        File.WriteAllText(file, "Hello World");

        await Share.Default.RequestAsync(new ShareFileRequest
        {
            Title = "Share text file",
            File = new ShareFile(file)
        });
    }
    public async Task ShareMultipleFiles()
    {
        string file1 = Path.Combine(FileSystem.CacheDirectory, "Attachment1.txt");
        string file2 = Path.Combine(FileSystem.CacheDirectory, "Attachment2.txt");

        File.WriteAllText(file1, "Content 1");
        File.WriteAllText(file2, "Content 2");

        await Share.Default.RequestAsync(new ShareMultipleFilesRequest
        {
            Title = "Share multiple files",
            Files = new List<ShareFile> { new ShareFile(file1), new ShareFile(file2) }
        });
    }
    public class SignInReportItem
    {
        public string? Username { get; set; }
        public string? TenantName { get; set; }
        public DateTime SignInTime { get; set; }
    }
}

