namespace HaystackStore;

public class NeedleCache : INeedleCache
{
    private readonly Dictionary<int, Dictionary<long, Metadata>> _caches = [];

    public NeedleCache(IConfiguration config)
    {
        var mountFolder = config.GetValue<string>("Haystack:MountFolder") ??
              throw new InvalidDataException("missing config Haystack:MountFolder");

        Init(mountFolder);
    }

    public Metadata? GetNeedle(int volumeId, long key)
    {
        _caches[volumeId].TryGetValue(key, out Metadata? metadata);

        return metadata;
    }

    public void CacheNeedle(int volumeId, Metadata needle)
    {
        _caches[volumeId][needle.Key] = needle;
    }

    private void Init(string mountFolder)
    {
        var directories = Directory.GetDirectories(mountFolder);

        var nVols = directories.Length;

        if (nVols == 0)
        {
            throw new InvalidOperationException("No volume is detected");
        }

        for (var i = 0; i < nVols; i++)
        {
            _caches[i] = [];
        }
    }
}

