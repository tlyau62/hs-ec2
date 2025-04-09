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
        var bytes = new List<byte>();

        bytes.AddRange(BitConverter.GetBytes(Header));
        bytes.AddRange(BitConverter.GetBytes(Key));
        bytes.AddRange(BitConverter.GetBytes(Flags));
        bytes.AddRange(BitConverter.GetBytes(Size));
        bytes.AddRange(Data);
        bytes.AddRange(BitConverter.GetBytes(Footer));

        return bytes.ToArray();
    }
}
