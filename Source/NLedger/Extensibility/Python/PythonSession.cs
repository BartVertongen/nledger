﻿// **********************************************************************************
// Copyright (c) 2015-2021, Dmitry Merzlyakov.  All rights reserved.
// Licensed under the FreeBSD Public License. See LICENSE file included with the distribution for details and disclaimer.
// 
// This file is part of NLedger that is a .Net port of C++ Ledger tool (ledger-cli.org). Original code is licensed under:
// Copyright (c) 2003-2021, John Wiegley.  All rights reserved.
// See LICENSE.LEDGER file included with the distribution for details and disclaimer.
// **********************************************************************************
using System;
using System.Linq;
using System.Reflection;
using IronPython.Hosting;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using Microsoft.Scripting.Runtime;
using NLedger.Expressions;
using NLedger.Extensibility.Net;
using NLedger.Textual;
using NLedger.Utility;
using NLedger.Utils;


namespace NLedger.Extensibility.Python
{
    public class PythonSession : ExtendedSession
    {
        public static readonly string LoggerCategory = "extension.python";

        //private readonly IDictionary<string, BaseFunctor> Globals = new Dictionary<string, BaseFunctor>();

        //public INamespaceResolver NamespaceResolver { get; }

        //public IValueConverter ValueConverter { get; }

        //public PythonNamespaceScope RootNamespace { get; }

        public ScriptEngine Engine { get; private set; }

        public ScriptScope Scope { get; private set; }


		public static new PythonSession Current => ExtendedSession.Current as PythonSession;

        /// <summary>
        /// Creates Standalone .Python session that provides a scope where Ledger domain objects 
        /// can function under the control of custom code.
        /// </summary>
        public static PythonSession CreateStandaloneSession(Func<MainApplicationContext> contextFactory = null)
        {
            return CreateStandaloneSession(() => new PythonSession());
        }

        public PythonSession()
        {
            //This fails, try another way , tries to imprt something  of zip module
            this.Engine = IronPython.Hosting.Python.CreateEngine();
            this.Scope = Engine.CreateScope();

			/*NamespaceResolver = namespaceResolver ?? throw new ArgumentNullException(nameof(namespaceResolver));
            ValueConverter = valueConverter ?? throw new ArgumentNullException(nameof(valueConverter));
            RootNamespace = new PythonNamespaceScope(NamespaceResolver, ValueConverter);*/
		}

        public override void DefineGlobal(string name, object value)
        {
            if (String.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));
            this.Scope.SetVariable(name, value);

            //Globals[name] = BaseFunctor.Selector(value, ValueConverter);
        }

        public override void Eval(string code, ExtensionEvalModeEnum mode)
        {
			// Eval statement in the "basic" .Net session is ignored but can be implemented in derived classes
			if (mode == ExtensionEvalModeEnum.EvalExpr)
            {
				ScriptEngine oEngine = IronPython.Hosting.Python.CreateEngine();
				ScriptSource oSource = oEngine.CreateScriptSourceFromString(code);
				dynamic dynResult = oSource.Execute();
			}
            else if (mode == ExtensionEvalModeEnum.EvalMulti)
            {
                //TODO: make this work
				ScriptEngine oEngine = IronPython.Hosting.Python.CreateEngine();
				ScriptSource oSource = oEngine.CreateScriptSourceFromString(code, SourceCodeKind.Statements);
                oSource.Execute(this.Scope);
				//CompiledCode oCompiledCode = oSource.Compile();
				//oEngine.
			}
        }

        public override void ImportOption(string line)
        {
            bool isComment = false;
            var args = StringExtensions.SplitArguments(StringExtensions.NextElement(ref line))
                                       .Where(s => !(isComment || (isComment = s[0].IsCommentChar())))
                                       .ToArray();

            if (String.Equals(line, "assemblies", StringComparison.InvariantCultureIgnoreCase))
            {
                if (args.Length != 0)
                    throw new ParseError("Directive 'import assemblies' does not require any arguments");

                //LoadAssembliesDirective();
            }
            else if (String.Equals(line, "assembly", StringComparison.InvariantCultureIgnoreCase))
            {
                if (args.Length != 1)
                    throw new ParseError("Directive 'import assembly' requires an assembly name as a single argument");

                //LoadAssemblyDirective(args[0]);
            }
            else if (String.Equals(line, "file", StringComparison.InvariantCultureIgnoreCase))
            {
                if (args.Length != 1)
                    throw new ParseError("Directive 'import file' requires a file name as a single argument. Use quotes if the file name contains whitespaces.");

                //LoadAssemblyFileDirective(args[0]);
            }
            else if (String.Equals(line, "alias", StringComparison.InvariantCultureIgnoreCase))
            {
                if (args.Length != 3 || !String.Equals(args[1], "for", StringComparison.InvariantCultureIgnoreCase))
                    throw new ParseError("Directive 'import alias [alias] for [path]' requires an alias and a path as parameters.");

                //AddAliasDirective(args[0], args[2]);
            }
            else
                throw new ParseError("Directive 'import' for dotNet extension can only contain 'assemblies', 'assembly', 'file' or 'alias' clauses.");
        }

        //We have to override it
        //We could create the engine here
        public override void Initialize()
        {		
            //The scope depend on what we are going to to with the Engine.

			//var source = engine.CreateScriptSourceFromFile(AppDomain.CurrentDomain.BaseDirectory + “myscript.py”);
			//var scope = engine.CreateScope();
			//source.Execute(scope);
			//var myclass = scope.GetVariable(“reversor”);
		}

        public override bool IsInitialized() { return true;  }

        protected override ExprOp LookupFunction(string name)
        {
            //BaseFunctor functor;
            /*if (Globals.TryGetValue(name, out functor))
                return ExprOp.WrapFunctor(functor.ExprFunctor);

            return RootNamespace.Lookup(Scopus.SymbolKindEnum.FUNCTION, name);*/
            return null;
        }

        /*protected virtual void LoadAssembliesDirective()
        {
            NamespaceResolver.AddAllAssemblies();
        }*/

        /*protected virtual void LoadAssemblyDirective(string assemblyName)
        {
            if (String.IsNullOrWhiteSpace(assemblyName))
                throw new ArgumentNullException(nameof(assemblyName));

            try
            {
                var assembly = Assembly.Load(assemblyName);
                if (assembly != null)
                    NamespaceResolver.AddAssembly(assembly);
            }
            catch (Exception ex)
            {
                Logger.Current.Debug(LoggerCategory, () => $"Error loading assembly {assemblyName}: {ex.ToString()}");
            }
        }*/

        /*protected virtual void LoadAssemblyFileDirective(string assemblyFileName)
        {
            if (String.IsNullOrWhiteSpace(assemblyFileName))
                throw new ArgumentNullException(nameof(assemblyFileName));

            var assembly = Assembly.LoadFrom(assemblyFileName);
            if (assembly != null)
                NamespaceResolver.AddAssembly(assembly);
        }*/

        /*protected virtual void AddAliasDirective(string aliasName, string path)
        {
            if (String.IsNullOrWhiteSpace(aliasName))
                throw new ArgumentNullException(nameof(aliasName));
            if (String.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException(nameof(path));

            if (aliasName.Contains('.'))
                throw new ArgumentException($"Alias '{aliasName}' contains dot(s) that are not allowed in alias names");

            //Globals[aliasName] = PathFunctor.ParsePath(path, NamespaceResolver, ValueConverter);
        }*/
    }
}