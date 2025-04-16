using System.IO.Compression;
using System.Text.RegularExpressions;

namespace HaystackStore;

public class FsStoreService : IFsStoreService
{
    private readonly string mountFolder = "/Users/tlyau/Documents/git/cs5296-project/app/HaystackStore/Fs";

    private readonly string _uploadArea;

    private readonly IFileRead _fileRead;

    public FsStoreService(IConfiguration config, IFileRead fileRead)
    {
        mountFolder = config.GetValue<string>("Fs:MountFolder") ??
            throw new InvalidDataException("missing config Fs:MountFolder");
        _uploadArea = config.GetValue<string>("UploadArea") ??
            throw new InvalidDataException("missing config UploadArea");
        _fileRead = fileRead;
    }

    public byte[]? ReadPhoto(int key)
    {
        var filePath = Path.Join(mountFolder, $"{key}.png");

        if (!_fileRead.Exists(filePath))
        {
            return null;
        }

        return _fileRead.ReadAllBytes(filePath);
    }

    public void WritePhoto(int key, byte[] data)
    {
        var filePath = Path.Join(mountFolder, $"{key}.png");

        File.WriteAllBytes(filePath, data);
    }

    public void UnpackPhotos(string keyPattern, byte[] data)
    {
        using var stream = new MemoryStream(data);
        using var zip = new ZipArchive(stream);
        var keyregex = new Regex(keyPattern);

        foreach (var file in zip.Entries)
        {
            var filename = Path.GetFileNameWithoutExtension(file.Name);
            var matches = keyregex.Matches(filename);

            if (!matches.Any())
            {
                continue;
            }

            var keystr = keyregex.Matches(filename)[0].Groups[0].Captures[0].Value;

            var key = int.Parse(keystr);
            using var fileData = file.Open();
            using var fileStream = new MemoryStream();

            fileData.CopyTo(fileStream);

            WritePhoto(key, fileStream.ToArray());
        }
    }

    public void LoadPhotos(string keyPattern, string location)
    {
        var fileLocation = Path.Join(_uploadArea, location);

        if (!File.Exists(fileLocation))
        {
            return;
        }

        var data = File.ReadAllBytes(fileLocation);

        UnpackPhotos(keyPattern, data);
    }
}

