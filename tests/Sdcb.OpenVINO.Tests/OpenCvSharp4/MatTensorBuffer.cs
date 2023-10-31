using OpenCvSharp;

namespace Sdcb.OpenVINO.Tests.OpenCvSharp4;

internal class MatTensorBuffer : TensorBuffer
{
    private Mat _mat;

    public MatTensorBuffer(Mat matSrc)
    {
        if (matSrc.IsContinuous())
        {
            _mat = matSrc[0, matSrc.Width, 0, matSrc.Height];
        }
        else
        {
            _mat = matSrc.Clone(); // create ROI
        }
    }

    protected override IntPtr GetDataPointer()
    {
        return _mat.Data;
    }

    protected override int GetDataByteLength()
    {
        return (int)(_mat.DataEnd - _mat.DataStart);
    }

    protected override void ReleaseUnmanagedData()
    {
        if (_mat != null)
        {
            _mat.Dispose();
            _mat = null!;
        }
    }

    public override string ToString()
    {
        return "<MatTensorBuffer " + "Width: " + _mat.Width + " Height: " + _mat.Height + ">";
    }
}