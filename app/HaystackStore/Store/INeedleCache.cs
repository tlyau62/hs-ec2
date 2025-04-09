namespace HaystackStore;

public interface INeedleCache
{
    Metadata? GetNeedle(int volumeId, long key);
    void CacheNeedle(int volumeId, Metadata needle);
}

