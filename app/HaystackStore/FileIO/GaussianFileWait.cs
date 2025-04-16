using System.IO.Compression;
using System.Text.RegularExpressions;
using MathNet.Numerics.Distributions;

namespace HaystackStore;

public class GaussianFileWait : IFileWait
{
    private readonly double _mean; // in msec

    private readonly double _std;

    private readonly int _readsize;

    private readonly int _inodeSize;

    private readonly int _dirEntrySize;

    private readonly int _dirBlockSize;

    private readonly LogNormal _N;

    private bool _isEnabled = false;

    public GaussianFileWait(IConfiguration config)
    {
        _mean = config.GetValue<double?>("FileStat:MeanLatency") ??
            throw new InvalidDataException("missing config FileStat:MeanLatency");
        _std = config.GetValue<double?>("FileStat:StdLatency") ??
            throw new InvalidDataException("missing config FileStat:StdLatency");
        _readsize = config.GetValue<int?>("FileStat:Readsize") ??
            throw new InvalidDataException("missing config FileStat:Readsize");
        _inodeSize = config.GetValue<int?>("FileStat:InodeSize") ??
            throw new InvalidDataException("missing config FileStat:InodeSize");
        _dirEntrySize = config.GetValue<int?>("FileStat:DirectoryEntrySize") ??
            throw new InvalidDataException("missing config FileStat:DirectoryEntrySize");
        _dirBlockSize = config.GetValue<int?>("FileStat:DirectoryBlockSize") ??
            throw new InvalidDataException("missing config FileStat:DirectoryBlockSize");

        _N = ConvertNormalToLogNormal(_mean, _std);
    }

    public int Latency => _isEnabled ? (int)_N.Sample() : 0;

    public double ByteLatency => (double)Latency / _readsize;

    public void Enable()
    {
        _isEnabled = true;
    }

    public void Stop()
    {
        _isEnabled = false;
    }

    public void WaitBytesRead(int totalBytes)
    {
        var bytesLatency = ByteLatency * totalBytes;

        Thread.Sleep((int)bytesLatency);
    }

    public void WaitMetadataRead()
    {
        var totalMetadataSize = _inodeSize + _dirEntrySize + _dirBlockSize;

        WaitBytesRead(totalMetadataSize);
    }

    private LogNormal ConvertNormalToLogNormal(double mean, double stdDev)
    {
        double sigmaSquared = stdDev * stdDev;
        double muSquared = mean * mean;
        double varianceTerm = sigmaSquared / muSquared;
        double sigmaLogSquared = Math.Log(varianceTerm + 1);
        double sigmaLog = Math.Sqrt(sigmaLogSquared);
        double muLog = Math.Log(mean) - sigmaLogSquared / 2;

        return new LogNormal(muLog, sigmaLog);
    }
}

