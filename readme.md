# OpenVINO.NET ![NuGet](https://img.shields.io/nuget/dt/Sdcb.OpenVINO.svg?style=flat-square) [![QQ](https://img.shields.io/badge/QQ_Group-495782587-52B6EF?style=social&logo=tencent-qq&logoColor=000&logoWidth=20)](http://qm.qq.com/cgi-bin/qm/qr?_wv=1027&k=mma4msRKd372Z6dWpmBp4JZ9RL4Jrf8X&authKey=gccTx0h0RaH5b8B8jtuPJocU7MgFRUznqbV%2FLgsKdsK8RqZE%2BOhnETQ7nYVTp1W0&noverify=0&group_code=495782587)

High quality .NET wrapper for OpenVINO™ toolkit.

Please check my cnblogs blog for more details:
* 博客园 [20231015-sdcb-openvino-net](https://www.cnblogs.com/sdflysha/p/20231015-sdcb-openvino-net.html)
* PaddleOCR tutoria video:
  * How to use, run offline, GPU inference, performance tuning:
    * bilibili: https://www.bilibili.com/video/BV1bM411f74Z
    * youtube: https://www.youtube.com/watch?v=gG4iYgSbEXo
  * Deploy on different Linux platforms including OrangePI-4 LTS:
    * bilibili: https://www.bilibili.com/video/BV1R64y1L7Sv

## Packages

### Main packages

| Package                               | Version | Description                           |
| ------------------------------------- | ------- | ------------------------------------- |
| Sdcb.OpenVINO                         | 0.7.1   | .NET PInvoke interface                |
| Sdcb.OpenVINO.Extensions.OpenCvSharp4 | 0.7.0   | OpenVINO OpenCvSharp4 extensions      |
| Sdcb.OpenVINO.PaddleOCR               | 0.8.0   | OpenVINO Paddle OCR Toolkit           |
| Sdcb.OpenVINO.PaddleOCR.Models.Online | 0.8.0   | Online Models for OpenVINO Paddle OCR |

### Platform shared runtime packages

Current `Sdcb.OpenVINO` native runtime packages target OpenVINO 2026.2.0 only. On Linux/macOS, the loader only resolves the `2620` native library suffix, such as `libopenvino_c.so.2620` and `libopenvino_c.2620.dylib`.

| Package                                  | Version  | Description                           |
| ---------------------------------------- | -------- | ------------------------------------- |
| Sdcb.OpenVINO.runtime.android-x64        | 2026.2.0 | Runtime for Android x64               |
| Sdcb.OpenVINO.runtime.centos.8-x64       | 2026.2.0 | Runtime for CentOS 8 x64              |
| Sdcb.OpenVINO.runtime.linux-arm          | 2026.2.0 | Runtime for Debian 9+ ARM             |
| Sdcb.OpenVINO.runtime.osx.12.6-arm64     | 2026.2.0 | Runtime for macOS 12.6 ARM64          |
| Sdcb.OpenVINO.runtime.rhel.8-x64         | 2026.2.0 | Runtime for RHEL 8 x64                |
| Sdcb.OpenVINO.runtime.ubuntu.22.04-arm64 | 2026.2.0 | Runtime for Ubuntu 22.04 ARM64        |
| Sdcb.OpenVINO.runtime.ubuntu.22.04-x64   | 2026.2.0 | Runtime for Ubuntu 22.04 x64          |
| Sdcb.OpenVINO.runtime.ubuntu.24.04-x64   | 2026.2.0 | Runtime for Ubuntu 24.04 x64          |
| Sdcb.OpenVINO.runtime.win-mt-x64         | 2026.2.0 | Runtime for Windows x64 with VC MT runtime |
| Sdcb.OpenVINO.runtime.win-x64            | 2026.2.0 | Runtime for Windows x64               |

## Infer examples:

Install packages:
* Sdcb.OpenVINO
* A matching `Sdcb.OpenVINO.runtime.*` package from the table above
* Optional: `Sdcb.OpenVINO.Extensions.OpenCvSharp4`, `OpenCvSharp4`, and a matching OpenCvSharp4 runtime package if you need OpenCvSharp integration

For Windows x64, install either `Sdcb.OpenVINO.runtime.win-x64` or `Sdcb.OpenVINO.runtime.win-mt-x64`.

### Yolov8 models inference example:

* [Yolov8 detection model](https://github.com/sdcb/sdcb-openvino-yolov8-det)
* [Yolov8 classification model](https://github.com/sdcb/sdcb-openvino-yolov8-cls)

### Face detection example:
Please refer to [this project](https://github.com/sdcb/mini-openvino-facedetection)

### PaddleOCR example:
Please refer to [this project](https://github.com/sdcb/mini-openvino-paddleocr)

# Projects

## Sdcb.OpenVINO.PaddleOCR

### Packages

| Package                               | Version | Description                           |
| ------------------------------------- | ------- | ------------------------------------- |
| Sdcb.OpenVINO.PaddleOCR               | 0.8.0   | OpenVINO Paddle OCR Toolkit           |
| Sdcb.OpenVINO.PaddleOCR.Models.Online | 0.8.0   | Online Models for OpenVINO Paddle OCR |

### Usage

You can refer to this github for mini-openvino-paddleocr demo: https://github.com/sdcb/mini-openvino-paddleocr

Actually it's very similar to my another project [PaddleSharp](https://github.com/sdcb/PaddleSharp/blob/master/docs/ocr.md)

### PP-OCRv6 ONNX online models

`Sdcb.OpenVINO.PaddleOCR.Models.Online` provides PP-OCRv6 Chinese ONNX model presets:

| Model | Detection | Recognition | Text line 180-degree classifier | Document orientation classifier |
| ----- | --------- | ----------- | -------------------------------- | ------------------------------- |
| `OnlineFullModels.ChineseV6Medium` | `PP-OCRv6_medium_det_onnx_infer` | `PP-OCRv6_medium_rec_onnx_infer` | `PP-LCNet_x1_0_textline_ori_onnx_infer` | `PP-LCNet_x1_0_doc_ori_onnx_infer` |
| `OnlineFullModels.ChineseV6Small` | `PP-OCRv6_small_det_onnx_infer` | `PP-OCRv6_small_rec_onnx_infer` | `PP-LCNet_x0_25_textline_ori_onnx_infer` | `PP-LCNet_x1_0_doc_ori_onnx_infer` |
| `OnlineFullModels.ChineseV6Tiny` | `PP-OCRv6_tiny_det_onnx_infer` | `PP-OCRv6_tiny_rec_onnx_infer` | `PP-LCNet_x0_25_textline_ori_onnx_infer` | `PP-LCNet_x1_0_doc_ori_onnx_infer` |

```csharp
using OpenCvSharp;
using Sdcb.OpenVINO;
using Sdcb.OpenVINO.PaddleOCR;
using Sdcb.OpenVINO.PaddleOCR.Models;
using Sdcb.OpenVINO.PaddleOCR.Models.Online;

FullOcrModel model = await OnlineFullModels.ChineseV6Small.DownloadAsync();

using PaddleOcrAll ocr = new(model, new PaddleOcrOptions(new DeviceOptions("CPU")))
{
    AllowRotateDetection = true,
    Enable180Classification = true,

    // Disabled by default. Enable it only when the input may be a full page
    // rotated by 0, 90, 180 or 270 degrees.
    EnableDocumentOrientationClassification = true,
};

using Mat src = Cv2.ImRead("test.jpg");
PaddleOcrResult result = ocr.Run(src);
Console.WriteLine(result.Text);
```

Notes:

* PP-OCRv6 recognition labels are loaded from the downloaded model `inference.yml`.
* PP-OCRv6 detection uses the official DB postprocess defaults: `BoxThreshold = 0.2`, `BoxScoreThreahold = 0.45`, `UnclipRatio = 1.4`. These properties can still be overwritten after constructing `PaddleOcrDetector`.
* Full-document orientation classification is not enabled by default, even when the selected `OnlineFullModels` preset includes the document orientation model.

The detector, text line classifier, recognizer, and document orientation classifier can also be used independently:

```csharp
using OpenCvSharp;
using Sdcb.OpenVINO;
using Sdcb.OpenVINO.PaddleOCR;
using Sdcb.OpenVINO.PaddleOCR.Models.Online;

using Mat src = Cv2.ImRead("document.jpg");

using PaddleOcrDocumentOrientationClassifier docOrientation = new(
    await OnlineDocumentOrientationClassificationModel.PPDocOrientationX10.DownloadAsync(),
    new DeviceOptions("CPU"));
PaddleOcrDocumentOrientationResult orientation = docOrientation.Run(src);
using Mat upright = orientation.RotateToUpright(src);

using PaddleOcrClassifier textLineOrientation = new(
    await OnlineOnnxClassificationModel.TextLineOrientationX025.DownloadAsync(),
    new DeviceOptions("CPU"));
Ocr180DegreeClsResult lineResult = textLineOrientation.ShouldRotate180(upright);
```

# LICENSE

[Apache](./LICENSE.txt)
