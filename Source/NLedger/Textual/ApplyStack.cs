// **********************************************************************************
// Copyright (c) 2015-2021, Dmitry Merzlyakov.  All rights reserved.
// Licensed under the FreeBSD Public License. See LICENSE file included with the distribution for details and disclaimer.
// 
// This file is part of NLedger that is a .Net port of C++ Ledger tool (ledger-cli.org). Original code is licensed under:
// Copyright (c) 2003-2021, John Wiegley.  All rights reserved.
// See LICENSE.LEDGER file included with the distribution for details and disclaimer.
// **********************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;


namespace NLedger.Textual
{
	/// <summary>
	/// A Stack of Applications.
	/// </summary>
	/// <remarks>Ported from the type std::list<application_t> apply_stack (textual.cc)</remarks>
	public class ApplyStack
    {
		private readonly Stack<Tuple<string, object>> Items;

		public ApplyStack(ApplyStack parent = null)
        {
            Parent = parent;
            Items = new Stack<Tuple<string, object>>();
        }

        public ApplyStack Parent { get; private set; }

        /// <summary>
        /// Gives the number of Items in the Stack.
        /// </summary>
        public int Size
        {
            get { return Items.Count; }
        }

		/// <summary>
		/// Gets all items of a given type from the stack and this parent, all the way up.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public IEnumerable<T> GetApplications<T>()
        {
            List<T> items = Items.
                Where(item => item.Item2 != null && item.Item2.GetType() == typeof(T)).
                Select(item => (T)item.Item2).ToList();

            if (Parent != null)
                items.AddRange(Parent.GetApplications<T>());

            return items;
        }

        /// <summary>
        /// Return the first application in the stack of the given type.
        /// If nothing found we try in the parent Stack.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetApplication<T>()
        {
            Tuple<string,object> result = Items.FirstOrDefault(item => item.Item2 != null && item.Item2.GetType() == typeof(T));

            if (result != null)
                return (T)result.Item2;
            else
                return Parent != null ? Parent.GetApplication<T>() : default(T);
        }

        /// <summary>
        /// Adds an Item to the Stack
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void PushFront<T>(string key, T value)
        {
            Items.Push(new Tuple<string, object>(key, value));
        }

        /// <summary>
        /// Removes an Item from the current stack.
        /// </summary>
        public void PopFront()
        {
            Items.Pop();
        }

        /// <summary>
        /// Checks if the Stack top Item has the requested type.
        /// </summary>
        /// <typeparam name="T">requested type</typeparam>
        /// <returns>tue if found.</returns>
        public bool IsFrontType<T>()
        {
            return Items.Any() && Items.Peek().Item2 is T;
        }

        public T Front<T>()
        {
            return (T)Items.Peek().Item2;
        }

        public string FrontLabel()
        {
            return Items.Peek().Item1;
        }
    }
}