namespace HaystackStore;

public class Metadata
{
    public long Key { get; set; }
    public bool Flags { get; set; }
    public long Offset { get; set; }
    public int Size { get; set; }

    public override string ToString()
    {
        return $"Key = {Key}, Flags = {Flags}, Offset = {Offset}, Size = {Size}";
    }
}
