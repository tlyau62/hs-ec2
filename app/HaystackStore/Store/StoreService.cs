using System.IO.Compression;
using System.Text.RegularExpressions;

namespace HaystackStore;

public class StoreService : IStoreService
{
    private readonly IVolumeManger _volumeManger;

    private readonly INeedleCache _needleCache;

    private readonly string _uploadArea;

    public StoreService(IVolumeManger volumeManger, INeedleCache needleCache, IConfiguration config)
    {
        _volumeManger = volumeManger;
        _needleCache = needleCache;
        _uploadArea = config.GetValue<string>("UploadArea") ??
            throw new InvalidDataException("missing config UploadArea");

        Init();
    }

    public byte[]? ReadPhoto(int key)
    {
        var volume = _volumeManger.GetVolume(key); // hash
        var metadata = _needleCache.GetNeedle(volume.VolumeId, key);

        if (metadata == null)
        {
            return null;
        }

        var needle = volume.ReadNeedleAtomic(metadata);

        return needle.Data;
    }

    public void WritePhoto(int key, byte[] data)
    {
        var volume = _volumeManger.GetVolume(key); // hash
        var metadata = volume.WriteNeedle(key, data);

        _needleCache.CacheNeedle(volume.VolumeId, metadata);
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

    private void Init()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(_uploadArea));

        foreach (var vol in _volumeManger.GetVolumes())
        {
            foreach (var metadata in vol.GetAllMetadata())
            {
                _needleCache.CacheNeedle(vol.VolumeId, metadata);
            }
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

