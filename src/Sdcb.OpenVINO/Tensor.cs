using System;
using System.Collections.Generic;
using System.Text;

namespace Sdcb.OpenVINO;

public class Tensor : NativeResource
{
    public Tensor(IntPtr handle, bool owned = true) : base(handle, owned)
    {
    }

    protected override void ReleaseHandle(IntPtr handle)
    {
        throw new NotImplementedException();
    }
}
