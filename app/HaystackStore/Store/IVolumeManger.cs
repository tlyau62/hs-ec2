namespace HaystackStore;

public interface IVolumeManger
{
    IVolume GetVolume(int key);
    IEnumerable<IVolume> GetVolumes();
}

