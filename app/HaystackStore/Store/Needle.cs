namespace HaystackStore;

public class Needle : INeedle
{
    // uint allows full 32-bit range for 0x12345678
    public uint Header { get; set; }
    public long Key { get; set; }
    public bool Flags { get; set; }
    public int Size { get; set; }
    public byte[] Data { get; set; } = [];
    public uint Footer { get; set; }
    public uint Checksum { get; set; }
    public int Padding { get; set; }
    public int TotalSize =>
        4 + // Header
        8 + // Key
        1 + // Flags 
        4 + // Size
        Data.Length +
        4 + // Footer
        8 + // Checksum
        Padding;

    public byte[] GetBytes()
    {
        return new byte[0];
        // byte[] intBytes = BitConverter.GetBytes(intValue);
    }
}
