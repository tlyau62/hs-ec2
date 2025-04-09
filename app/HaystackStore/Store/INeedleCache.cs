namespace HaystackStore;

public interface INeedleCache
{
    Metadata? GetNeedle(int key);
    void CacheNeedle(INeedle needle, long offset);
}

