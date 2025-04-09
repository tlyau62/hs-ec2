namespace HaystackStore;

public interface ISuperblock
{
    /**
     * return offset
     */
    int AppendNeedle(INeedle needle);
    INeedle ReadNeedle(long offset);
}

