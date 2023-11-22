// **********************************************************************************
// Copyright (c) 2015-2021, Dmitry Merzlyakov.  All rights reserved.
// Licensed under the FreeBSD Public License. See LICENSE file included with the distribution for details and disclaimer.
// 
// This file is part of NLedger that is a .Net port of C++ Ledger tool (ledger-cli.org). Original code is licensed under:
// Copyright (c) 2003-2021, John Wiegley.  All rights reserved.
// See LICENSE.LEDGER file included with the distribution for details and disclaimer.
// **********************************************************************************
using NLedger.Commodities;
using NLedger.Extensibility;
using NLedger.Formatting;
using NLedger.Scopus;
using NLedger.Times;
using NLedger.Utility;
using NLedger.Utils;
using System;
using System.Collections.Generic;


namespace NLedger
{
    public sealed class MainApplicationContext
    {
		[ThreadStatic]
		public static MainApplicationContext CurrentInstance;
		private static readonly IDictionary<string, string> Empty = new Dictionary<string, string>();

		private CommodityPool _CommodityPool;
		private Commodity.CommodityDefaults _CommodityDefaults;
		private IApplicationServiceProvider _ApplicationServiceProvider;
		private IDictionary<string, string> _EnvironmentVariables;

		public static MainApplicationContext Current => CurrentInstance;

        public MainApplicationContext(IApplicationServiceProvider applicationServiceProvider = null)
        {
            _ApplicationServiceProvider = applicationServiceProvider ?? new ApplicationServiceProvider();
        }

        // For GlobalScope
        public bool ArgsOnly { get; set; }

        public string InitFile { get; set; }

        // For CommodityPool
        public CommodityPool CommodityPool
        {
            get { return _CommodityPool ?? (_CommodityPool = new CommodityPool()); }
            set { _CommodityPool = value; }
        }

        public Commodity.CommodityDefaults CommodityDefaults
        {
            get { return _CommodityDefaults ?? (_CommodityDefaults = new Commodity.CommodityDefaults()); }
            set { _CommodityDefaults = value; }
        }

        // For FileSystem
        public bool IsAtty { get; set; } = true;

        // For Times
        public TimesCommon TimesCommon { get; set; } = new TimesCommon();

        // For Scope
        public Scope DefaultScope { get; set; }

        public Scope EmptyScope { get; set; }

        // For Item
        public bool UseAuxDate { get; set; }

        // For Logger & Validator
        public ILogger Logger { get; set; } = new Logger();

        public bool IsVerifyEnabled { get; set; }

        // For Format
        public FormatElisionStyleEnum DefaultStyle { get; set; }

        public bool DefaultStyleChanged { get; set; }

        // For datetime extensions
        public TimeZoneInfo TimeZone { get; set; }

        // For Error Context
        public ErrorContext ErrorContext { get; set; } = new ErrorContext();

        // Cancellation Management
        private volatile CaughtSignalEnum _CancellationSignal;

        public CaughtSignalEnum CancellationSignal
        {
            get { return _CancellationSignal; }
            set { _CancellationSignal = value; }
        }

        // Default Pager Name
        public string DefaultPager { get; set; }

        // Environment Variables
        public IDictionary<string, string> EnvironmentVariables
        {
            get { return _EnvironmentVariables ?? Empty; }
        }

        public void SetEnvironmentVariables(IDictionary<string, string> variables)
        {
            _EnvironmentVariables = new Dictionary<string, string>(variables ?? Empty, StringComparer.InvariantCultureIgnoreCase);
        }

        public ExtendedSession ExtendedSession { get; private set; }

        public ExtendedSession SetExtendedSession(ExtendedSession extendedSession)
        {
            return ExtendedSession = extendedSession;
        }

        // Abstract Application Services
        public IApplicationServiceProvider ApplicationServiceProvider => _ApplicationServiceProvider;

        // Primarily, for testing purposes only
        public void SetApplicationServiceProvider(IApplicationServiceProvider applicationServiceProvider)
        {
            if (applicationServiceProvider == null)
                throw new ArgumentNullException(nameof(applicationServiceProvider));

            _ApplicationServiceProvider = applicationServiceProvider;
        }

        // Request an excluse access to the current thread
        public ThreadAcquirer AcquireCurrentThread()
        {
            return new ThreadAcquirer(this);
        }

        public MainApplicationContext Clone(IApplicationServiceProvider applicationServiceProvider = null)
        {
            var context = new MainApplicationContext(applicationServiceProvider ?? _ApplicationServiceProvider);
            context.ArgsOnly = ArgsOnly;
            context.InitFile = InitFile;
            context.CommodityPool = CommodityPool;
            context.CommodityDefaults = CommodityDefaults;
            context.IsAtty = IsAtty;
            context.TimesCommon = TimesCommon;
            context.DefaultScope = DefaultScope;
            context.EmptyScope = EmptyScope;
            context.Logger = Logger;
            context.IsVerifyEnabled = IsVerifyEnabled;
            context.DefaultStyle = DefaultStyle;
            context.DefaultStyleChanged = DefaultStyleChanged;
            context.TimeZone = TimeZone;
            context.ErrorContext = ErrorContext;
            context.CancellationSignal = CancellationSignal;
            context.DefaultPager = DefaultPager;
            context._EnvironmentVariables = _EnvironmentVariables;
            context.ExtendedSession = ExtendedSession;
            return context;
        }
    }
}