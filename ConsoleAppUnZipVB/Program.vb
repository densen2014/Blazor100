Imports System
Imports System.IO
Imports System.IO.Compression
Imports System.Text

Module Program
    Sub Main(args As String())
        Console.WriteLine("Hello World!")
        System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance)

        If File.Exists("测试文件.zip") Then
            File.Delete("测试文件.zip")
        End If
        If Directory.Exists("unzip") Then
            Directory.Delete("unzip", True)
        End If
        Directory.CreateDirectory("unzip")

        zipFolder("测试文件", "测试文件.zip")
        unzipFile("测试文件.zip", "unzip")
        unzipFile("ref.zip", "unzip")
    End Sub



    'zip a folder
    Private Sub zipFolder(folderPath As String, zipPath As String)
        Using archive As ZipArchive = ZipFile.Open(zipPath, ZipArchiveMode.Create)
            For Each file As String In Directory.GetFiles(folderPath)
                archive.CreateEntryFromFile(file, Path.GetFileName(file))
            Next
        End Using
    End Sub

    'unzip a file
    Private Sub unzipFile(zip文件路径 As String, 解压路径 As String)
        If Not Directory.Exists(解压路径) Then
            Directory.CreateDirectory(解压路径)
        End If
        ZipFile.ExtractToDirectory(zip文件路径, 解压路径, Text.Encoding.GetEncoding(“GB2312”), True)
    End Sub

End Module
