using Sdcb.OpenVINO.Natives;
using System;

namespace Sdcb.OpenVINO;

using static NativeMethods;

/// <summary>
/// A static class that provides access to a shared instance of <see cref="OVCore"/>.
/// </summary>
public static class SharedOVCore
{
    private static Lazy<IntPtr> _core = new(CreateSharedHandle);

    /// <summary>
    /// Gets the instance of <see cref="OVCore"/>.
    /// </summary>
    public unsafe static OVCore Instance => new((ov_core*)_core.Value, owned: false);

    private unsafe static IntPtr CreateSharedHandle()
    {
        ov_core* core;
        OpenVINOException.ThrowIfFailed(ov_core_create(&core));
        return (IntPtr)core;
    }

    /// <summary>
    /// Disposes the shared handle.
    /// </summary>
    public unsafe static void DisposeSharedHandle()
    {
        if (_core.IsValueCreated)
        {
            ov_core* core = (ov_core*)_core.Value;
            ov_core_free(core);
            _core = new Lazy<IntPtr>(CreateSharedHandle);
        }
    }
}
