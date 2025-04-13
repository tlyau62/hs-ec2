using System.Reflection.Metadata;

namespace HaystackStore;

public class VolumeManger : IVolumeManger
{
    private int nVols = 0;

    private readonly string mountFolder = "/Users/tlyau/Documents/git/cs5296-project/app/HaystackStore/Volumes";

    private readonly string volumePrefix = "volume_";

    private IDictionary<int, Volume> _volumes = new Dictionary<int, Volume>();

    public VolumeManger(IConfiguration config)
    {
        mountFolder = config.GetValue<string>("Haystack:MountFolder") ??
            throw new InvalidDataException("missing config Haystack:MountFolder");
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

    public string GetVolumePath(string dir, int id)
    {
        return Path.Join(dir, volumePrefix + id);
    }

    private void Init()
    {
        var directories = Directory.GetDirectories(mountFolder);

        nVols = directories.Length;

        if (nVols == 0)
        {
            throw new InvalidOperationException("No volume is detected");
        }

        for (var i = 0; i < nVols; i++)
        {
            var dir = directories[i];
            var filePath = GetVolumePath(dir, i);
            FileStream volumeFile;
            Volume volume;

            if (!File.Exists(filePath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
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

