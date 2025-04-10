namespace HaystackStore;

public class Needle
{
    public const uint HEADER_MAGIC_NUMBER = 0x12345678;
    public const uint FOOTER_MAGIC_NUMBER = 0x09ABCDEF;

    // uint allows full 32-bit range for 0x12345678
    public uint Header { get; set; }
    public long Key { get; set; }
    public bool Flags { get; set; }
    public int Size { get; set; }
    public byte[] Data { get; set; } = [];
    public uint Footer { get; set; }
    public uint Checksum { get; set; }
    public byte[] TotalBytes
    {
        get
        {
            var bytes = new List<byte>();

            bytes.AddRange(BitConverter.GetBytes(Header));
            bytes.AddRange(BitConverter.GetBytes(Key));
            bytes.Add((byte)(Flags ? 1 : 0));
            bytes.AddRange(BitConverter.GetBytes(Size));
            bytes.AddRange(Data);
            bytes.AddRange(BitConverter.GetBytes(Footer));
            bytes.AddRange(BitConverter.GetBytes(Checksum));

            return bytes.ToArray();
        }
    }

    public byte[] Padding
    {
        get
        {
            const int ALIGNMENT = 8;
            var currentSize = TotalBytes.Length;
            int paddingNeeded = (ALIGNMENT - (currentSize % ALIGNMENT)) % ALIGNMENT;
            return new byte[paddingNeeded];
        }
    }

    public override string ToString()
    {
        return $"Header = {Header}, Key = {Key}, Flags = {Flags}, Size = {Size}";
    }
}
