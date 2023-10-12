# OpenVINO.NET ![NuGet](https://img.shields.io/nuget/dt/Sdcb.OpenVINO.svg?style=flat-square) [![QQ](https://img.shields.io/badge/QQ_Group-495782587-52B6EF?style=social&logo=tencent-qq&logoColor=000&logoWidth=20)](http://qm.qq.com/cgi-bin/qm/qr?_wv=1027&k=mma4msRKd372Z6dWpmBp4JZ9RL4Jrf8X&authKey=gccTx0h0RaH5b8B8jtuPJocU7MgFRUznqbV%2FLgsKdsK8RqZE%2BOhnETQ7nYVTp1W0&noverify=0&group_code=495782587)

High quality .NET wrapper for OpenVINO™ toolkit.

## Packages

| Package                                | Version 📌                                                                                                                                                | Description                  |
| -------------------------------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------- | ---------------------------- |
| Sdcb.OpenVINO                          | [![NuGet](https://img.shields.io/nuget/v/Sdcb.OpenVINO.svg)](https://nuget.org/packages/Sdcb.OpenVINO)                                                   | .NET PInvoke interface       |
| Sdcb.OpenVINO.runtime.centos.7-x64     | [![NuGet](https://img.shields.io/nuget/v/Sdcb.OpenVINO.runtime.centos.7-x64.svg)](https://nuget.org/packages/Sdcb.OpenVINO.runtime.centos.7-x64)         | Runtime for CentOS 7 x64     |
| Sdcb.OpenVINO.runtime.debian.9-arm     | [![NuGet](https://img.shields.io/nuget/v/Sdcb.OpenVINO.runtime.debian.9-arm.svg)](https://nuget.org/packages/Sdcb.OpenVINO.runtime.debian.9-arm)         | Runtime for Debian 9 ARM     |
| Sdcb.OpenVINO.runtime.debian.9-arm64   | [![NuGet](https://img.shields.io/nuget/v/Sdcb.OpenVINO.runtime.debian.9-arm64.svg)](https://nuget.org/packages/Sdcb.OpenVINO.runtime.debian.9-arm64)     | Runtime for Debian 9 ARM64   |
| Sdcb.OpenVINO.runtime.rhel.8-x64       | [![NuGet](https://img.shields.io/nuget/v/Sdcb.OpenVINO.runtime.rhel.8-x64.svg)](https://nuget.org/packages/Sdcb.OpenVINO.runtime.rhel.8-x64)             | Runtime for RHEL 8 x64       |
| Sdcb.OpenVINO.runtime.ubuntu.18.04-x64 | [![NuGet](https://img.shields.io/nuget/v/Sdcb.OpenVINO.runtime.ubuntu.18.04-x64.svg)](https://nuget.org/packages/Sdcb.OpenVINO.runtime.ubuntu.18.04-x64) | Runtime for Ubuntu 18.04 x64 |
| Sdcb.OpenVINO.runtime.ubuntu.20.04-x64 | [![NuGet](https://img.shields.io/nuget/v/Sdcb.OpenVINO.runtime.ubuntu.20.04-x64.svg)](https://nuget.org/packages/Sdcb.OpenVINO.runtime.ubuntu.20.04-x64) | Runtime for Ubuntu 20.04 x64 |
| Sdcb.OpenVINO.runtime.ubuntu.22.04-x64 | [![NuGet](https://img.shields.io/nuget/v/Sdcb.OpenVINO.runtime.ubuntu.22.04-x64.svg)](https://nuget.org/packages/Sdcb.OpenVINO.runtime.ubuntu.22.04-x64) | Runtime for Ubuntu 22.04 x64 |
| Sdcb.OpenVINO.runtime.win-x64          | [![NuGet](https://img.shields.io/nuget/v/Sdcb.OpenVINO.runtime.win-x64.svg)](https://nuget.org/packages/Sdcb.OpenVINO.runtime.win-x64)                   | Runtime for Windows x64      |

## Linux docker images

| Docker Image           | Version 📌                                                                                                            | Description                                   |
| ---------------------- | -------------------------------------------------------------------------------------------------------------------- | --------------------------------------------- |
| sdflysha/openvino-base | [![Docker](https://img.shields.io/docker/v/sdflysha/openvino-base)](https://hub.docker.com/r/sdflysha/openvino-base) | .NET 7 SDK, OpenCvSharp 4.8, Ubuntu 22.04 x64 |

Note: 

This docker image was built by [this dockerfile](https://github.com/sdcb/dockerfiles/blob/main/openvino/openvino-base/dockerfile), You can build other docker images as well.

## Infer examples:

Install packages:
* OpenCvSharp4
* OpenCVSharp4.runtime.win
* Sdcb.OpenVINO
* Sdcb.OpenVINO.runtime.win-x64

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

### Usage

You can refer to this github for mini-openvino-paddleocr demo: https://github.com/sdcb/mini-openvino-paddleocr

Actually it's very similar to my another project [PaddleSharp](https://github.com/sdcb/PaddleSharp/blob/master/docs/ocr.md)

# LICENSE

[Apache](./LICENSE.txt)
