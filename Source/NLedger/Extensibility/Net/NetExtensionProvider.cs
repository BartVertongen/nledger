// **********************************************************************************
// Copyright (c) 2015-2021, Dmitry Merzlyakov.  All rights reserved.
// Licensed under the FreeBSD Public License. See LICENSE file included with the distribution for details and disclaimer.
// 
// This file is part of NLedger that is a .Net port of C++ Ledger tool (ledger-cli.org). Original code is licensed under:
// Copyright (c) 2003-2021, John Wiegley.  All rights reserved.
// See LICENSE.LEDGER file included with the distribution for details and disclaimer.
// **********************************************************************************
using System;


namespace NLedger.Extensibility.Net
{
    public class NetExtensionProvider : IExtensionProvider
    {
        public NetExtensionProvider(Func<NetSession> netSessionFactory = null, Action<NetSession> configureAction = null)
        {
			//NetSessionFactory = netSessionFactory ?? (() => new NetSession(new NamespaceResolver(), new ValueConverter()));
            //With Global scan the NETSession unit tests work
			NetSessionFactory = netSessionFactory ?? (() => new NetSession(new NamespaceResolver(true), new ValueConverter()));
			ConfigureAction = configureAction;
        }

        public Func<NetSession> NetSessionFactory { get; }

        public Action<NetSession> ConfigureAction { get; }

        public ExtendedSession CreateExtendedSession()
        {
            var extendedSession = NetSessionFactory();
            ConfigureAction?.Invoke(extendedSession);
            return extendedSession;
        }
    }
}