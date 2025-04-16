using Microsoft.Win32.SafeHandles;

namespace HaystackStore;

public class Volume : IVolume, IDisposable
{
    private readonly FileStream _volumeFile;

    private readonly SafeFileHandle _fileHandle;

    private int _id;

    private readonly IFileWait _fileWait;

    public Volume(int id, FileStream volumeFile, IFileWait fileWait)
    {
        _id = id;
        _volumeFile = volumeFile;
        _fileHandle = volumeFile.SafeFileHandle;
        _fileWait = fileWait;
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
            Footer = Needle.FOOTER_MAGIC_NUMBER
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

    public Needle ReadNeedleAtomic(Metadata metadata)
    {
        var byteLength = Needle.GetByteLength(metadata.Size);
        var bytes = new byte[byteLength].AsSpan();

        // Atomic read
        RandomAccess.Read(_fileHandle, bytes, metadata.Offset);

        var currentOffset = 0;

        // Read header
        var headerBytes = bytes.Slice(currentOffset, sizeof(uint));
        var header = BitConverter.ToUInt32(headerBytes);
        if (header != Needle.HEADER_MAGIC_NUMBER)
        {
            throw new InvalidDataException("Invalid needle header");
        }
        currentOffset += sizeof(uint);

        // Read key
        var keyBytes = bytes.Slice(currentOffset, sizeof(long));
        var key = BitConverter.ToInt64(keyBytes);
        currentOffset += sizeof(long);

        // Read flags
        var flagsBytes = bytes.Slice(currentOffset, sizeof(byte));
        var flags = flagsBytes[0];
        currentOffset += sizeof(byte);

        // Read size
        var sizeBytes = bytes.Slice(currentOffset, sizeof(int));
        var size = BitConverter.ToInt32(sizeBytes);
        currentOffset += sizeof(int);

        // Read data
        var data = bytes.Slice(currentOffset, size);
        currentOffset += size;

        // Read checksum
        var checksumBytes = bytes.Slice(currentOffset, sizeof(uint));
        var checksum = BitConverter.ToUInt32(checksumBytes);
        currentOffset += sizeof(uint);

        // Read footer
        var footerBytes = bytes.Slice(currentOffset, sizeof(uint));
        var footer = BitConverter.ToUInt32(footerBytes);
        if (footer != Needle.FOOTER_MAGIC_NUMBER)
        {
            throw new InvalidDataException("Invalid needle footer");
        }

        // Construct needle
        var needle = new Needle
        {
            Header = header,
            Key = key,
            Flags = flags == 1,
            Size = size,
            Data = data.ToArray(),
            Checksum = checksum,
            Footer = footer
        };

        if (needle.Checksum != CalculateChecksum(needle))
        {
            throw new InvalidDataException("Checksum verification failed");
        }

        _fileWait.WaitBytesRead(needle.TotalBytes.Length);

        return needle;
    }

    public Needle ReadNeedle(long offset)
    {
        var currentOffset = offset;

        // Read header
        var headerBytes = new byte[sizeof(uint)];
        RandomAccess.Read(_fileHandle, headerBytes.AsSpan(), currentOffset);
        var header = BitConverter.ToUInt32(headerBytes);
        if (header != Needle.HEADER_MAGIC_NUMBER)
        {
            throw new InvalidDataException("Invalid needle header");
        }
        currentOffset += sizeof(uint);

        // Read key
        var keyBytes = new byte[sizeof(long)];
        RandomAccess.Read(_fileHandle, keyBytes.AsSpan(), currentOffset);
        var key = BitConverter.ToInt64(keyBytes);
        currentOffset += sizeof(long);

        // Read flags
        var flagsBytes = new byte[sizeof(byte)];
        RandomAccess.Read(_fileHandle, flagsBytes.AsSpan(), currentOffset);
        var flags = flagsBytes[0];
        currentOffset += sizeof(byte);

        // Read size
        var sizeBytes = new byte[sizeof(int)];
        RandomAccess.Read(_fileHandle, sizeBytes.AsSpan(), currentOffset);
        var size = BitConverter.ToInt32(sizeBytes);
        currentOffset += sizeof(int);

        // Read data
        var data = new byte[size];
        RandomAccess.Read(_fileHandle, data.AsSpan(), currentOffset);
        currentOffset += size;

        // Read checksum
        var checksumBytes = new byte[sizeof(uint)];
        RandomAccess.Read(_fileHandle, checksumBytes.AsSpan(), currentOffset);
        var checksum = BitConverter.ToUInt32(checksumBytes);
        currentOffset += sizeof(uint);

        // Read footer
        var footerBytes = new byte[sizeof(uint)];
        RandomAccess.Read(_fileHandle, footerBytes.AsSpan(), currentOffset);
        var footer = BitConverter.ToUInt32(footerBytes);
        if (footer != Needle.FOOTER_MAGIC_NUMBER)
        {
            throw new InvalidDataException("Invalid needle footer");
        }

        // Construct needle
        var needle = new Needle
        {
            Header = header,
            Key = key,
            Flags = flags == 1,
            Size = size,
            Data = data,
            Checksum = checksum,
            Footer = footer
        };

        if (needle.Checksum != CalculateChecksum(needle))
        {
            throw new InvalidDataException("Checksum verification failed");
        }

        return needle;
    }

    private uint CalculateChecksum(Needle needle)
    {
        var checksum = needle.TotalBytes.Sum(b => b);

        checksum -= BitConverter.GetBytes(needle.Checksum).Sum(b => b);

        return (uint)checksum;
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
            offset += needle.TotalBytes.Length + needle.Padding.Length;
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