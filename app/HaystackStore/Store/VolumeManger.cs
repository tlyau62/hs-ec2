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
            var path = GetVolumePath(i);
            var volumeFile = File.Exists(path) ? File.OpenRead(path) : File.Create(path);
            var volume = new Volume(volumeFile);

            _volumes[i] = volume;
        }
    }
}

