using System.Reflection.Metadata;

namespace HaystackStore;

public class VolumeManger : IVolumeManger
{
    private readonly int nVols = 1;

    private readonly string mountFolder = "/Users/tlyau/Documents/git/cs5296-project/app/HaystackStore/Volumes";

    private readonly string volumePrefix = "volume_";

    private IDictionary<int, Volume> _volumes = new Dictionary<int, Volume>();

    public VolumeManger()
    {
        Init();
    }

    public IVolume GetVolume(int key)
    {
        var id = GetVolumnId(key);
        var volume = _volumes[id] ?? throw new InvalidOperationException($"volume {id} not found");

        return volume;
    }

    public int GetVolumnId(int key)
    {
        return key % nVols;
    }

    public string GetVolumePath(int id)
    {
        return Path.Join(mountFolder, volumePrefix + id);
    }

    private void Init()
    {
        for (var i = 0; i < nVols; i++)
        {
            var filePath = GetVolumePath(i);
            FileStream volumeFile;
            Volume volume;

            if (!File.Exists(filePath))
            {
                volumeFile = new FileStream(filePath, FileMode.CreateNew, FileAccess.ReadWrite);
            }
            else
            {
                volumeFile = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite);
            }

            volume = new Volume(i, volumeFile);
            _volumes[i] = volume;
        }
    }

    public IEnumerable<IVolume> GetVolumes()
    {
        return _volumes.Values;
    }
}

