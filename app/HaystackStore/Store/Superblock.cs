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

    /**
     * return offset
     */
    public int AppendNeedle(INeedle needle)
    {
        return 0;
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

    private byte[] CalculatePadding(int currentSize)
    {
        const int ALIGNMENT = 8;
        int paddingNeeded = (ALIGNMENT - (currentSize % ALIGNMENT)) % ALIGNMENT;
        return new byte[paddingNeeded]; // Initialized to zeros
    }
}

