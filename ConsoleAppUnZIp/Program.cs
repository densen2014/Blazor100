// See https://aka.ms/new-console-template for more information
using System.IO.Compression;

System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

Console.WriteLine("Hello, World!");
if (File.Exists("测试文件.zip")) File.Delete("测试文件.zip");
if (Directory.Exists("unzip"))
{
    Directory.Delete("unzip",true);
}
Directory.CreateDirectory("unzip");

zipFolder("测试文件", "测试文件.zip");
unzipFile("测试文件.zip", "unzip");
unzipFile("ref.zip", "unzipref");
    
//zip a folder
void zipFolder(string folderPath, string zipPath)
{
    using (ZipArchive archive = ZipFile.Open(zipPath, ZipArchiveMode.Create))
    {
        foreach (string file in Directory.GetFiles(folderPath))
        {
            archive.CreateEntryFromFile(file, Path.GetFileName(file));
        }
    }
}

//unzip a file
void unzipFile(string zip文件路径, string 解压路径)
{
    if (!Directory.Exists(解压路径))
    {
        Directory.CreateDirectory(解压路径);
    }
    ZipFile.ExtractToDirectory(zip文件路径, 解压路径, System.Text.Encoding.GetEncoding("GB2312"), true);
}
