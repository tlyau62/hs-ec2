namespace HaystackStore;

public class StoreService : IStoreService
{
    private readonly IVolumeManger _volumeManger;

    private readonly INeedleCache _needleCache;

    public StoreService(IVolumeManger volumeManger, INeedleCache needleCache)
    {
        _volumeManger = volumeManger;
        _needleCache = needleCache;
    }

    public byte[] ReadPhoto(int key)
    {
        var volume = _volumeManger.GetVolume(key); // hash
        var metadata = _needleCache.GetNeedle(key) ??
            throw new InvalidOperationException($"Fail to get metadata for key {key}");
        var needle = volume.Superblock.ReadNeedle(metadata.Offset);

        return needle.Data;
    }

    public void WritePhoto(int key, byte[] data)
    {
        var volume = _volumeManger.GetVolume(key); // hash
        var needle = volume.Superblock.CreateNeedle(key, data);
        var offset = volume.Superblock.AppendNeedle(needle);

        _needleCache.CacheNeedle(needle, offset);
    }
}

