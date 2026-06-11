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

### Core packages

| Package                                | Version 📌                                                                                                                                                | Description                  |
| -------------------------------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------- | ---------------------------- |
| Sdcb.OpenVINO                          | [![NuGet](https://img.shields.io/nuget/v/Sdcb.OpenVINO.svg)](https://nuget.org/packages/Sdcb.OpenVINO)                                                   | .NET PInvoke interface       |
| Sdcb.OpenVINO.Extensions.OpenCvSharp4  | [![NuGet](https://img.shields.io/nuget/v/Sdcb.OpenVINO.Extensions.OpenCvSharp4.svg)](https://nuget.org/packages/Sdcb.OpenVINO.Extensions.OpenCvSharp4)   | OpenVINO OpenCvSharp4 extensions   |

### Platform shared runtime packages

Current `Sdcb.OpenVINO` native runtime packages target OpenVINO 2026.2.0 only. On Linux/macOS, the loader only resolves the `2620` native library suffix, such as `libopenvino_c.so.2620` and `libopenvino_c.2620.dylib`.

| Package                                | Version 📌                                                                                                                                                | Description                  |
| -------------------------------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------- | ---------------------------- |
| Sdcb.OpenVINO.runtime.centos.8-x64     | [![NuGet](https://img.shields.io/nuget/v/Sdcb.OpenVINO.runtime.centos.8-x64.svg)](https://nuget.org/packages/Sdcb.OpenVINO.runtime.centos.8-x64)         | Runtime for CentOS 8 x64     |
| Sdcb.OpenVINO.runtime.linux-arm        | [![NuGet](https://img.shields.io/nuget/v/Sdcb.OpenVINO.runtime.linux-arm.svg)](https://nuget.org/packages/Sdcb.OpenVINO.runtime.linux-arm)               | Runtime for Debian 9+ ARM    |
| Sdcb.OpenVINO.runtime.rhel.8-x64       | [![NuGet](https://img.shields.io/nuget/v/Sdcb.OpenVINO.runtime.rhel.8-x64.svg)](https://nuget.org/packages/Sdcb.OpenVINO.runtime.rhel.8-x64)             | Runtime for RHEL 8 x64       |
| Sdcb.OpenVINO.runtime.ubuntu.22.04-arm64 | [![NuGet](https://img.shields.io/nuget/v/Sdcb.OpenVINO.runtime.ubuntu.22.04-arm64.svg)](https://nuget.org/packages/Sdcb.OpenVINO.runtime.ubuntu.22.04-arm64) | Runtime for Ubuntu 22.04 ARM64 |
| Sdcb.OpenVINO.runtime.ubuntu.22.04-x64 | [![NuGet](https://img.shields.io/nuget/v/Sdcb.OpenVINO.runtime.ubuntu.22.04-x64.svg)](https://nuget.org/packages/Sdcb.OpenVINO.runtime.ubuntu.22.04-x64) | Runtime for Ubuntu 22.04 x64 |
| Sdcb.OpenVINO.runtime.ubuntu.24.04-x64 | [![NuGet](https://img.shields.io/nuget/v/Sdcb.OpenVINO.runtime.ubuntu.24.04-x64.svg)](https://nuget.org/packages/Sdcb.OpenVINO.runtime.ubuntu.24.04-x64) | Runtime for Ubuntu 24.04 x64 |
| Sdcb.OpenVINO.runtime.android-x64      | [![NuGet](https://img.shields.io/nuget/v/Sdcb.OpenVINO.runtime.android-x64.svg)](https://nuget.org/packages/Sdcb.OpenVINO.runtime.android-x64)           | Runtime for Android x64      |
| Sdcb.OpenVINO.runtime.osx.12.6-arm64   | [![NuGet](https://img.shields.io/nuget/v/Sdcb.OpenVINO.runtime.osx.12.6-arm64.svg)](https://nuget.org/packages/Sdcb.OpenVINO.runtime.osx.12.6-arm64)     | Runtime for macOS 12.6 ARM64 |
| Sdcb.OpenVINO.runtime.win-x64          | [![NuGet](https://img.shields.io/nuget/v/Sdcb.OpenVINO.runtime.win-x64.svg)](https://nuget.org/packages/Sdcb.OpenVINO.runtime.win-x64)                   | Runtime for Windows x64      |
| Sdcb.OpenVINO.runtime.win-mt-x64       | [![NuGet](https://img.shields.io/nuget/v/Sdcb.OpenVINO.runtime.win-mt-x64.svg)](https://nuget.org/packages/Sdcb.OpenVINO.runtime.win-mt-x64)             | Runtime for Windows x64 with VC MT runtime |

### OpenCvSharp4 mini runtime

| Id                                     | Version | Size      | OS      | Arch |
| -------------------------------------- | ----- | -------- | ------------ | ----- |
| Sdcb.OpenCvSharp4.mini.runtime.centos.7-arm64      | [![NuGet](https://img.shields.io/nuget/v/Sdcb.OpenCvSharp4.mini.runtime.centos.7-arm64.svg)](https://nuget.org/packages/Sdcb.OpenCvSharp4.mini.runtime.centos.7-arm64) | 3.23MB  | CentOS 7     | ARM64 |
| Sdcb.OpenCvSharp4.mini.runtime.centos.7-x64       | [![NuGet](https://img.shields.io/nuget/v/Sdcb.OpenCvSharp4.mini.runtime.centos.7-x64.svg)](https://nuget.org/packages/Sdcb.OpenCvSharp4.mini.runtime.centos.7-x64) | 16.75MB | CentOS 7     | x64   |
| Sdcb.OpenCvSharp4.mini.runtime.debian.11-arm64    | [![NuGet](https://img.shields.io/nuget/v/Sdcb.OpenCvSharp4.mini.runtime.debian.11-arm64.svg)](https://nuget.org/packages/Sdcb.OpenCvSharp4.mini.runtime.debian.11-arm64) | 4.05MB  | Debian 11    | ARM64 |
| Sdcb.OpenCvSharp4.mini.runtime.debian.11-x64      | [![NuGet](https://img.shields.io/nuget/v/Sdcb.OpenCvSharp4.mini.runtime.debian.11-x64.svg)](https://nuget.org/packages/Sdcb.OpenCvSharp4.mini.runtime.debian.11-x64) | 18.13MB | Debian 11    | x64   |
| Sdcb.OpenCvSharp4.mini.runtime.debian.12-arm64    | [![NuGet](https://img.shields.io/nuget/v/Sdcb.OpenCvSharp4.mini.runtime.debian.12-arm64.svg)](https://nuget.org/packages/Sdcb.OpenCvSharp4.mini.runtime.debian.12-arm64) | 4.18MB  | Debian 12    | ARM64 |
| Sdcb.OpenCvSharp4.mini.runtime.debian.12-x64      | [![NuGet](https://img.shields.io/nuget/v/Sdcb.OpenCvSharp4.mini.runtime.debian.12-x64.svg)](https://nuget.org/packages/Sdcb.OpenCvSharp4.mini.runtime.debian.12-x64) | 17.47MB | Debian 12    | x64   |
| Sdcb.OpenCvSharp4.mini.runtime.ubuntu.22.04-arm64 | [![NuGet](https://img.shields.io/nuget/v/Sdcb.OpenCvSharp4.mini.runtime.ubuntu.22.04-arm64.svg)](https://nuget.org/packages/Sdcb.OpenCvSharp4.mini.runtime.ubuntu.22.04-arm64) | 4.18MB  | Ubuntu 22.04 | ARM64 |
| Sdcb.OpenCvSharp4.mini.runtime.ubuntu.22.04-x64   | [![NuGet](https://img.shields.io/nuget/v/Sdcb.OpenCvSharp4.mini.runtime.ubuntu.22.04-x64.svg)](https://nuget.org/packages/Sdcb.OpenCvSharp4.mini.runtime.ubuntu.22.04-x64) | 17.47MB | Ubuntu 22.04 | x64   |
| Sdcb.OpenCvSharp4.mini.runtime.android-arm64      | [![NuGet](https://img.shields.io/nuget/v/Sdcb.OpenCvSharp4.mini.runtime.android-arm64.svg)](https://nuget.org/packages/Sdcb.OpenCvSharp4.mini.runtime.android-arm64) | 4.04MB  | Android      | ARM64 |
| Sdcb.OpenCvSharp4.mini.runtime.android-x64      | [![NuGet](https://img.shields.io/nuget/v/Sdcb.OpenCvSharp4.mini.runtime.android-x64.svg)](https://nuget.org/packages/Sdcb.OpenCvSharp4.mini.runtime.android-x64) | 5.9MB  | Android      | x64 |


## Linux docker images(deprecated)

| Docker Image           | Version 📌                                                                                                            | Description                                   |
| ---------------------- | -------------------------------------------------------------------------------------------------------------------- | --------------------------------------------- |
| sdflysha/openvino-base | [![Docker](https://img.shields.io/docker/v/sdflysha/openvino-base)](https://hub.docker.com/r/sdflysha/openvino-base) | .NET 7 SDK, OpenCvSharp 4.8, Ubuntu 22.04 x64 |

Note: 

This docker image was built by [this dockerfile](https://github.com/sdcb/dockerfiles/blob/main/openvino/openvino-base/dockerfile), You can build other docker images as well.

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

| Package                               | Version 📌                                                                                                                                              | Description                           |
| ------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------ | ------------------------------------- |
| Sdcb.OpenVINO.PaddleOCR               | [![NuGet](https://img.shields.io/nuget/v/Sdcb.OpenVINO.PaddleOCR.svg)](https://nuget.org/packages/Sdcb.OpenVINO.PaddleOCR)                             | OpenVINO Paddle OCR Toolkit           |
| Sdcb.OpenVINO.PaddleOCR.Models.Online | [![NuGet](https://img.shields.io/nuget/v/Sdcb.OpenVINO.PaddleOCR.Models.Online.svg)](https://nuget.org/packages/Sdcb.OpenVINO.PaddleOCR.Models.Online) | Online Models for OpenVINO Paddle OCR |

The build instructions/docs is in this [opencvsharp-mini-runtime](https://github.com/sdcb/opencvsharp-mini-runtime) repository.

### Usage

You can refer to this github for mini-openvino-paddleocr demo: https://github.com/sdcb/mini-openvino-paddleocr

Actually it's very similar to my another project [PaddleSharp](https://github.com/sdcb/PaddleSharp/blob/master/docs/ocr.md)

# LICENSE

[Apache](./LICENSE.txt)
