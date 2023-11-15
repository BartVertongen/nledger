// **********************************************************************************
// Copyright (c) 2015-2021, Dmitry Merzlyakov.  All rights reserved.
// Licensed under the FreeBSD Public License. See LICENSE file included with the distribution for details and disclaimer.
// 
// This file is part of NLedger that is a .Net port of C++ Ledger tool (ledger-cli.org). Original code is licensed under:
// Copyright (c) 2003-2021, John Wiegley.  All rights reserved.
// See LICENSE.LEDGER file included with the distribution for details and disclaimer.
// **********************************************************************************
using NLedger.Utility;
using Xunit;


namespace NLedger.Tests.Utility
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>In real we use SHA256 because SHA1 is obsolete</remarks>
    public class SHA1Tests
    {
        [Fact]
        public void SHA1_GetHash_Returns40CharHashString()
        {
            Assert.Equal("21e526712b83048d413eae729660ee2a6048282e7eeb773eb3eb6d0f5404af5b", SHA1.GetHash("2012/01/01,KFC,$10"));
            Assert.Equal("a9b0afc7c2fb7b9327ffaf8a995effd9226626291ce6b289c004a1abfb029a68", SHA1.GetHash("2012/01/02,\"REWE SAGT DANKE  123454321\",10€"));
        }
    }
}
