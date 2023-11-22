
using System;
using NLedger.Abstracts;
using NLedger.Abstracts.Impl;
using NLedger.Extensibility;


namespace NLedger
{
	public class ApplicationServiceProvider : IApplicationServiceProvider
	{
		public ApplicationServiceProvider(Func<IQuoteProvider> quoteProviderFactory = null
			, Func<IProcessManager> processManagerFactory = null
			, Func<IManPageProvider> manPageProviderFactory = null
			, Func<IVirtualConsoleProvider> virtualConsoleProviderFactory = null
			, Func<IFileSystemProvider> fileSystemProviderFactory = null
			, Func<IPagerProvider> pagerProviderFactory = null
			, Func<IExtensionProvider> extensionProviderFactory = null)
		{
			_QuoteProvider = new Lazy<IQuoteProvider>(quoteProviderFactory ?? (() => new QuoteProvider()));
			_ProcessManager = new Lazy<IProcessManager>(processManagerFactory ?? (() => new ProcessManager()));
			_ManPageProvider = new Lazy<IManPageProvider>(manPageProviderFactory ?? (() => new ManPageProvider()));
			_VirtualConsoleProvider = new Lazy<IVirtualConsoleProvider>(virtualConsoleProviderFactory ?? (() => new VirtualConsoleProvider()));
			_FileSystemProvider = new Lazy<IFileSystemProvider>(fileSystemProviderFactory ?? (() => new FileSystemProvider()));
			_PagerProvider = new Lazy<IPagerProvider>(pagerProviderFactory ?? (() => new PagerProvider()));
			_ExtensionProvider = new Lazy<IExtensionProvider>(extensionProviderFactory ?? EmptyExtensionProvider.CurrentFactory);
		}

		public IQuoteProvider QuoteProvider => _QuoteProvider.Value;

		public IProcessManager ProcessManager => _ProcessManager.Value;

		public IManPageProvider ManPageProvider => _ManPageProvider.Value;

		public IVirtualConsoleProvider VirtualConsoleProvider => _VirtualConsoleProvider.Value;

		public IFileSystemProvider FileSystemProvider => _FileSystemProvider.Value;

		public IPagerProvider PagerProvider => _PagerProvider.Value;

		public IExtensionProvider ExtensionProvider => _ExtensionProvider.Value;

		private readonly Lazy<IQuoteProvider> _QuoteProvider;
		private readonly Lazy<IProcessManager> _ProcessManager;
		private readonly Lazy<IManPageProvider> _ManPageProvider;
		private readonly Lazy<IVirtualConsoleProvider> _VirtualConsoleProvider;
		private readonly Lazy<IFileSystemProvider> _FileSystemProvider;
		private readonly Lazy<IPagerProvider> _PagerProvider;
		private readonly Lazy<IExtensionProvider> _ExtensionProvider;
	}
}