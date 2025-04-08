namespace HaystackStore;

public interface IStoreService
{
    byte[] ReadPhoto(int key);
    void WritePhoto(int key, byte[] data);
}

