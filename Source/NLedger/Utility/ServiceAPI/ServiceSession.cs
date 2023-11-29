﻿// **********************************************************************************
// Copyright (c) 2015-2021, Dmitry Merzlyakov.  All rights reserved.
// Licensed under the FreeBSD Public License.
// See LICENSE file included with the distribution for details and disclaimer.
// 
// This file is part of NLedger that is a .Net port of C++ Ledger tool (ledger-cli.org).
// Original code is licensed under:
// Copyright (c) 2003-2021, John Wiegley.  All rights reserved.
// See LICENSE.LEDGER file included with the distribution for details and disclaimer.
// **********************************************************************************
using NLedger.Scopus;
using NLedger.Utils;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;


namespace NLedger.Utility.ServiceAPI
{
    /// <summary>
    /// Service API session object that represent parsed ledger journal.
    /// </summary>
    public class ServiceSession : IDisposable
    {
		private const string LedgerSessionStarting = "Ledger session starting";
		private const string LedgerSessionEnded = "Ledger session ended";

		public ServiceEngine ServiceEngine { get; }

		public MainApplicationContext MainApplicationContext { get; }

		public GlobalScope GlobalScope { get; private set; }

		public int Status { get; private set; } = 1;

		public TimeSpan ExecutionTime { get; private set; }

		public string InputText { get; private set; }

		public string OutputText { get; private set; }

		public string ErrorText { get; private set; }
		public ServiceSession(ServiceEngine serviceEngine, IEnumerable<string> args, string inputText, CancellationToken token)
        {
            if (serviceEngine == null)
                throw new ArgumentNullException(nameof(serviceEngine));
            if (args == null)
                throw new ArgumentNullException(nameof(args));

            ServiceEngine = serviceEngine;
            InputText = inputText ?? String.Empty;

            using (new ScopeTimeTracker(time => ExecutionTime = time))
                MainApplicationContext = InitializeSession(args, token);
        }

        public bool HasInitializationErrors => Status > 0;

        public bool IsActive => GlobalScope != null && !HasInitializationErrors;

        public ServiceResponse ExecuteCommand(string command)
        {
            return ExecutingCommand(command, CancellationToken.None);
        }

        public Task<ServiceResponse> ExecuteCommandAsync(string command, CancellationToken token = default(CancellationToken))
        {
            return Task.Run(() => ExecutingCommand(command, token));
        }

        public ServiceQueryResult ExecuteQuery(string query)
        {
            return ExecutingQuery(query, CancellationToken.None);
        }

        public Task<ServiceQueryResult> ExecuteQueryAsync(string query, CancellationToken token = default(CancellationToken))
        {
            return Task.Run(() => ExecutingQuery(query, token));
        }

        public void Dispose()
        {
            CloseSession();
        }

        private MainApplicationContext InitializeSession(IEnumerable<string> args, CancellationToken token)
        {
            //the  memoryStreamManager contains a stream for stdIn, stdOut and stdErr
            using (var memoryStreamManager = new MemoryStreamManager(InputText))
            {
                var context = ServiceEngine.CreateContext(memoryStreamManager);
                token.Register(() => context.CancellationSignal = CaughtSignalEnum.INTERRUPTED);
                using (context.AcquireCurrentThread())
                {
                    //Handle the NLedger debug arguments only
                    GlobalScope.HandleDebugOptions(args);
                    Logger.Current.Info(() => LedgerSessionStarting);

                    GlobalScope = new GlobalScope(context.EnvironmentVariables);
                    GlobalScope.Session.FlushOnNextDataFile = true;

                    try
                    {
                        // Binds the GlobalScope with the Report Scope
                        BindScope boundScope = new BindScope(GlobalScope, GlobalScope.Report);
						// Look for options and a command verb in the command-line arguments
						args = GlobalScope.ReadCommandArguments(boundScope, args);
                        GlobalScope.Session.ReadJournalFiles();
                        Status = 0;
                    }
                    catch (CountError errors)
                    {
                        Status = errors.Count;
                    }
                    catch (Exception err)
                    {
                        GlobalScope.ReportError(err);
                    }

                    OutputText = memoryStreamManager.GetOutputText();
                    ErrorText = memoryStreamManager.GetErrorText();
                    return context;
                }
            }
        }

        private ServiceResponse ExecutingCommand(string command, CancellationToken token)
        {
            if (!IsActive)
                throw new InvalidOperationException("Session is not active");

            return new ServiceResponse(this, command, token);
        }

        private ServiceQueryResult ExecutingQuery(string query, CancellationToken token)
        {
            if (!IsActive)
                throw new InvalidOperationException("Session is not active");

            return new ServiceQueryResult(this, query, token);
        }

        private void CloseSession()
        {
            if (GlobalScope != null && MainApplicationContext != null)
            {
                using (MainApplicationContext.AcquireCurrentThread())
                {
                    GlobalScope.QuickClose();
                    GlobalScope.Dispose();
                    GlobalScope = null;

                    Logger.Current.Info(() => LedgerSessionEnded);
                }
            }
        }
    }
}