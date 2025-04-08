namespace HaystackStore;

public class Needle : INeedle
{
    public Needle(long key, byte[] data)
    {
        Key = key;
        Data = data;
    }

    public int Header { get; }
    public long Key { get; set; }
    public bool Flags { get; set; }
    public int[] Size { get; set; }
    public byte[] Data { get; set; }
    public int Footer { get; set; }
    public byte[] Checksum { get; set; }
    public byte[] Padding { get; set; }
}

