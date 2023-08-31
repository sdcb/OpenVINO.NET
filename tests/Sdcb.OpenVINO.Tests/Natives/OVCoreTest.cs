using Sdcb.OpenVINO.Natives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace Sdcb.OpenVINO.Tests.Natives;

using static NativeMethods;

public class OVCoreTest
{
    private readonly ITestOutputHelper _console;
    private readonly string _modelFile;

    public OVCoreTest(ITestOutputHelper console)
    {
        _console = console;

        string dir = "ppocrv3-det-cn";
        _modelFile = new FileInfo(Path.Combine(dir, "inference.pdmodel")).FullName;

        if (!File.Exists(_modelFile))
        {
            Assembly asm = Assembly.LoadFrom("Sdcb.PaddleOCR.Models.LocalV3.dll");
            WriteStreamToLocal(asm, "Sdcb.PaddleOCR.Models.LocalV3.models.ch_PP_OCRv3_det.inference.pdmodel", dir, "inference.pdmodel");
            WriteStreamToLocal(asm, "Sdcb.PaddleOCR.Models.LocalV3.models.ch_PP_OCRv3_det.inference.pdiparams", dir, "inference.pdiparams");
        }
    }

    private static void WriteStreamToLocal(Assembly asm, string resourceName, string localDir, string localFile)
    {
        Directory.CreateDirectory(localDir);
        using FileStream localFileStream = File.OpenWrite(Path.Combine(localDir, localFile));
        using Stream asmStream = asm.GetManifestResourceStream(resourceName)!;
        asmStream.CopyTo(localFileStream);
    }

    [Fact]
    public unsafe void OVCoreCanCreated()
    {
        ov_core* core = null;
        try
        {
            CheckResult(ov_core_create(&core));
        }
        finally
        {
            ov_core_free(core);
        }
    }

    [Fact]
    public unsafe void OVCoreCanLoadModel()
    {
        ov_core* core = null;
        ov_model* model = null;
        try
        {
            CheckResult(ov_core_create(&core));
            fixed (byte* modelPathPtr = Encoding.UTF8.GetBytes(_modelFile))
            {
                CheckResult(ov_core_read_model(core, modelPathPtr, null, &model));
            }
        }
        finally
        {
            if (model != null) ov_model_free(model);
            if (core != null) ov_core_free(core);
        }
    }

    [Fact]
    public unsafe void OVCoreCanLoadModelByUnicode()
    {
        if (Environment.OSVersion.Platform != PlatformID.Win32NT)
        {
            _console.WriteLine($"This test only available in windows.");
            return;
        }

        ov_core* core = null;
        ov_model* model = null;
        try
        {
            CheckResult(ov_core_create(&core));
            fixed (char* modelPathPtr = _modelFile)
            {
                CheckResult(ov_core_read_model_unicode(core, modelPathPtr, null, &model));
            }
        }
        finally
        {
            if (model != null) ov_model_free(model);
            if (core != null) ov_core_free(core);
        }
    }

    [Fact]
    public unsafe void OVCoreCanPrintFriendlyName()
    {
        ov_core* core = null;
        ov_model* model = null;
        try
        {
            CheckResult(ov_core_create(&core));
            fixed (byte* modelPathPtr = Encoding.UTF8.GetBytes(_modelFile))
            {
                CheckResult(ov_core_read_model(core, modelPathPtr, null, &model));

                byte* friendlyName;
                CheckResult(ov_model_get_friendly_name(model, &friendlyName));
                _console.WriteLine(Marshal.PtrToStringUTF8((IntPtr)friendlyName));
                ov_free(friendlyName);
            }
        }
        finally
        {
            if (model != null) ov_model_free(model);
            if (core != null) ov_core_free(core);
        }
    }

    [Fact]
    public unsafe void OVModelCanCreateShape()
    {
        ov_shape_t shape;
        try
        {
            long[] dims = new long[] { 1, 480, 640, 3 };
            fixed (long* dimsPtr = dims)
            {
                ov_shape_create(4, dimsPtr, &shape);
            }

            ReadOnlySpan<long> memory = new(shape.dims, 4);
            Assert.Equal(4, shape.rank);
            Assert.Equal(dims, memory.ToArray());
        }
        finally
        {
            ov_shape_free(&shape);
        }
    }

    [Fact]
    public unsafe void OVModelHaveIO()
    {
        ov_core* core = null;
        ov_model* model = null;
        ov_output_const_port* outputPort = null;
        ov_output_const_port* inputPort = null;
        try
        {
            CheckResult(ov_core_create(&core));
            fixed (byte* modelPathPtr = Encoding.UTF8.GetBytes(_modelFile))
            {
                CheckResult(ov_core_read_model(core, modelPathPtr, null, &model));

                CheckResult(ov_model_const_output(model, &outputPort));
                Assert.True(outputPort != null);

                CheckResult(ov_model_const_input(model, &inputPort));
                Assert.True(inputPort != null);
            }
        }
        finally
        {
            if (outputPort != null) ov_output_const_port_free(outputPort);
            if (inputPort != null) ov_output_const_port_free(inputPort);
            if (model != null) ov_model_free(model);
            if (core != null) ov_core_free(core);
        }
    }

    static void CheckResult(ov_status_e e)
    {
        Assert.Equal(ov_status_e.OK, e);
    }

    unsafe delegate void OVModelAction(ov_model* model);
}
