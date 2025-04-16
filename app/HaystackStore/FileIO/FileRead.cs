using System.IO.Compression;
using System.Text.RegularExpressions;
using MathNet.Numerics.Distributions;
using Microsoft.Win32.SafeHandles;

namespace HaystackStore;

public class FileRead : IFileRead
{
    private IFileWait _fileWait;

    public FileRead(IFileWait fileWait)
    {
        _fileWait = fileWait;
    }

    public bool Exists(string filePath)
    {
        _fileWait.WaitMetadataRead();

        return File.Exists(filePath);
    }

    public byte[] ReadAllBytes(string filePath)
    {
        var bytes = File.ReadAllBytes(filePath);

        _fileWait.WaitBytesRead(bytes.Length);

        return bytes;
    }
}

