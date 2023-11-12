// **********************************************************************************
// Copyright (c) 2023, Bart Vertongen.  All rights reserved.
// Licensed under the FreeBSD Public License. See LICENSE file included with the distribution for details and disclaimer.
// 
// This file is part of NLedger that is a .Net port of C++ Ledger tool (ledger-cli.org). Original code is licensed under:
// Copyright (c) 2003-2021, John Wiegley.  All rights reserved.
// See LICENSE.LEDGER file included with the distribution for details and disclaimer.
// **********************************************************************************
using System;


namespace NLedger.Extensibility.Python
{
    public class PythonExtensionProvider : IExtensionProvider
    {
        public PythonExtensionProvider(Func<PythonSession> netSessionFactory = null, Action<PythonSession> configureAction = null)
        {
            /*NetSessionFactory = PythonSessionFactory ?? (() => new NetSession(new NamespaceResolver(), new ValueConverter()));
            ConfigureAction = configureAction;*/
        }

        public Func<PythonSession> NetSessionFactory { get; }
        public Action<PythonSession> ConfigureAction { get; }

        public ExtendedSession CreateExtendedSession()
        {
			ExtendedSession extendedSession = null;// PythonSessionFactory();
            //ConfigureAction?.Invoke(extendedSession);
            return extendedSession;
        }
    }
}