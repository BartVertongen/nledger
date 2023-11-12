// **********************************************************************************
// Copyright (c) 2015-2021, Dmitry Merzlyakov.  All rights reserved.
// Licensed under the FreeBSD Public License.
// See LICENSE file included with the distribution for details and disclaimer.
// 
// This file is part of NLedger that is a .Net port of C++ Ledger tool (ledger-cli.org).
// Original code is licensed under:
// Copyright (c) 2003-2021, John Wiegley.  All rights reserved.
// See LICENSE.LEDGER file included with the distribution for details and disclaimer.
// **********************************************************************************
using NLedger.Utility.Settings;
using System;


namespace NLedger.CLI
{
    class Program
    {
        /// <summary>
        /// This Main gets all the settings and providers afterwards it calls an inner Main.
        /// </summary>
        /// <param name="args"></param>
        /// <remarks>The arguments will be processed by the internal Main class</remarks>
        static void Main(string[] args)
        {
            // System.Diagnostics.Debugger.Launch(); // This debugging option might be useful in case of troubleshooting of NLTest issues

            //Creates an extensionProviderSelector and then add the existing providers
            var extensionProviderSelector = new Extensibility.ExtensionProviderSelector()
                    .AddProvider("dotnet", () => new Extensibility.Net.NetExtensionProvider())
                    //TODO: fix Python extensibility (IronPython?)
                    .AddProvider("python", () => new Extensibility.Python.PythonExtensionProvider());

            //The context contains different providers and Settings
            var context = new NLedgerConfiguration().CreateConsoleApplicationContext(extensionProviderSelector);
            var main = new Main(context);

            //But it depends on the Windows OS, so not multi-platform
            //TODO: fix dependency on windows apis
            var argString = CommandLineArgs.GetArguments(args); // This way is preferrable because of double quotas that are missed by using args
            Environment.ExitCode = main.Execute(argString);
        }
    }
}