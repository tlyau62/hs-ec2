# CS5296 Projects

## Components

### The Store

Class Diagram

```mermaid
classDiagram
    class HaystackStore {
        -volumeId: string
        -volumeFile: File
        -superblock: Superblock
        -inMemoryIndex: Map<NeedleId, NeedleMetadata>
        +writePhoto(photoId, photoData): NeedleId
        +readPhoto(needleId): PhotoData
        +deletePhoto(needleId): boolean
        -appendNeedle(needle): void
        -readNeedle(offset, size): Needle
    }

    class Superblock {
        -magicNumber: number
        -version: number
        -needleCount: number
        -dataSize: number
        -checksum: number
    }

    class Needle {
        +header: NeedleHeader
        +data: PhotoData
        +footer: NeedleFooter
    }

    class NeedleHeader {
        -magicNumber: number
        -cookie: number
        -key: NeedleId
        -dataSize: number
        -flags: number
    }

    class NeedleFooter {
        -magicNumber: number
        -checksum: number
    }

    class NeedleMetadata {
        +offset: number
        +size: number
        +flags: number
        +cookie: number
    }

    HaystackStore "1" --> "1" Superblock
    HaystackStore "1" --> "*" NeedleMetadata
    HaystackStore "1" --> "*" Needle
    Needle "1" --> "1" NeedleHeader
    Needle "1" --> "1" NeedleFooter
```

Key points:

- The HaystackStore class manages the entire volume file
- Photos are stored as "Needles" within the volume file
- All metadata is kept in memory (inMemoryIndex)
- Superblock contains volume-level metadata
- Each Needle has header, photo data, and footer

Activity Diagram for Photo Read Operation

```mermaid
flowchart TD
    A[Start Read Request] --> B{Needle ID in\nmemory index?}
    B -->|Yes| C[Get offset/size from index]
    B -->|No| D[Return Photo Not Found]
    C --> E[Seek to offset in volume file]
    E --> F[Read needle header]
    F --> G[Verify magic number/cookie]
    G --> H[Read photo data]
    H --> I[Read needle footer]
    I --> J[Verify checksum]
    J --> K[Return photo data]
    G -->|Invalid| L[Return Corrupt Data Error]
    J -->|Invalid| L
```

Component Diagram for Stage 1 Testing

```mermaid
flowchart TD
    subgraph EC2 Instance
        A[Test Client] --> B[Haystack Store]
        A --> C[NFS Client]
        B --> D[EBS Volume\nHaystack Format]
        C --> E[EBS Volume\nTraditional FS]
    end

    F[Photo Dataset] --> A
    A --> G[Metrics Collector]
    G --> H[Performance Report]
```

Sequence Diagram for Photo Write

```mermaid
sequenceDiagram
    participant Client
    participant HaystackStore
    participant VolumeFile
    participant MemoryIndex

    Client->>HaystackStore: writePhoto(photoId, data)
    HaystackStore->>VolumeFile: Append needle (header+data+footer)
    VolumeFile-->>HaystackStore: New file offset
    HaystackStore->>MemoryIndex: Store metadata (offset, size, etc)
    HaystackStore-->>Client: Return needleId
```

Keypoints:

1. Implementation Recommendations for TypeScript:
   For the volume file operations, consider using Node.js fs module with async APIs
2. Use a Map object for the in-memory index (O(1) lookups)
3. Implement proper file locking for concurrent access
4. Consider using Protocol Buffers or similar for binary serialization of needles
5. For performance testing against NFS, you might want to:
   - Create a test harness that generates synthetic photo requests
   - Measure operations per second, latency distributions
   - Track memory usage and I/O patterns
