namespace HaystackStore;

public class StoreService : IStoreService
{
    private readonly IVolumeManger _volumeManger;

    private readonly INeedleCache _needleCache;

    public StoreService(IVolumeManger volumeManger, INeedleCache needleCache)
    {
        _volumeManger = volumeManger;
        _needleCache = needleCache;

        Init();
    }

    public byte[] ReadPhoto(int key)
    {
        var volume = _volumeManger.GetVolume(key); // hash
        var metadata = _needleCache.GetNeedle(volume.VolumeId, key) ??
            throw new InvalidOperationException($"Fail to get metadata for key {key}");
        var needle = volume.ReadNeedle(metadata.Offset);

        return needle.Data;
    }

    public void WritePhoto(int key, byte[] data)
    {
        var volume = _volumeManger.GetVolume(key); // hash
        var metadata = volume.WriteNeedle(key, data);

        _needleCache.CacheNeedle(volume.VolumeId, metadata);
    }

    private void Init()
    {
        foreach (var vol in _volumeManger.GetVolumes())
        {
            foreach (var metadata in vol.GetAllMetadata())
            {
                _needleCache.CacheNeedle(vol.VolumeId, metadata);
            }
        }
    }
}

