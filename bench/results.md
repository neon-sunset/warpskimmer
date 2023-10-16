```
AMD Ryzen 5800X (AVX2)
.NET SDK:
 Version:           8.0.100-rtm.23513.4
 Commit:            c65382f002
 Workload version:  8.0.100-manifests.69ee1144

Runtime Environment:
 OS Name:     Windows
 OS Version:  10.0.22621
 OS Platform: Windows
 RID:         win-x64
 Base Path:   C:\Program Files\dotnet\sdk\8.0.100-rtm.23513.4\

.NET workloads installed:
 Workload version: 8.0.100-manifests.69ee1144
There are no installed workloads to display.

Host:
  Version:      8.0.0-rtm.23511.16
  Architecture: x64
  Commit:       256bf22a3d
```

| OptimizationPreference | IlcInstructionSet | Iterations | Elapsed | Per iteration |
| ---------------------- | ----------------- | ---------- | ------- | ------------- |
| None                   | `default`         | 5          | 02.28s  | 22.61ms       |
| None                   | `x86-x64-v2`      | 5          | 02.13s  | 21.35ms       |
| None                   | `x86-x64-v3`      | 5          | 02.05s  | 20.44ms       |
| None                   | `native`          | 5          | 02.02s  | 20.24ms       |
| Speed                  | `default`         | 5          | 01.95s  | 19.44ms       |
| Speed                  | `x86-x64-v2`      | 5          | 01.87s  | 18.79ms       |
| Speed                  | `x86-x64-v3`      | 5          | 01.68s  | 16.85ms       |
| Speed                  | `native`          | 5          | 01.65s  | 16.55ms       |

<Details>
    <Summary>Data</Summary>

### `OptimizationPreference`: not set

#### `IlcInstructionSet`: default
```
Elapsed: 00:00:02.2840156 per iteration: 22.84ms
Elapsed: 00:00:02.2603665 per iteration: 22.6ms
Elapsed: 00:00:02.2671803 per iteration: 22.67ms
Elapsed: 00:00:02.2608182 per iteration: 22.61ms
Elapsed: 00:00:02.2773528 per iteration: 22.77ms
```

#### `IlcInstructionSet`: x86-x64-v2
```
Elapsed: 00:00:02.1351768 per iteration: 21.35ms
Elapsed: 00:00:02.1346578 per iteration: 21.35ms
Elapsed: 00:00:02.1308745 per iteration: 21.31ms
Elapsed: 00:00:02.1298078 per iteration: 21.3ms
Elapsed: 00:00:02.1348217 per iteration: 21.35ms
```

#### `IlcInstructionSet`: x86-x64-v3
```
Elapsed: 00:00:02.0518181 per iteration: 20.52ms
Elapsed: 00:00:02.0489025 per iteration: 20.49ms
Elapsed: 00:00:02.0429215 per iteration: 20.43ms
Elapsed: 00:00:02.0442737 per iteration: 20.44ms
Elapsed: 00:00:02.0398385 per iteration: 20.4ms
```

#### `IlcInstructionSet`: native
```
Elapsed: 00:00:02.0238547 per iteration: 20.24ms
Elapsed: 00:00:02.0258877 per iteration: 20.26ms
Elapsed: 00:00:02.0266846 per iteration: 20.27ms
Elapsed: 00:00:02.0231904 per iteration: 20.23ms
Elapsed: 00:00:02.0189550 per iteration: 20.19ms
```

### `OptimizationPreference`: Speed

#### `IlcInstructionSet`: default
```
Elapsed: 00:00:01.9578083 per iteration: 19.58ms
Elapsed: 00:00:01.9513075 per iteration: 19.51ms
Elapsed: 00:00:01.9417853 per iteration: 19.42ms
Elapsed: 00:00:01.9438932 per iteration: 19.44ms
Elapsed: 00:00:01.9428932 per iteration: 19.43ms
```

#### `IlcInstructionSet`: x86-x64-v2
```
Elapsed: 00:00:01.8892263 per iteration: 18.89ms
Elapsed: 00:00:01.8895930 per iteration: 18.9ms
Elapsed: 00:00:01.8794199 per iteration: 18.79ms
Elapsed: 00:00:01.8752040 per iteration: 18.75ms
Elapsed: 00:00:01.8747006 per iteration: 18.75ms
```

#### `IlcInstructionSet`: x86-x64-v3
```
Elapsed: 00:00:01.6846312 per iteration: 16.85ms
Elapsed: 00:00:01.7029798 per iteration: 17.03ms
Elapsed: 00:00:01.6874002 per iteration: 16.87ms
Elapsed: 00:00:01.6852899 per iteration: 16.85ms
Elapsed: 00:00:01.6802060 per iteration: 16.8ms
```

#### `IlcInstructionSet`: native
```
Elapsed: 00:00:01.6545408 per iteration: 16.55ms
Elapsed: 00:00:01.6552406 per iteration: 16.55ms
Elapsed: 00:00:01.6529058 per iteration: 16.53ms
Elapsed: 00:00:01.6661091 per iteration: 16.66ms
Elapsed: 00:00:01.6553239 per iteration: 16.55ms
```

</Details>
