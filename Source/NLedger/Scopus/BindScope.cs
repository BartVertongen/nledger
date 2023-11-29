// **********************************************************************************
// Copyright (c) 2015-2021, Dmitry Merzlyakov.  All rights reserved.
// Licensed under the FreeBSD Public License. See LICENSE file included with the distribution for details and disclaimer.
// 
// This file is part of NLedger that is a .Net port of C++ Ledger tool (ledger-cli.org). Original code is licensed under:
// Copyright (c) 2003-2021, John Wiegley.  All rights reserved.
// See LICENSE.LEDGER file included with the distribution for details and disclaimer.
// **********************************************************************************
using NLedger.Expressions;
using NLedger.Utils;
using System;


namespace NLedger.Scopus
{
	/// <summary>
	/// A BindScope is a Scope to bind two Scopes.
	/// </summary>
	/// <remarks>Ported from bind_scope_t</remarks>
	public class BindScope : ChildScope
    {
        public BindScope(Scope parent, Scope grandChild) : base(parent)
        {
            GrandChild = grandChild;
            Logger.Current.Debug("scope.symbols", () => String.Format("Binding scope {0} with {1}", parent, grandChild));
        }

        public Scope GrandChild { get; private set; }

        public override string Description
        {
            get { return GrandChild.Description; }
        }

        public override void Define(SymbolKindEnum kind, string name, ExprOp exprOp)
        {
            Parent.Define(kind, name, exprOp);
            GrandChild.Define(kind, name, exprOp);
        }

        /// <summary>
        /// Looks for the Symbol in the GrandChild first.
        /// If not found it will look in the GrandParent.
        /// </summary>
        /// <param name="kind">The kind of symbol</param>
        /// <param name="name">bname of the symbol</param>
        /// <returns></returns>
        public override ExprOp Lookup(SymbolKindEnum kind, string name)
        {
            return GrandChild.Lookup(kind, name) ?? base.Lookup(kind, name);
        }
    }
}