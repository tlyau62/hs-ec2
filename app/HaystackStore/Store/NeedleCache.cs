namespace HaystackStore;

public class NeedleCache : INeedleCache
{
    private readonly Dictionary<int, Dictionary<long, Metadata>> _caches = [];

    public NeedleCache(IConfiguration config)
    {
        var size = config.GetValue<int?>("Haystack:Size") ??
            throw new InvalidDataException("missing config Haystack:Size");
        Init(size);
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

    private void Init(int volumeSizes)
    {
        for (var i = 0; i < volumeSizes; i++)
        {
            _caches[i] = [];
        }
    }
}

