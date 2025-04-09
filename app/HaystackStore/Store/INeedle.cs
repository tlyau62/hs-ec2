namespace HaystackStore;

public interface INeedle
{
    uint Header { get; set; }
    long Key { get; set; }
    bool Flags { get; set; }
    int Size { get; set; }
    byte[] Data { get; set; }
    uint Footer { get; set; }
    uint Checksum { get; set; }
    int Padding { get; set; }
    // for calculating padding
    int TotalSize { get; }
    // for calculating checksum
    byte[] GetBytes();
}

