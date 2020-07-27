﻿using NLedger.Scopus;
using NLedger.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLedger.Utility.ServiceAPI
{
    public class ServiceSession : IDisposable
    {
        public ServiceSession(MainApplicationContext mainApplicationContext, IEnumerable<string> args)
        {
            if (mainApplicationContext == null)
                throw new ArgumentNullException(nameof(mainApplicationContext));
            if (args == null)
                throw new ArgumentNullException(nameof(args));

            MainApplicationContext = mainApplicationContext;
            InitializeSession(args);
        }

        public MainApplicationContext MainApplicationContext { get; }
        public GlobalScope GlobalScope { get; private set; }
        public int Status { get; private set; } = 1;

        public bool HasInitializationErrors => Status == 0;
        public bool IsActive => GlobalScope != null && !HasInitializationErrors;

        public ServiceResponse ExecuteCommand(string command)
        {
            if (!IsActive)
                throw new InvalidOperationException("Session is not active");

            return new ServiceResponse(this, command);
        }

        public void Dispose()
        {
            CloseSession();
        }

        private void InitializeSession(IEnumerable<string> args)
        {
            using (MainApplicationContext.AcquireCurrentThread())
            {
                GlobalScope.HandleDebugOptions(args);
                Logger.Current.Info(() => LedgerSessionStarting);

                GlobalScope = new GlobalScope(MainApplicationContext.EnvironmentVariables);
                GlobalScope.Session.FlushOnNextDataFile = true;

                try
                {
                    // Look for options and a command verb in the command-line arguments
                    BindScope boundScope = new BindScope(GlobalScope, GlobalScope.Report);
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
            }
        }

        private void CloseSession()
        {
            if (GlobalScope != null)
            {
                GlobalScope.QuickClose();
                GlobalScope.Dispose();
                GlobalScope = null;

                Logger.Current.Info(() => LedgerSessionEnded);
            }
        }

        private const string LedgerSessionStarting = "Ledger session starting";
        private const string LedgerSessionEnded = "Ledger session ended";
    }
}
