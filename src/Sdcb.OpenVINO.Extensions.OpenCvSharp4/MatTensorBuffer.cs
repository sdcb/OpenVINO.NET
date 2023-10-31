using OpenCvSharp;
using Sdcb.OpenVINO.Natives;
using System;

namespace Sdcb.OpenVINO.Extensions.OpenCvSharp4;

internal class MatTensorBuffer : TensorBuffer
{
    private Mat _mat;

    public MatTensorBuffer(Mat matSrc) : base(GetElementType(matSrc))
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

    private static ov_element_type_e GetElementType(Mat src)
    {
        MatType matType = src.Type();
        return matType.Depth switch
        {
            MatType.CV_8U => ov_element_type_e.U8,
            MatType.CV_8S => ov_element_type_e.I8,
            MatType.CV_16U => ov_element_type_e.U16,
            MatType.CV_16S => ov_element_type_e.I16,
            MatType.CV_32S => ov_element_type_e.I32,
            MatType.CV_32F => ov_element_type_e.F32,
            MatType.CV_64F => ov_element_type_e.F64,
            _ => throw new NotSupportedException($"Mat.MatType.Depth ({matType.Depth}) is not supported.")
        };
    }

    protected override IntPtr GetDataPointer()
    {
        return _mat.Data;
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