﻿using NLedger.Expressions;
using NLedger.Textual;
using NLedger.Utility;
using NLedger.Values;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NLedger.Extensibility.Net
{
    public class NetSession : ExtendedSession
    {
        public NetSession(INamespaceResolver namespaceResolver, IValueConverter valueConverter)
        {
            NamespaceResolver = namespaceResolver ?? throw new ArgumentNullException(nameof(namespaceResolver));
            ValueConverter = valueConverter ?? throw new ArgumentNullException(nameof(valueConverter));
            RootNamespace = new NetNamespaceScope(NamespaceResolver, ValueConverter);
        }

        public INamespaceResolver NamespaceResolver { get; }
        public IValueConverter ValueConverter { get; }
        public NetNamespaceScope RootNamespace { get; }

        public override void DefineGlobal(string name, object value)
        {
            if (String.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));

            Globals[name] = BaseFunctor.Selector(value, ValueConverter);
        }

        public override void Eval(string code, ExtensionEvalModeEnum mode)
        {
            //throw new NotImplementedException();
        }

        public override void ImportOption(string line)
        {
            bool isComment = false;
            var args = StringExtensions.SplitArguments(StringExtensions.NextElement(ref line)).Where(s => !(isComment || (isComment = s[0].IsCommentChar()))).ToArray();

            if (String.Equals(line, "assembly", StringComparison.InvariantCultureIgnoreCase))
            {
                if (args.Length != 1)
                    throw new ParseError("Directive 'import assembly' requires an assembly name as a single argument");

                LoadAssemblyDirective(args[0]);
            }
            else if (String.Equals(line, "file", StringComparison.InvariantCultureIgnoreCase))
            {
                if (args.Length != 1)
                    throw new ParseError("Directive 'import file' requires a file name as a single argument. Use quotes if the file name contains whitespaces.");

                LoadAssemblyFileDirective(args[0]);
            }
            else if (String.Equals(line, "alias", StringComparison.InvariantCultureIgnoreCase))
            {
                if (args.Length != 3 || !String.Equals(args[1], "for", StringComparison.InvariantCultureIgnoreCase))
                    throw new ParseError("Directive 'import alias [alias] for [path]' an alias and a path as parameters.");

                AddAliasDirective(args[0], args[2]);
            }
            else
                throw new ParseError("Directive 'import' for dotNet extension can only contain 'assembly', 'file' or 'alias' clauses.");
        }

        public override void Initialize()
        {
            //throw new NotImplementedException();
        }

        public override bool IsInitialized()
        {
            return true;
        }

        protected override ExprOp LookupFunction(string name)
        {
            BaseFunctor functor;
            if (Globals.TryGetValue(name, out functor))
                return ExprOp.WrapFunctor(functor.ExprFunctor);

            return RootNamespace.Lookup(Scopus.SymbolKindEnum.FUNCTION, name);
        }

        protected virtual void LoadAssemblyDirective(string assemblyName)
        {
            if (String.IsNullOrWhiteSpace(assemblyName))
                throw new ArgumentNullException(nameof(assemblyName));

            var assembly = Assembly.Load(assemblyName);
            if (assembly != null)
                NamespaceResolver.AddAssembly(assembly);
        }

        protected virtual void LoadAssemblyFileDirective(string assemblyFileName)
        {
            if (String.IsNullOrWhiteSpace(assemblyFileName))
                throw new ArgumentNullException(nameof(assemblyFileName));

            var assembly = Assembly.LoadFrom(assemblyFileName);
            if (assembly != null)
                NamespaceResolver.AddAssembly(assembly);
        }

        private void AddAliasDirective(string aliasName, string path)
        {
            if (String.IsNullOrWhiteSpace(aliasName))
                throw new ArgumentNullException(nameof(aliasName));
            if (String.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException(nameof(path));

            if (aliasName.Contains('.'))
                throw new ArgumentException($"Alias '{aliasName}' contains dot(s) that are not allowed in alias names");


            throw new NotImplementedException("TODO");
        }

        private readonly IDictionary<string, BaseFunctor> Globals = new Dictionary<string, BaseFunctor>();
    }
}
