namespace HaystackStore;

public class Superblock : ISuperblock
{
    public const uint HEADER_MAGIC_NUMBER = 0x12345678;

    public const uint FOOTER_MAGIC_NUMBER = 0x09ABCDEF;

    private readonly FileStream _volumeFile;

    public Superblock(FileStream volumeFile)
    {
        _volumeFile = volumeFile;
    }

    public INeedle CreateNeedle(long key, byte[] data)
    {
        var needle = new Needle();

        needle.Header = HEADER_MAGIC_NUMBER;
        needle.Key = key;
        needle.Flags = false;
        needle.Size = data.Length;
        needle.Data = data;
        needle.Footer = FOOTER_MAGIC_NUMBER;
        needle.Checksum = CalculateChecksum(needle.GetBytes());
        needle.Padding = CalculatePadding(needle.TotalSize);

        return needle;
    }

    /**
     * return offset
     */
    public long AppendNeedle(INeedle needle)
    {
        using var writer = new BinaryWriter(_volumeFile, System.Text.Encoding.UTF8, true);
        var offset = _volumeFile.Seek(0, SeekOrigin.End);

        writer.Write(needle.Header);
        writer.Write(needle.Key);
        writer.Write(needle.Flags ? 1 : 0);
        writer.Write(needle.Size);
        writer.Write(needle.Data);
        writer.Write(needle.Footer);
        writer.Write(needle.Checksum);
        writer.Write(needle.Padding);
        writer.Write(new byte[needle.Padding]);

        return offset;
    }

    public INeedle ReadNeedle(long offset)
    {
        using var reader = new BinaryReader(_volumeFile, System.Text.Encoding.UTF8, true);

        _volumeFile.Seek(offset, SeekOrigin.Begin);

        var needle = new Needle();

        needle.Header = reader.ReadUInt32();

        if (needle.Header != HEADER_MAGIC_NUMBER)
        {
            throw new InvalidDataException("Invalid needle header");
        }

        needle.Key = reader.ReadInt64();
        needle.Flags = reader.ReadByte() == 1;
        needle.Size = reader.ReadInt32();
        needle.Data = reader.ReadBytes(needle.Size);

        needle.Footer = reader.ReadUInt32();
        if (needle.Footer != FOOTER_MAGIC_NUMBER)
        {
            throw new InvalidDataException("Invalid needle footer");
        }

        needle.Checksum = reader.ReadUInt32();
        if (needle.Checksum != CalculateChecksum(needle.GetBytes()))
        {
            throw new InvalidDataException("Checksum verification failed");
        }

        needle.Padding = reader.ReadInt32();
        reader.ReadBytes(needle.Padding); // consume the padding

        return needle;
    }

    // Very simple check sum
    private uint CalculateChecksum(byte[] data)
    {
        return (uint)data.Sum(b => (int)b);
    }

    private int CalculatePadding(int currentSize)
    {
        const int ALIGNMENT = 8;

        return (ALIGNMENT - (currentSize % ALIGNMENT)) % ALIGNMENT;
    }
}

