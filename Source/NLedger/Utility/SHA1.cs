// **********************************************************************************
// Copyright (c) 2015-2021, Dmitry Merzlyakov.  All rights reserved.
// Licensed under the FreeBSD Public License. See LICENSE file included with the distribution for details and disclaimer.
// 
// This file is part of NLedger that is a .Net port of C++ Ledger tool (ledger-cli.org). Original code is licensed under:
// Copyright (c) 2003-2021, John Wiegley.  All rights reserved.
// See LICENSE.LEDGER file included with the distribution for details and disclaimer.
// **********************************************************************************
using System;
using System.Security.Cryptography;
using System.Text;


namespace NLedger.Utility
{
    /// <summary>
    /// The original name of the class is SHA1 but we use SHA256
    /// </summary>
    public static class SHA1
    {
        public static string GetHash(string input)
        {
            //SHA1 was obsolete so it is replaced by SHA256
			using SHA256 sha256 = SHA256.Create();
            {
                var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }
    }
}