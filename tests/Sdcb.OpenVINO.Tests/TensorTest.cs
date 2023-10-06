using Sdcb.OpenVINO.Natives;

namespace Sdcb.OpenVINO.Tests;

public class TensorTest
{
    [Fact]
    public void CreateFromFloatArray()
    {
        using Tensor tensor = Tensor.FromArray(new float[960 * 960 * 3], new Shape(960, 960, 3));
        Assert.Equal(ov_element_type_e.F32, tensor.ElementType);
    }

    [Fact]
    public void CreateFromByteArray()
    {
        using Tensor tensor = Tensor.FromArray(new byte[960 * 960 * 3], new Shape(960, 960, 3));
        Assert.Equal(ov_element_type_e.U8, tensor.ElementType);
    }

    [Fact]
    public void CreateFromSpan()
    {
        using Tensor tensor = Tensor.FromRaw(new byte[960 * 960 * 3 * 4], new Shape(960, 960, 3), ov_element_type_e.F32);
    }

    [Fact]
    public void VerifyDataTest()
    {
        byte[] data = new byte[] { 1, 2, 3, 4, 5 }; 
        using Tensor tensor = Tensor.FromRaw(data, new Shape(5));
        Span<byte> result = tensor.GetData<byte>();
        Assert.Equal(data, result.ToArray());
    }

    [Fact]
    public void VerifyDataLength()
    {
        float[] data = new float[] { 1, 2, 3, 4, 5 };
        using Tensor tensor = Tensor.FromArray(data, new Shape(5));
        Span<byte> result = tensor.GetData<byte>();
        Assert.Equal(data.Length * 4, result.Length);
    }
}
