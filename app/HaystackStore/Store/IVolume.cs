namespace HaystackStore;

public interface IVolume
{
    int VolumeId { get; }
    /**
     * return offset
     */
    Metadata WriteNeedle(long key, byte[] data);
    Needle ReadNeedle(long offset);
    byte[] ReadData(Metadata metadata);
    IEnumerable<Metadata> GetAllMetadata();
}

