using System;

namespace Sdcb.OpenVINO;

public readonly record struct NumStreamDef(int StreamCount)
{
    public static NumStreamDef Auto => new(-1);
    public static NumStreamDef Numa => new(-2);
    public static implicit operator NumStreamDef(int streamCount) => streamCount switch
    {
        < -2 => throw new ArgumentException("value must >= -2 or AUTO/NUMA", nameof(streamCount)),
        _ => new NumStreamDef(streamCount),
    };

    public static NumStreamDef Parse(string streamCount) => streamCount switch
    {
        "AUTO" => Auto,
        "NUMA" => Numa,
        _ => int.Parse(streamCount)
    };

    public override string ToString() => StreamCount switch
    {
        -1 => "AUTO",
        -2 => "NUMA",
        _ => StreamCount.ToString()
    };
}