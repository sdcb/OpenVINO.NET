```

BenchmarkDotNet v0.13.10, Windows 10 (10.0.19044.3693/21H2/November2021Update)
  [Host]     : .NET 7.0.14 (7.0.1423.51910), X64 RyuJIT AVX2
  Job-PARHUX : .NET 7.0.14 (7.0.1423.51910), X64 RyuJIT AVX2
  Job-YLHAAW : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2

IterationCount=10  LaunchCount=1  WarmupCount=1  

```
| Method                   | Runtime  | modelHeight | maxWidth | Mean     | Error    | StdDev   | Ratio | RatioSD | Allocated | Alloc Ratio |
|------------------------- |--------- |------------ |--------- |---------:|---------:|---------:|------:|--------:|----------:|------------:|
| **StackingVerticallyBySdcb** | **.NET 7.0** | **48**          | **320**      | **415.2 μs** | **15.14 μs** | **10.01 μs** |  **1.00** |    **0.00** |   **1.93 KB** |        **1.00** |
| StackingVerticallyByAven | .NET 7.0 | 48          | 320      | 252.8 μs |  8.97 μs |  5.93 μs |  0.61 |    0.02 |   1.79 KB |        0.93 |
|                          |          |             |          |          |          |          |       |         |           |             |
| StackingVerticallyBySdcb | .NET 8.0 | 48          | 320      | 408.3 μs | 11.43 μs |  7.56 μs |  1.00 |    0.00 |   1.93 KB |        1.00 |
| StackingVerticallyByAven | .NET 8.0 | 48          | 320      | 248.5 μs | 12.37 μs |  8.18 μs |  0.61 |    0.02 |   1.79 KB |        0.93 |
|                          |          |             |          |          |          |          |       |         |           |             |
| **StackingVerticallyBySdcb** | **.NET 7.0** | **48**          | **512**      | **423.3 μs** | **18.53 μs** | **12.26 μs** |  **1.00** |    **0.00** |   **1.54 KB** |        **1.00** |
| StackingVerticallyByAven | .NET 7.0 | 48          | 512      | 361.4 μs | 14.72 μs |  9.74 μs |  0.85 |    0.04 |   1.62 KB |        1.05 |
|                          |          |             |          |          |          |          |       |         |           |             |
| StackingVerticallyBySdcb | .NET 8.0 | 48          | 512      | 419.8 μs | 14.95 μs |  9.89 μs |  1.00 |    0.00 |   1.54 KB |        1.00 |
| StackingVerticallyByAven | .NET 8.0 | 48          | 512      | 360.3 μs | 14.88 μs |  9.84 μs |  0.86 |    0.03 |   1.62 KB |        1.05 |