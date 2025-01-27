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
//using NLedger.Extensibility.Python.Platform;
using NLedger.Tests;
using System;
using System.IO;
using Xunit;


namespace NLedger.Extensibility.Python.Tests
{
    public class PythonModuleTests : TestFixture, IDisposable
	{
        public PythonModuleTests()
        {
            /*Assert.True(PythonConnector.Current.IsAvailable);
            PythonConnectionContext = PythonConnector.Current.Connect();
            PythonConnector.Current.KeepAlive = false;*/
        }

        //public PythonConnectionContext PythonConnectionContext { get; }

        public void Dispose()
        {
            //PythonConnectionContext.Dispose();
        }

        /// <summary>
        /// This method runs all Python Module unit tests defined in the file /NLedger.Extensibility.Python.Module/tests/ledger_tests.py
        /// It uses application-hosted PythonNet connection, so it does not require PythonNet to be installed in Python environment.
        /// It is expected that produced test result is equal to the test result in Python console (>python.exe ledger_tests.py)
        /// Remember that running unit tests in Python console requires PythonNet installed.
        /// 
        /// Troubleshooting: if this method fails, you should run unit tests in Python console and make sure that they pass well.
        /// If further iinvestigation is needed, check comments in this method below explaining how to get the test result 
        /// collection and observe it.
        /// </summary>
        [Fact]
        public void PythonModule_ExecuteUnitTests()
        {
            var moduleTestFile = Path.GetFullPath("../../../../NLedger.Extensibility.Python.Module/tests/ledger_tests.py");
            Assert.True(File.Exists(moduleTestFile));
            var moduleTestFolder = Path.GetDirectoryName(moduleTestFile);

            //PythonSession.PythonModuleInitialization();
            try
            {
				using PythonSession oSession = new PythonSession();
				PythonSession.Current.Initialize();

				// Unit tests are executed in local scope. Since Python unit test runner can only manage "main" scope,
				// it is necessary to run it explicitely (unittest.main(...)) specifying a test file name as a parameter
				// Finally, get a boolean value that indicates whether all tests passed well or not (...result.wasSuccessful())

				// Test runner parameters are: 
				// exit=False - supresses exiting the current process when all tests are done
				// module=None - supresses module importing (basically, "main")
				// argv[0] - just a string indicating the current process name (required)
				// argv[1] - name of a unit test file (should be searchable by Python paths)

				// This method only use a final boolean value (Yes/No) to check whether tests are passed without any diagnostics.
				// If it is neccessary to troubleshoot this step, it is recommended to extract a test result object and go through detected errors
				// FOr example: var res = scope.Eval(@"unittest.main(exit=False,module=None,argv=('EmbeddedHost','ledger_tests')).result");

				oSession.Scope.SetVariable("module_test_folder", moduleTestFolder);

				oSession.Engine.Import("sys");
				oSession.Engine.Execute(@"sys.path.insert(0, module_test_folder)");

				oSession.Scope.Import("unittest");

                /* This code is helpful to troubleshoot Python test issues executed under xUnits
                var sb = new StringBuilder();
                var result = scope.Eval(@"unittest.main(exit=False,module=None,argv=('EmbeddedHost','ledger_tests')).result");
                sb.AppendLine($"Test runs: {result.GetAttr("testsRun")}");
                PrintTestResultEntry(result.GetAttr("errors"), sb, "Errors");
                PrintTestResultEntry(result.GetAttr("failures"), sb, "Failures");
                Console.WriteLine(sb.ToString()); */

                Assert.True(oSession.Engine.Execute<bool>(@"unittest.main(exit=False,module=None,argv=('EmbeddedHost','ledger_tests')).result.wasSuccessful()"));
            }
            finally
            {
                //PythonSession.PythonModuleShutdown();
            }
        }

        /*private static void PrintTestResultEntry(Object pyObject, StringBuilder sb, string description)
        {
            sb.AppendLine($"{description}: {pyObject.Length()}");
            for(int i = 0; i < pyObject.Length(); i++)
            {
                var obj = pyObject[i];
                for (int j = 0; j < obj.Length(); j++)
                    sb.AppendLine(obj[j].ToString());
                sb.AppendLine();
            }
        }*/
    }
}