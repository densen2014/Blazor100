// See https://aka.ms/new-console-template for more information
using System.IO.Compression;

Console.WriteLine("Hello, World!");
if (File.Exists("测试文件.zip")) File.Delete("测试文件.zip");
if (Directory.Exists("unzip"))
{
    Directory.Delete("unzip");
}
Directory.CreateDirectory("unzip");

zipFolder("测试文件", "测试文件.zip");
unzipFile("测试文件.zip", "unzip");

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
void unzipFile(string zipPath, string folderPath)
{
    using (ZipArchive archive = ZipFile.Open(zipPath, ZipArchiveMode.Read))
    {
        foreach (ZipArchiveEntry entry in archive.Entries)
        {
            entry.ExtractToFile(Path.Combine(folderPath, entry.FullName));
        }
    }
}
