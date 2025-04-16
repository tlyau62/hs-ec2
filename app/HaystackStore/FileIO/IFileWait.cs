namespace HaystackStore;

public interface IFileWait
{
    /**
     * Wait for contiguous bytes read of a file
     */
    void WaitBytesRead(int totalBytes);

    /**
     * Wait for locating a file bytes on a disk
     */
    void WaitMetadataRead();

    void Enable();

    void Stop();
}

