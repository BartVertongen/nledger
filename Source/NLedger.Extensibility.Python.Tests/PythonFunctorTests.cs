// **********************************************************************************
// Copyright (c) 2015-2021, Dmitry Merzlyakov.  All rights reserved.
// Licensed under the FreeBSD Public License. See LICENSE file included with the distribution for details and disclaimer.
// 
// This file is part of NLedger that is a .Net port of C++ Ledger tool (ledger-cli.org). Original code is licensed under:
// Copyright (c) 2003-2021, John Wiegley.  All rights reserved.
// See LICENSE.LEDGER file included with the distribution for details and disclaimer.
// **********************************************************************************
using NLedger.Tests;
using Xunit;


namespace NLedger.Extensibility.Python.Tests
{
    public class PythonFunctorTests : TestFixture
    {
        public PythonFunctorTests()
        {
            //Assert.True(PythonConnector.Current.IsAvailable);
            //thonConnectionContext = PythonConnector.Current.Connect();
            //PythonConnector.Current.KeepAlive = false;
        }

        //public PythonConnectionContext PythonConnectionContext { get; }

        public override void Dispose()
        {
            //PythonConnectionContext.Dispose();
            base.Dispose();
        }

        [PythonFact]
        public void PythonFunctor_Constructor_PopulatesProperties()
        {
            using (var session = new PythonSession())
            {
                //var converter = new PythonValueConverter(session);

                //Make a python object from a NET object
                //var obj = PyObject.FromManagedObject("some-string");

                var name = "some-name";

                //Make a function with the previous object as argument
                //var functor = new PythonFunctor(name, obj, converter);

                //Assert.Equal(name, functor.Name);

                //Assert.Equal(obj.ToString(), functor.Obj.ToString());
                //Assert.Equal(converter, functor.PythonValueConverter);
            }                
        }

        [PythonFact]
        public void PythonFunctor_ExprFunc_ReturnsValue()
        {
            using (var session = new PythonSession())
            {
				//var converter = new PythonValueConverter(session);

				//Make a python object from a NET object
				//var obj = PyObject.FromManagedObject("some-string");

                //Make a Function with previous object as argument
				//var functor = new PythonFunctor("some-name", obj, converter);

                //Get the argument of the function
				//var val = functor.ExprFunctor(session);

                //Check the name of the argument
				//Assert.Equal("some-string", val.AsString);
			}
		}
    }
}