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


namespace NLedger.Extensibility
{
    public sealed class ExtensionProviderSelector
    {
        /// <summary>
        /// Maps a string to an ExtensionProvider
        /// </summary>
        public IDictionary<string, Func<IExtensionProvider>> Providers { get; set; } = new Dictionary<string, Func<IExtensionProvider>>(StringComparer.InvariantCultureIgnoreCase);

		/// <summary>
		/// Adds an ExtensionProvider to the ExtensionProviderSelector
		/// </summary>
		/// <param name="name"> key of the ExtensionProvider for the map</param>
		/// <param name="factory">Reference to a method that creates the Extension Provider</param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException">if the name is empty</exception>
		/// <exception cref="InvalidOperationException">if the provider already exists</exception>
		public ExtensionProviderSelector AddProvider(string name, Func<IExtensionProvider> factory)
        {
            if (String.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));
            if (factory == null)
                throw new ArgumentNullException(nameof(factory));
            //Checks whether the ExtensionProvider with that key is not yet existing
            if (Providers.ContainsKey(name))
                throw new InvalidOperationException($"Provider '{name}' already exists");

            Providers.Add(name, factory);
            return this;
        }

        public Func<IExtensionProvider> GetProvider(string name)
        {
            if (String.IsNullOrWhiteSpace(name))
                return null;

            Func<IExtensionProvider> factory;
            if (!Providers.TryGetValue(name, out factory))
                throw new InvalidOperationException($"No extension provider with name '{name}'");

            return factory;
        }
    }
}