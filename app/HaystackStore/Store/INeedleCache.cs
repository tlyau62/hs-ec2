namespace HaystackStore;

public interface INeedleCache
{
    Metadata? GetNeedle(int key);
    void CacheNeedle(Needle needle, int offset);
}

