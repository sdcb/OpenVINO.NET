```

BenchmarkDotNet v0.13.10, Windows 10 (10.0.19044.3693/21H2/November2021Update)
  [Host]     : .NET 7.0.14 (7.0.1423.51910), X64 RyuJIT AVX2
  Job-PARHUX : .NET 7.0.14 (7.0.1423.51910), X64 RyuJIT AVX2
  Job-YLHAAW : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2

IterationCount=10  LaunchCount=1  WarmupCount=1  

```
| Method                   | Runtime  | FileName   | Mean      | Error      | StdDev    | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
|------------------------- |--------- |----------- |----------:|-----------:|----------:|------:|--------:|-------:|----------:|------------:|
| **StackingVerticallyBySdcb** | **.NET 7.0** | **test_1.jpg** |  **1.714 μs** |  **0.0270 μs** | **0.0161 μs** |  **1.00** |    **0.00** | **0.0668** |     **280 B** |        **1.00** |
| StackingVerticallyByAven | .NET 7.0 | test_1.jpg |  1.155 μs |  0.0175 μs | 0.0116 μs |  0.67 |    0.01 | 0.0553 |     232 B |        0.83 |
|                          |          |            |           |            |           |       |         |        |           |             |
| StackingVerticallyBySdcb | .NET 8.0 | test_1.jpg |  1.621 μs |  0.0194 μs | 0.0128 μs |  1.00 |    0.00 | 0.0668 |     280 B |        1.00 |
| StackingVerticallyByAven | .NET 8.0 | test_1.jpg |  1.130 μs |  0.0054 μs | 0.0036 μs |  0.70 |    0.01 | 0.0553 |     232 B |        0.83 |
|                          |          |            |           |            |           |       |         |        |           |             |
| **StackingVerticallyBySdcb** | **.NET 7.0** | **test_2.jpg** |  **6.522 μs** |  **0.0300 μs** | **0.0178 μs** |  **1.00** |    **0.00** | **0.0610** |     **280 B** |        **1.00** |
| StackingVerticallyByAven | .NET 7.0 | test_2.jpg |  3.980 μs |  0.0239 μs | 0.0142 μs |  0.61 |    0.00 | 0.0534 |     232 B |        0.83 |
|                          |          |            |           |            |           |       |         |        |           |             |
| StackingVerticallyBySdcb | .NET 8.0 | test_2.jpg |  6.641 μs |  0.1991 μs | 0.1317 μs |  1.00 |    0.00 | 0.0610 |     280 B |        1.00 |
| StackingVerticallyByAven | .NET 8.0 | test_2.jpg |  3.986 μs |  0.0611 μs | 0.0404 μs |  0.60 |    0.02 | 0.0534 |     232 B |        0.83 |
|                          |          |            |           |            |           |       |         |        |           |             |
| **StackingVerticallyBySdcb** | **.NET 7.0** | **test_3.jpg** | **13.510 μs** |  **0.1125 μs** | **0.0744 μs** |  **1.00** |    **0.00** | **0.0610** |     **280 B** |        **1.00** |
| StackingVerticallyByAven | .NET 7.0 | test_3.jpg |  7.880 μs |  0.0544 μs | 0.0360 μs |  0.58 |    0.00 | 0.0458 |     232 B |        0.83 |
|                          |          |            |           |            |           |       |         |        |           |             |
| StackingVerticallyBySdcb | .NET 8.0 | test_3.jpg | 13.609 μs |  0.2727 μs | 0.1623 μs |  1.00 |    0.00 | 0.0610 |     280 B |        1.00 |
| StackingVerticallyByAven | .NET 8.0 | test_3.jpg |  7.997 μs |  0.2122 μs | 0.1263 μs |  0.59 |    0.01 | 0.0458 |     232 B |        0.83 |
|                          |          |            |           |            |           |       |         |        |           |             |
| **StackingVerticallyBySdcb** | **.NET 7.0** | **test_4.jpg** | **74.877 μs** | **11.3797 μs** | **7.5270 μs** |  **1.00** |    **0.00** |      **-** |     **280 B** |        **1.00** |
| StackingVerticallyByAven | .NET 7.0 | test_4.jpg | 48.114 μs |  0.4437 μs | 0.2935 μs |  0.65 |    0.06 |      - |     232 B |        0.83 |
|                          |          |            |           |            |           |       |         |        |           |             |
| StackingVerticallyBySdcb | .NET 8.0 | test_4.jpg | 85.467 μs |  0.4689 μs | 0.2790 μs |  1.00 |    0.00 |      - |     280 B |        1.00 |
| StackingVerticallyByAven | .NET 8.0 | test_4.jpg | 48.134 μs |  0.6190 μs | 0.3683 μs |  0.56 |    0.00 |      - |     232 B |        0.83 |
