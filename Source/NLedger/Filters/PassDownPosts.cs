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
using NLedger.Chain;
using NLedger.Items;
using NLedger.Utility;
using System.Collections.Generic;


namespace NLedger.Filters
{
    public class PassDownPosts : PostHandler
    {
        /// <summary>
        /// Iterates through all the posts in the Iterator and handles each, with the given Handler
        /// </summary>
        /// <param name="handler">The handler for the posts</param>
        /// <param name="posts">Iterator for the posts</param>
        public PassDownPosts(PostHandler handler, IEnumerable<Post> posts)
            : base(handler)
        {
            foreach(Post post in posts)
            {
                try
                {
                    Handle(post);
                }
                catch
                {
                    ErrorContext.Current.AddErrorContext(Item.ItemContext(post, "While handling posting"));
                    throw;
                }
            }
            Flush();
        }
    }
}