namespace HaystackStore;

public class NeedleCache : INeedleCache
{
    private readonly Dictionary<int, Dictionary<long, Metadata>> _caches = [];

    public NeedleCache(int volumeSizes)
    {
        Init(volumeSizes);
    }

    public Metadata? GetNeedle(int volumeId, long key)
    {
        var metadata = _caches[volumeId][key];

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

