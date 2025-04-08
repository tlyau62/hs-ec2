namespace HaystackStore;

public interface INeedle
{
    int Header { get; }
    long Key { get; set; }
    bool Flags { get; set; }
    int[] Size { get; set; }
    byte[] Data { get; set; }
    int Footer { get; set; }
    byte[] Checksum { get; set; }
    byte[] Padding { get; set; }
}

