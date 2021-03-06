﻿using BenchmarkRunner.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BenchmarkRunner.Runner
{
    public class BenchmarkRunController
    {
        private readonly RunParameters _parameters;
        private readonly IOptionsProvider _optionsProvider;

        public BenchmarkRunController(RunParameters parameters, IOptionsProvider optionsProvider)
        {
            _parameters = parameters;
            _optionsProvider = optionsProvider;
        }

        public Task RunAsync()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = BuildArguments(),
                CreateNoWindow = false,
                WindowStyle = ProcessWindowStyle.Normal,
                WorkingDirectory = GetWorkingDirectory()
            };
            var process = Process.Start(startInfo);
            return Task.Run(() => process.WaitForExit());
        }

        private string GetWorkingDirectory()
        {
            if (_parameters.Runtime == TargetRuntime.NetFramework)
                return _parameters.OutputPath;

            return _parameters.ProjectPath;
        }

        private string GetExecutable()
        {
            if (_parameters.Runtime == TargetRuntime.NetFramework)
                return _parameters.AssemblyPath;

            return "dotnet run -c Release";
        }

        private string BuildArguments()
        {
            string arguments = "/k " + GetExecutable();

            if (_parameters.IsDryRun)
                arguments += " -j Dry";

            arguments += " " + _parameters.BuildFilter();

            if (_optionsProvider.IsMemoryDiagnoserEnabled)
                arguments += " -m";

            if (_optionsProvider.IsDisassemblyDiagnoserEnabled)
                arguments += " -d";

            if (_optionsProvider.IsEtwProfilerEnabled)
                arguments += " -p ETW";

            if (!string.IsNullOrWhiteSpace(_optionsProvider.CommandlineParameters))
                arguments += " " + _optionsProvider.CommandlineParameters;

            if (!arguments.Contains(" -e "))
            {
                arguments += " -e markdown"; // always included for summary tab
            }
            else
            {
                if (!arguments.Contains("markdown"))
                    arguments = arguments.Replace(" -e ", " -e markdown ");
            }
            return arguments;
        }
    }
}
