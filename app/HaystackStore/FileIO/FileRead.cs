using System.IO.Compression;
using System.Text.RegularExpressions;
using MathNet.Numerics.Distributions;
using Microsoft.Win32.SafeHandles;

namespace HaystackStore;

public class FileRead : IFileRead
{
    private IFileWait _fileWait;

    private readonly FileOptions O_DIRECT = (FileOptions)0x4000;

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
        var options = new FileStreamOptions();

        options.Mode = FileMode.Open;
        options.Access = FileAccess.Read;
        options.Share = FileShare.Read;
        options.Options = O_DIRECT;

        using var mem = new MemoryStream();
        using var fs = new FileStream(filePath, options);

        fs.CopyTo(mem);

        var bytes = mem.ToArray();

        _fileWait.WaitBytesRead(bytes.Length);

        return bytes;
    }
}

