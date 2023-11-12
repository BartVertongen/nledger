// **********************************************************************************
// Copyright (c) 2015-2021, Dmitry Merzlyakov.  All rights reserved.
// Licensed under the FreeBSD Public License. See LICENSE file included with the distribution for details and disclaimer.
// 
// This file is part of NLedger that is a .Net port of C++ Ledger tool (ledger-cli.org). Original code is licensed under:
// Copyright (c) 2003-2021, John Wiegley.  All rights reserved.
// See LICENSE.LEDGER file included with the distribution for details and disclaimer.
// **********************************************************************************
using System;


namespace NLedger.Utils
{
	/// <summary>
	/// Possible values for the Signal.
	/// </summary>
	/// <remarks>Ported from enum caught_signal_t</remarks>
	public enum CaughtSignalEnum
    {
        NONE_CAUGHT,
        INTERRUPTED,
        PIPE_CLOSED
    }

    public static class CancellationManager
    {
        public static bool IsCancellationRequested
        {
            get { return CaughtSignal != CaughtSignalEnum.NONE_CAUGHT; }
        }

		/// <summary>
		/// Gets the signal from the ApplicationContext
		/// </summary>
		/// <remarks>Ported from extern caught_signal_t caught_signal</remarks>
		public static CaughtSignalEnum CaughtSignal
        {
            get { return MainApplicationContext.Current.CancellationSignal; }
        }

		/// <summary>
		/// Will throw an Exception or Runtime when there was a Canceling Signal.
		/// </summary>
		/// <remarks>Ported from inline void check_for_signal()</remarks>
		public static void CheckForSignal()
        {
            switch (CaughtSignal)
            {
                case CaughtSignalEnum.NONE_CAUGHT:
                    break;
                case CaughtSignalEnum.INTERRUPTED:
                    throw new RuntimeError("Interrupted by user (use Control-D to quit)");
                case CaughtSignalEnum.PIPE_CLOSED:
                    throw new RuntimeError("Pipe terminated");
                default:
                    throw new InvalidOperationException(String.Format("Unknown signal: {0}", CaughtSignal));
            }
        }

        /// <summary>
        /// Removes the current Cancleling Signal.
        /// </summary>
        public static void DiscardCancellationRequest()
        {
            MainApplicationContext.Current.CancellationSignal = CaughtSignalEnum.NONE_CAUGHT;
        }
    }
}