namespace HaystackStore;

public interface ISuperblock
{
    /**
     * return offset
     */
    long AppendNeedle(INeedle needle);
    INeedle ReadNeedle(long offset);
    INeedle CreateNeedle(long key, byte[] data);
}

