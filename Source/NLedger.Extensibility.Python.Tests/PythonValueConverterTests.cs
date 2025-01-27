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
using Microsoft.Scripting.Hosting;
using NLedger.Tests;
using Xunit;


namespace NLedger.Extensibility.Python.Tests
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// To run the test we need 
    /// </remarks>
    public class PythonValueConverterTests : TestFixture
	{
        public PythonValueConverterTests()
        {
        }

        [Fact]
        public void PythonValues_PythonBooleanTrue_ShouldbeNETBoolean()
        {
            // Arrange
			using PythonSession oSession = new PythonSession();
			ScriptSource oSource = oSession.Engine.CreateScriptSourceFromString("True");

            // Action
			dynamic dynVal = oSource.Execute();

            // Assert
			Assert.True(dynVal.GetType() == typeof(bool));
		}

		[Fact]
		public void PythonValues_PythonBooleanFalse_ShouldbeNETBoolean()
		{
			// Arrange
			using PythonSession oSession = new PythonSession();
			ScriptSource oSource = oSession.Engine.CreateScriptSourceFromString("False");

			// Action
			dynamic dynVal = oSource.Execute();

			// Assert
			Assert.True(dynVal.GetType() == typeof(bool));
		}

		[Fact]
		public void PythonValues_PythonBooleanTrue_ShouldbeNETtrue()
		{
			// Arrange
			using PythonSession oSession = new PythonSession();
			ScriptSource oSource = oSession.Engine.CreateScriptSourceFromString("True");

			// Action
			dynamic dynVal = oSource.Execute();

			// Assert
			Assert.True(dynVal);
		}

		[Fact]
		public void PythonValues_PythonBooleanFalse_ShouldbeNETfalse()
		{
			// Arrange
			using PythonSession oSession = new PythonSession();
			ScriptSource oSource = oSession.Engine.CreateScriptSourceFromString("False");

			// Action
			dynamic dynVal = oSource.Execute();

			// Assert
			Assert.False(dynVal);
		}

        //The difference between Fact and PythonFact is the textFicture to create an MainApplicationContext 
		[Fact]
        ///<remarks>In fact the type is Int32</remarks>
		public void PythonValues_Pythonint_ShouldbeNETint()
		{
			// Arrange
			using PythonSession oSession = new PythonSession();
			ScriptSource oSource = oSession.Engine.CreateScriptSourceFromString("10");

			// Action
			dynamic dynVal = oSource.Execute();

			// Assert
			Assert.True(dynVal.GetType() == typeof(int));
		}

		/*[PythonFact]
        public void PythonValueConverter_GetObject_Conversions()
        {
            try
            {
                PythonSession.Current.Initialize();
                //To convert Python values to NET values

                // Boolean/False
                val = Value.Get(false);
                Assert.Equal(ValueTypeEnum.Boolean, val.Type);
                //py = converter.GetObject(val);
                Assert.Equal("bool", py.GetType().Name);
                Assert.Equal("False", py.ToString());

                // Boolean/True
                val = Value.Get(true);
                Assert.Equal(ValueTypeEnum.Boolean, val.Type);
                //py = converter.GetObject(val);
                Assert.Equal("bool", py.GetType().Name);
				Assert.Equal("True", py.ToString());

                // Integer/10
                val = Value.Get(10);
                Assert.Equal(ValueTypeEnum.Integer, val.Type);
                //py = converter.GetObject(val);
                Assert.Equal("int", py.GetType().Name);
                Assert.Equal("10", py.ToString());

                // DateTime/2021-10-20 23:55:50
                val = Value.Get(new DateTime(2021, 10, 20, 23, 55, 50));
                Assert.Equal(ValueTypeEnum.DateTime, val.Type);
                //py = converter.GetObject(val);
                Assert.Equal("datetime", py.GetType().Name);
                Assert.Equal("2021-10-20 23:55:50", py.ToString());

                // Date/2021-10-20
                val = Value.Get(new Date(2021, 10, 20));
                Assert.Equal(ValueTypeEnum.Date, val.Type);
                //py = converter.GetObject(val);
                //Assert.Equal("date", py.GetPythonTypeName());
                Assert.Equal("2021-10-20", py.ToString());

                // String/some-string
                val = Value.StringValue("some-string");
                Assert.Equal(ValueTypeEnum.String, val.Type);
                //py = converter.GetObject(val);
                Assert.Equal("str", py.GetType().Name);
                Assert.Equal("some-string", py.ToString());

                // Amount/10
                val = Value.Get(new Amount(10));
                Assert.Equal(ValueTypeEnum.Amount, val.Type);
                //py = converter.GetObject(val);
                Assert.Equal("Amount", py.GetType().Name);
                Assert.Equal("10", py.ToString());

                // Balance/10
                val = Value.Get(new Balance(10));
                Assert.Equal(ValueTypeEnum.Balance, val.Type);
                //py = converter.GetObject(val);
                Assert.Equal("Balance", py.GetType().Name);
                Assert.Equal("10", py.ToString());

                // Mask/mask
                val = Value.MaskValue("mask");
                Assert.Equal(ValueTypeEnum.Mask, val.Type);
                //py = converter.GetObject(val);
                Assert.Equal("Value", py.GetType().Name);
                Assert.Equal("mask", py.ToString());

                // Sequence
                val = Value.Get(new List<Value>() { Value.Get(10), Value.Get(20), Value.Get(30) });
                Assert.Equal(ValueTypeEnum.Sequence, val.Type);
                //py = converter.GetObject(val);
                Assert.Equal("list", py.GetType().Name);
                Assert.Equal("[10, 20, 30]", py.ToString());

                // Post
                val = Value.Get(new Post());
                Assert.Equal(ValueTypeEnum.Scope, val.Type);
                //py = converter.GetObject(val);
                Assert.Equal("Posting", py.GetType().Name);

                // Xact
                val = Value.Get(new Xact());
                Assert.Equal(ValueTypeEnum.Scope, val.Type);
                //py = converter.GetObject(val);
                Assert.Equal("Transaction", py.GetType().Name);

                // Account
                val = Value.Get(new Account());
                Assert.Equal(ValueTypeEnum.Scope, val.Type);
                //py = converter.GetObject(val);
                Assert.Equal("Account", py.GetType().Name);

                // PeriodXact
                val = Value.Get(new PeriodXact());
                Assert.Equal(ValueTypeEnum.Scope, val.Type);
                //py = converter.GetObject(val);
                Assert.Equal("PeriodicTransaction", py.GetType().Name);

                // AutoXact
                val = Value.Get(new AutoXact());
                Assert.Equal(ValueTypeEnum.Scope, val.Type);
                //py = converter.GetObject(val);
                Assert.Equal("AutomatedTransaction", py.GetType().Name);
            }

        }*/

		[PythonFact]
        public void PythonValueConverter_GetValue_Conversions()
        {
            //PythonSession.Current.Initialize();
            using (PythonSession oSession = new PythonSession())
            {
				//Assert.Equal(ValueTypeEnum.Void, converter.GetValue(null).Type);
				//Assert.Equal(ValueTypeEnum.Void, converter.GetValue(Runtime.None).Type);

				//REM all these should be different tests

				//Make a Python module in NET
				//var scope = PythonSession.Current.MainModule.ModuleObject;

				//Import datetime in the module
				//scope.Import("datetime");

				//ScriptEngine oEngine = IronPython.Hosting.Python.CreateEngine();
				ScriptSource oSource = oSession.Engine.CreateScriptSourceFromString("False");
				dynamic dynVal = oSource.Execute();
                Assert.True(dynVal.GetType() == typeof(bool));


                /*py = scope.Eval("'some-string'");
                //val = converter.GetValue(py);
                Assert.Equal(ValueTypeEnum.String, val.Type);
                Assert.Equal("some-string", val.AsString);*/


                /*py = scope.Eval("datetime.date(2021, 10, 20)");
                //val = converter.GetValue(py);
                Assert.Equal(ValueTypeEnum.Date, val.Type);
                Assert.Equal(new Date(2021, 10, 20), val.AsDate);*/


                /*py = scope.Eval("datetime.datetime(2021, 10, 20, 23, 59, 33)");
                //val = converter.GetValue(py);
                Assert.Equal(ValueTypeEnum.DateTime, val.Type);
                Assert.Equal(new DateTime(2021, 10, 20, 23, 59, 33), val.AsDateTime);*/


                /*py = scope.Eval("ledger.Balance(10)");
                //val = converter.GetValue(py);
                Assert.Equal(ValueTypeEnum.Balance, val.Type);
                Assert.Equal("10", val.AsBalance.ToString().Trim());*/


                /*py = scope.Eval("ledger.Amount(10)");
                //val = converter.GetValue(py);
                Assert.Equal(ValueTypeEnum.Amount, val.Type);
                Assert.Equal("10", val.AsAmount.ToString());*/


                /*py = scope.Eval("ledger.Mask('mask')");
                //val = converter.GetValue(py);
                Assert.Equal(ValueTypeEnum.Mask, val.Type);
                Assert.Equal("mask", val.AsMask.ToString());*/


                /*py = scope.Eval("ledger.Posting()");
                //val = converter.GetValue(py);
                Assert.Equal(ValueTypeEnum.Scope, val.Type);*/


                /*py = scope.Eval("ledger.Transaction()");
                //val = converter.GetValue(py);
                Assert.Equal(ValueTypeEnum.Scope, val.Type);*/


                /*py = scope.Eval("ledger.PeriodicTransaction()");
                //val = converter.GetValue(py);
                Assert.Equal(ValueTypeEnum.Scope, val.Type);*/


                /*py = scope.Eval("ledger.AutomatedTransaction()");
                //val = converter.GetValue(py);
                Assert.Equal(ValueTypeEnum.Scope, val.Type);*/

                /*py = scope.Eval("ledger.Account()");
                //val = converter.GetValue(py);
                Assert.Equal(ValueTypeEnum.Scope, val.Type);*/


                /*py = scope.Eval("ledger.Value(10)");
                //val = converter.GetValue(py);
                Assert.Equal(ValueTypeEnum.Integer, val.Type);*/


                /*py = scope.Eval("[10, 20, 30]");
                //val = converter.GetValue(py);
                Assert.Equal(ValueTypeEnum.Sequence, val.Type);
                Assert.Equal("20", val.AsSequence[1].ToString());*/
            }
        }
    }
}