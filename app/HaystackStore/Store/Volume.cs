namespace HaystackStore;

public class Volume : IVolume, IDisposable
{
    private readonly FileStream _volumeFile;

    private int _id;

    public Volume(int id, FileStream volumeFile)
    {
        _id = id;
        _volumeFile = volumeFile;
    }

    public int VolumeId => _id;

    public Metadata WriteNeedle(long key, byte[] data)
    {
        var needle = new Needle
        {
            Header = Needle.HEADER_MAGIC_NUMBER,
            Key = key,
            Flags = false,
            Size = data.Length,
            Data = data,
            Footer = Needle.FOOTER_MAGIC_NUMBER,
        };

        needle.Checksum = CalculateChecksum(needle);

        var offset = WriteNeedleToDisk(needle);

        return CreateMetadata(offset, needle);
    }

    private long WriteNeedleToDisk(Needle needle)
    {
        using var writer = new BinaryWriter(_volumeFile, System.Text.Encoding.UTF8, true);
        var offset = _volumeFile.Seek(0, SeekOrigin.End);

        writer.Write(needle.Header);
        writer.Write(needle.Key);
        writer.Write(needle.Flags ? (byte)1 : (byte)0);
        writer.Write(needle.Size);
        writer.Write(needle.Data);
        writer.Write(needle.Checksum);
        writer.Write(needle.Footer);
        writer.Write(needle.Padding);

        return offset;
    }

    public Needle ReadNeedle(long offset)
    {
        using var reader = new BinaryReader(_volumeFile, System.Text.Encoding.UTF8, true);
        _volumeFile.Seek(offset, SeekOrigin.Begin);

        var needle = new Needle
        {
            Header = reader.ReadUInt32(),
            Key = reader.ReadInt64(),
            Flags = reader.ReadByte() == 1,
            Size = reader.ReadInt32()
        };

        needle.Data = reader.ReadBytes(needle.Size);
        needle.Checksum = reader.ReadUInt32();
        needle.Footer = reader.ReadUInt32();

        if (needle.Header != Needle.HEADER_MAGIC_NUMBER)
        {
            throw new InvalidDataException("Invalid needle header");
        }

        if (needle.Footer != Needle.FOOTER_MAGIC_NUMBER)
        {
            throw new InvalidDataException("Invalid needle footer");
        }

        if (needle.Checksum != CalculateChecksum(needle))
        {
            throw new InvalidDataException("Checksum verification failed");
        }

        return needle;
    }

    private uint CalculateChecksum(Needle needle)
    {
        var checksum = (uint)needle.TotalBytes.Sum(b => b);

        checksum -= (uint)BitConverter.GetBytes(needle.Checksum).Sum(b => b);

        return checksum;
    }

    public void Dispose()
    {
        _volumeFile?.Dispose();
    }

    public IEnumerable<Metadata> GetAllMetadata()
    {
        var list = new List<Metadata>();
        long offset = 0;

        while (offset < _volumeFile.Length)
        {
            var needle = ReadNeedle(offset);
            var index = CreateMetadata(offset, needle);
            list.Add(index);
            offset = needle.TotalBytes.Length;
        }

        return list;
    }

    private Metadata CreateMetadata(long offset, Needle needle)
    {
        return new Metadata()
        {
            Key = needle.Key,
            Flags = needle.Flags,
            Offset = offset,
            Size = needle.Size
        };
    }
}