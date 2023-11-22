using NLedger.Abstracts;
using NLedger.Extensibility;


namespace NLedger
{
	public interface IApplicationServiceProvider
	{
		IQuoteProvider QuoteProvider { get; }

		IProcessManager ProcessManager { get; }

		IManPageProvider ManPageProvider { get; }

		IVirtualConsoleProvider VirtualConsoleProvider { get; }

		IFileSystemProvider FileSystemProvider { get; }

		IPagerProvider PagerProvider { get; }

		IExtensionProvider ExtensionProvider { get; }
	}
}