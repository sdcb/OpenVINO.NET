using OpenCvSharp;
using Sdcb.OpenVINO.Natives;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Xunit.Abstractions;

namespace Sdcb.OpenVINO.Tests.Natives;

using static NativeMethods;

public class OVCoreNativeTest
{
    private readonly ITestOutputHelper _console;
    private readonly string _modelFile;

    public OVCoreNativeTest(ITestOutputHelper console)
    {
        _console = console;
        _modelFile = PrepareModel();
    }

    internal static string PrepareModel(string dir = "ppocrv3-det-cn")
    {
        string modelFile = new FileInfo(Path.Combine(dir, "inference.pdmodel")).FullName;

        if (!File.Exists(modelFile))
        {
            Assembly asm = Assembly.LoadFrom("Sdcb.PaddleOCR.Models.LocalV3.dll");
            WriteStreamToLocal(asm, "Sdcb.PaddleOCR.Models.LocalV3.models.ch_PP_OCRv3_det.inference.pdmodel", dir, "inference.pdmodel");
            WriteStreamToLocal(asm, "Sdcb.PaddleOCR.Models.LocalV3.models.ch_PP_OCRv3_det.inference.pdiparams", dir, "inference.pdiparams");
        }
        return modelFile;
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
            Check(ov_core_create(&core));
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
            Check(ov_core_create(&core));
            fixed (byte* modelPathPtr = Encoding.UTF8.GetBytes(_modelFile))
            {
                Check(ov_core_read_model(core, modelPathPtr, null, &model));
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
            Check(ov_core_create(&core));
            fixed (char* modelPathPtr = _modelFile)
            {
                Check(ov_core_read_model_unicode(core, modelPathPtr, null, &model));
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
            Check(ov_core_create(&core));
            fixed (byte* modelPathPtr = Encoding.UTF8.GetBytes(_modelFile))
            {
                Check(ov_core_read_model(core, modelPathPtr, null, &model));

                byte* friendlyName;
                Check(ov_model_get_friendly_name(model, &friendlyName));
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
            long* dims = stackalloc long[4] { 1, 480, 640, 3 };
            Check(ov_shape_create(4, dims, &shape));

            ReadOnlySpan<long> memory = new(shape.dims, 4);
            Assert.Equal(4, shape.rank);
            Assert.Equal(new long[] { 1, 480, 640, 3 }, memory.ToArray());
        }
        finally
        {
            ov_shape_free(&shape);
        }
    }

    [Fact]
    public unsafe void OVPortTest()
    {
        ov_core* core = null;
        ov_model* model = null;
        ov_output_const_port* outputPort = null;
        ov_output_const_port* inputPort = null;
        try
        {
            Check(ov_core_create(&core));
            fixed (byte* modelPathPtr = Encoding.UTF8.GetBytes(_modelFile))
            {
                Check(ov_core_read_model(core, modelPathPtr, null, &model));

                Check(ov_model_const_output(model, &outputPort));
                Assert.True(outputPort != null);

                Check(ov_model_const_input(model, &inputPort));
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

    [Fact]
    public unsafe void TensorTest()
    {
        ov_shape_t shape = default;
        ov_tensor* tensor = null;
        try
        {
            using Mat mat = Cv2.ImRead(@"./assets/text.png");
            long* dims = stackalloc long[4] { 1, mat.Height, mat.Width, 3 };
            Check(ov_shape_create(4, dims, &shape));
            Check(ov_tensor_create_from_host_ptr(ov_element_type_e.U8, shape, (void*)mat.Data, &tensor));

            nint byteSize;
            Check(ov_tensor_get_byte_size(tensor, &byteSize));
            Assert.Equal(mat.Width * mat.Height * 3, byteSize);

            ov_element_type_e elementType;
            Check(ov_tensor_get_element_type(tensor, &elementType));
            Assert.Equal(ov_element_type_e.U8, elementType);

            ov_shape_t tensorShape;
            Check(ov_tensor_get_shape(tensor, &tensorShape));
            Assert.Equal(new long[] { 1, mat.Height, mat.Width, 3 }, new ReadOnlySpan<long>(tensorShape.dims, 4).ToArray());
            ov_shape_free(&tensorShape);

            nint elementSize;
            Check(ov_tensor_get_size(tensor, &elementSize));
            Assert.Equal(mat.Width * mat.Height * 3, elementSize);
        }
        finally
        {
            if (tensor != null) ov_tensor_free(tensor);
            if (shape.dims != null) ov_shape_free(&shape);
        }
    }

    [Fact]
    public unsafe void PreprocessorIO()
    {
        ov_core* core = null;
        ov_model* model = null;
        ov_output_const_port* outputPort = null;
        ov_output_const_port* inputPort = null;
        ov_shape_t shape = default;
        ov_tensor* tensor = null;
        ov_preprocess_prepostprocessor* preprocessor = null;

        try
        {
            Check(ov_core_create(&core));
            fixed (byte* modelPathPtr = Encoding.UTF8.GetBytes(_modelFile))
            {
                Check(ov_core_read_model(core, modelPathPtr, null, &model));
                Check(ov_model_const_output(model, &outputPort));
                Check(ov_model_const_input(model, &inputPort));
                using Mat mat = Cv2.ImRead(@"./assets/text.png");
                long* dims = stackalloc long[4] { 1, mat.Height, mat.Width, 3 };
                Check(ov_shape_create(4, dims, &shape));
                Check(ov_tensor_create_from_host_ptr(ov_element_type_e.U8, shape, (void*)mat.Data, &tensor));

                Check(ov_preprocess_prepostprocessor_create(model, &preprocessor));
                Assert.True(preprocessor != null);
            }
        }
        finally
        {
            if (preprocessor != null) ov_preprocess_prepostprocessor_free(preprocessor);
            if (tensor != null) ov_tensor_free(tensor);
            if (shape.dims != null) ov_shape_free(&shape);
            if (outputPort != null) ov_output_const_port_free(outputPort);
            if (inputPort != null) ov_output_const_port_free(inputPort);
            if (model != null) ov_model_free(model);
            if (core != null) ov_core_free(core);
        }
    }

    [Fact]
    public unsafe void InputInfo()
    {
        ov_core* core = null;
        ov_model* model = null;
        ov_output_const_port* outputPort = null;
        ov_output_const_port* inputPort = null;
        ov_shape_t shape = default;
        ov_tensor* tensor = null;
        ov_preprocess_prepostprocessor* preprocessor = null;
        ov_preprocess_input_info* inputInfo = null;

        try
        {
            Check(ov_core_create(&core));
            fixed (byte* modelPathPtr = Encoding.UTF8.GetBytes(_modelFile + '\0'))
            {
                Check(ov_core_read_model(core, modelPathPtr, null, &model));
                Check(ov_model_const_output(model, &outputPort));
                Check(ov_model_const_input(model, &inputPort));
                using Mat mat = Cv2.ImRead(@"./assets/text.png");
                long* dims = stackalloc long[4] { 1, mat.Height, mat.Width, 3 };
                Check(ov_shape_create(4, dims, &shape));
                Check(ov_tensor_create_from_host_ptr(ov_element_type_e.U8, shape, (void*)mat.Data, &tensor));
                Check(ov_preprocess_prepostprocessor_create(model, &preprocessor));

                Check(ov_preprocess_prepostprocessor_get_input_info_by_index(preprocessor, 0, &inputInfo));
                Assert.True(inputInfo != null);
            }
        }
        finally
        {
            if (inputInfo != null) ov_preprocess_input_info_free(inputInfo);
            if (preprocessor != null) ov_preprocess_prepostprocessor_free(preprocessor);
            if (tensor != null) ov_tensor_free(tensor);
            if (shape.dims != null) ov_shape_free(&shape);
            if (outputPort != null) ov_output_const_port_free(outputPort);
            if (inputPort != null) ov_output_const_port_free(inputPort);
            if (model != null) ov_model_free(model);
            if (core != null) ov_core_free(core);
        }
    }

    [Fact]
    public unsafe void TensorInfo()
    {
        ov_core* core = null;
        ov_model* model = null;
        ov_output_const_port* outputPort = null;
        ov_output_const_port* inputPort = null;
        ov_shape_t shape = default;
        ov_tensor* tensor = null;
        ov_preprocess_prepostprocessor* preprocessor = null;
        ov_preprocess_input_info* inputInfo = null;
        ov_preprocess_input_tensor_info* inputTensorInfo = null;

        try
        {
            Check(ov_core_create(&core));
            fixed (byte* modelPathPtr = Encoding.UTF8.GetBytes(_modelFile))
            {
                Check(ov_core_read_model(core, modelPathPtr, null, &model));
                Check(ov_model_const_output(model, &outputPort));
                Check(ov_model_const_input(model, &inputPort));
                using Mat mat = Cv2.ImRead(@"./assets/text.png");
                long* dims = stackalloc long[4] { 1, mat.Height, mat.Width, 3 };
                Check(ov_shape_create(4, dims, &shape));
                Check(ov_tensor_create_from_host_ptr(ov_element_type_e.U8, shape, (void*)mat.Data, &tensor));
                Check(ov_preprocess_prepostprocessor_create(model, &preprocessor));
                Check(ov_preprocess_prepostprocessor_get_input_info_by_index(preprocessor, 0, &inputInfo));

                Check(ov_preprocess_input_info_get_tensor_info(inputInfo, &inputTensorInfo));
                Assert.True(inputTensorInfo != null);

                Check(ov_preprocess_input_tensor_info_set_from(inputTensorInfo, tensor));
            }
        }
        finally
        {
            if (inputTensorInfo != null) ov_preprocess_input_tensor_info_free(inputTensorInfo);
            if (inputInfo != null) ov_preprocess_input_info_free(inputInfo);
            if (preprocessor != null) ov_preprocess_prepostprocessor_free(preprocessor);
            if (tensor != null) ov_tensor_free(tensor);
            if (shape.dims != null) ov_shape_free(&shape);
            if (outputPort != null) ov_output_const_port_free(outputPort);
            if (inputPort != null) ov_output_const_port_free(inputPort);
            if (model != null) ov_model_free(model);
            if (core != null) ov_core_free(core);
        }
    }

    [Fact]
    public unsafe void LayoutTest()
    {
        ov_layout* layout = null;
        try
        {
            byte* layoutDesc = stackalloc byte[4] { (byte)'N', (byte)'H', (byte)'W', (byte)'C' };
            Check(ov_layout_create(layoutDesc, &layout));
            Assert.True(layout != null);

            byte* layoutStrPtr = ov_layout_to_string(layout);
            Assert.True(layoutStrPtr != null);
            string layoutStr = Marshal.PtrToStringAnsi((IntPtr)layoutStrPtr)!;
            Assert.Equal("[N,H,W,C]", layoutStr);
            ov_free(layoutStrPtr);
        }
        finally
        {
            if (layout != null) ov_layout_free(layout);
        }
    }

    [Fact]
    public unsafe void SetLayout()
    {
        ov_core* core = null;
        ov_model* model = null;
        ov_output_const_port* outputPort = null;
        ov_output_const_port* inputPort = null;
        ov_shape_t shape = default;
        ov_tensor* tensor = null;
        ov_preprocess_prepostprocessor* preprocessor = null;
        ov_preprocess_input_info* inputInfo = null;
        ov_preprocess_input_tensor_info* inputTensorInfo = null;
        ov_layout* inputLayout = null;

        try
        {
            Check(ov_core_create(&core));
            fixed (byte* modelPathPtr = Encoding.UTF8.GetBytes(_modelFile))
            {
                Check(ov_core_read_model(core, modelPathPtr, null, &model));
                Check(ov_model_const_output(model, &outputPort));
                Check(ov_model_const_input(model, &inputPort));
                using Mat mat = Cv2.ImRead(@"./assets/text.png");
                long* dims = stackalloc long[4] { 1, mat.Height, mat.Width, 3 };
                Check(ov_shape_create(4, dims, &shape));
                Check(ov_tensor_create_from_host_ptr(ov_element_type_e.U8, shape, (void*)mat.Data, &tensor));
                Check(ov_preprocess_prepostprocessor_create(model, &preprocessor));
                Check(ov_preprocess_prepostprocessor_get_input_info_by_index(preprocessor, 0, &inputInfo));
                Check(ov_preprocess_input_info_get_tensor_info(inputInfo, &inputTensorInfo));
                Check(ov_preprocess_input_tensor_info_set_from(inputTensorInfo, tensor));
                byte* layoutDesc = stackalloc byte[4] { (byte)'N', (byte)'H', (byte)'W', (byte)'C' };
                Check(ov_layout_create(layoutDesc, &inputLayout));
                Check(ov_preprocess_input_tensor_info_set_layout(inputTensorInfo, inputLayout));
            }
        }
        finally
        {
            if (inputTensorInfo != null) ov_preprocess_input_tensor_info_free(inputTensorInfo);
            if (inputInfo != null) ov_preprocess_input_info_free(inputInfo);
            if (preprocessor != null) ov_preprocess_prepostprocessor_free(preprocessor);
            if (tensor != null) ov_tensor_free(tensor);
            if (shape.dims != null) ov_shape_free(&shape);
            if (outputPort != null) ov_output_const_port_free(outputPort);
            if (inputPort != null) ov_output_const_port_free(inputPort);
            if (model != null) ov_model_free(model);
            if (core != null) ov_core_free(core);
        }
    }

    [Fact]
    public unsafe void PreprocessSteps()
    {
        ov_core* core = null;
        ov_model* model = null;
        ov_output_const_port* outputPort = null;
        ov_output_const_port* inputPort = null;
        ov_shape_t shape = default;
        ov_tensor* tensor = null;
        ov_preprocess_prepostprocessor* preprocessor = null;
        ov_preprocess_input_info* inputInfo = null;
        ov_preprocess_input_tensor_info* inputTensorInfo = null;
        ov_layout* inputLayout = null;
        ov_preprocess_preprocess_steps* preprocessSteps = null;
        ov_preprocess_input_model_info* modelInfo;
        ov_layout* modelLayout = null;
        ov_model* newModel = null;
        ov_compiled_model* compiledModel = null;
        ov_infer_request* inferRequest = null;
        ov_tensor* outputTensor = null;
        ov_preprocess_output_info* outputInfo = null;
        ov_preprocess_output_tensor_info* outputTensorInfo = null;

        try
        {
            Check(ov_core_create(&core));
            fixed (byte* modelPathPtr = Encoding.UTF8.GetBytes(_modelFile))
            {
                Check(ov_core_read_model(core, modelPathPtr, null, &model));
                Check(ov_model_const_output(model, &outputPort));
                Check(ov_model_const_input(model, &inputPort));
                using Mat mat = Cv2.ImRead(@"./assets/text.png");
                Cv2.CopyMakeBorder(mat, mat, 0, 960 - mat.Rows, 0, 960 - mat.Cols, BorderTypes.Constant, Scalar.Black);
                mat.ConvertTo(mat, MatType.CV_32FC3, 1.0 / 255);
                //using Mat normalized = Normalize(mat);
                //float[] floatData = ExtractMat(normalized);
                long* dims = stackalloc long[4] { 1, mat.Height, mat.Width, 3 };
                Check(ov_shape_create(4, dims, &shape));
                Check(ov_tensor_create_from_host_ptr(ov_element_type_e.F32, shape, (void*)mat.Data, &tensor));
                Check(ov_preprocess_prepostprocessor_create(model, &preprocessor));
                Check(ov_preprocess_prepostprocessor_get_input_info_by_index(preprocessor, 0, &inputInfo));

                Check(ov_preprocess_input_info_get_tensor_info(inputInfo, &inputTensorInfo));
                Check(ov_preprocess_input_tensor_info_set_from(inputTensorInfo, tensor));
                byte* inputLayoutDesc = stackalloc byte[4] { (byte)'N', (byte)'H', (byte)'W', (byte)'C' };
                Check(ov_layout_create(inputLayoutDesc, &inputLayout));
                Check(ov_preprocess_input_tensor_info_set_layout(inputTensorInfo, inputLayout));

                Check(ov_preprocess_input_info_get_preprocess_steps(inputInfo, &preprocessSteps));

                //Check(ov_preprocess_preprocess_steps_resize(preprocessSteps, ov_preprocess_resize_algorithm_e.RESIZE_LINEAR));
                Check(ov_preprocess_input_info_get_model_info(inputInfo, &modelInfo));

                byte* modelLayoutDesc = stackalloc byte[4] { (byte)'N', (byte)'C', (byte)'H', (byte)'W' };
                Check(ov_layout_create(modelLayoutDesc, &modelLayout));
                Check(ov_preprocess_input_model_info_set_layout(modelInfo, modelLayout));

                Check(ov_preprocess_prepostprocessor_get_output_info_by_index(preprocessor, 0, &outputInfo));
                Check(ov_preprocess_output_info_get_tensor_info(outputInfo, &outputTensorInfo));
                Check(ov_preprocess_output_set_element_type(outputTensorInfo, ov_element_type_e.F32));

                Check(ov_preprocess_prepostprocessor_build(preprocessor, &newModel));

                fixed (byte* deviceName = Encoding.UTF8.GetBytes("CPU"))
                {
                    Check(ov_core_compile_model(core, newModel, deviceName, 0, &compiledModel));
                }

                Check(ov_compiled_model_create_infer_request(compiledModel, &inferRequest));
                Check(ov_infer_request_set_input_tensor_by_index(inferRequest, 0, tensor));
                Check(ov_infer_request_infer(inferRequest));
                Check(ov_infer_request_get_output_tensor_by_index(inferRequest, 0, &outputTensor));
                void* data;
                Check(ov_tensor_data(outputTensor, &data));
                nint dataSize;
                Check(ov_tensor_get_byte_size(outputTensor, &dataSize));
                using Mat result = new(960, 960, MatType.CV_32FC1, (IntPtr)data);
                result.ConvertTo(result, MatType.CV_8SC1, 255);
            }
        }
        finally
        {
            if (outputTensorInfo != null) ov_preprocess_output_tensor_info_free(outputTensorInfo);
            if (outputInfo != null) ov_preprocess_output_info_free(outputInfo);
            if (modelLayout != null) ov_layout_free(modelLayout);
            if (inputTensorInfo != null) ov_preprocess_input_tensor_info_free(inputTensorInfo);
            if (inputInfo != null) ov_preprocess_input_info_free(inputInfo);
            if (preprocessor != null) ov_preprocess_prepostprocessor_free(preprocessor);
            if (tensor != null) ov_tensor_free(tensor);
            if (shape.dims != null) ov_shape_free(&shape);
            if (outputPort != null) ov_output_const_port_free(outputPort);
            if (inputPort != null) ov_output_const_port_free(inputPort);
            if (model != null) ov_model_free(model);
            if (core != null) ov_core_free(core);
        }
    }

    unsafe static void Check(ov_status_e e, [CallerArgumentExpression(nameof(e))] string? expr = null)
    {
        if (e != ov_status_e.OK)
        {
            byte* errorInfoPtr = ov_get_error_info(e);
            string? errorInfo = Marshal.PtrToStringAnsi((IntPtr)errorInfoPtr);
            Assert.Fail($"{e}({errorInfo})\nSource: {expr}");
        }
    }

    unsafe delegate void OVModelAction(ov_model* model);
}
