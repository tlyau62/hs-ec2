namespace HaystackStore;

public interface IStoreService
{
    byte[]? ReadPhoto(int key);
    void WritePhoto(int key, byte[] data);

    /**
     * data is a zip file containing photos.
     * The filename of a photo is used as the key extracted by regex keyPattern.
     */
    void UnpackPhotos(string keyPattern, byte[] data);
}

