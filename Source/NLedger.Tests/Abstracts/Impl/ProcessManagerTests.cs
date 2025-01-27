﻿// **********************************************************************************
// Copyright (c) 2015-2021, Dmitry Merzlyakov.  All rights reserved.
// Licensed under the FreeBSD Public License.
// See LICENSE file included with the distribution for details and disclaimer.
// 
// This file is part of NLedger that is a .Net port of C++ Ledger tool (ledger-cli.org).
// Original code is licensed under:
// Copyright (c) 2003-2021, John Wiegley.  All rights reserved.
// See LICENSE.LEDGER file included with the distribution for details and disclaimer.
// **********************************************************************************
using NLedger.Abstracts.Impl;
using System.IO;
using Xunit;


namespace NLedger.Tests.Abstracts.Impl
{
    public class ProcessManagerTests
    {
		///<remarks>A Unit test should not depend on the OS or a physical file</remarks>
		[Fact]
        public void ProcessManager_RunProcess_CanExecuteBatchFile()
        {
            var shellName = NLedger.Utility.PlatformHelper.IsWindows() ? "cmd" : "bash";
            var shellPrefix = NLedger.Utility.PlatformHelper.IsWindows() ? "/c " : "";
            var shellExtension = NLedger.Utility.PlatformHelper.IsWindows() ? ".cmd" : ".sh";

            var result = ProcessManager.RunProcess(shellName, $"{shellPrefix}.{Path.DirectorySeparatorChar}ProcessManagerBatch{shellExtension} param1 param2");
            Assert.NotNull(result);
            Assert.Equal("", result.StandardError);
            Assert.Equal("Process Manager Test Batch File\r\nParameter 1 - param1\r\nParameter 2 - param2\r\nParameter 3 - \r\n", result.StandardOutput);
            Assert.Equal(0, result.ExitCode);
            Assert.False(result.IsTimeouted);
        }

		///<remarks>A Unit test should not depend on the OS or a physical file</remarks>
		[Fact]
        public void ProcessManager_RunProcess_DetectsBatchExistCode()
        {
            var shellName = NLedger.Utility.PlatformHelper.IsWindows() ? "cmd" : "bash";
            var shellPrefix = NLedger.Utility.PlatformHelper.IsWindows() ? "/c " : "";
            var shellExtension = NLedger.Utility.PlatformHelper.IsWindows() ? ".cmd" : ".sh";

            var result = ProcessManager.RunProcess(shellName, $"{shellPrefix}.{Path.DirectorySeparatorChar}ProcessManagerErrBatch{shellExtension}");
            Assert.NotNull(result);
            Assert.Equal("", result.StandardError);
            Assert.Equal(1, result.ExitCode);
        }
    }
}