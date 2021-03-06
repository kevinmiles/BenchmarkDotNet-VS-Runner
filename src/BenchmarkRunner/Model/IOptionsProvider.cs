﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BenchmarkRunner.Model
{
    public interface IOptionsProvider
    {
        string CommandlineParameters { get; }
        bool IsMemoryDiagnoserEnabled { get; }
        bool IsDisassemblyDiagnoserEnabled { get;  }
        bool IsEtwProfilerEnabled { get; }
    }
}
