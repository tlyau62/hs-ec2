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

    /**
     * Same as UnpackPhotos, but the zip is loaded from the existing location in the system.
     * This is for loading a photo zip uploaded directly from EBS
     * Location is relative to the mountFolder
     */
    void LoadPhotos(string keyPattern, string location);
}

