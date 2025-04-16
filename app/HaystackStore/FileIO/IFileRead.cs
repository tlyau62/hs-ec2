using Microsoft.Win32.SafeHandles;

namespace HaystackStore;

public interface IFileRead
{
    bool Exists(string filePath);

    byte[] ReadAllBytes(string filePath);
}

