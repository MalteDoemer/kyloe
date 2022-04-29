``` ini

BenchmarkDotNet=v0.13.1, OS=ubuntu 20.04
Intel Core i5-8250U CPU 1.60GHz (Kaby Lake R), 1 CPU, 8 logical and 4 physical cores
.NET SDK=6.0.202
  [Host]     : .NET 6.0.4 (6.0.422.16404), X64 RyuJIT
  DefaultJob : .NET 6.0.4 (6.0.422.16404), X64 RyuJIT


```
|    Method |     Mean |   Error |  StdDev |
|---------- |---------:|--------:|--------:|
| LexSimple | 246.5 μs | 4.82 μs | 5.73 μs |
