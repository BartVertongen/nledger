using System;


namespace NLedger
{
	public class ThreadAcquirer : IDisposable
	{
		public ThreadAcquirer(MainApplicationContext context)
		{
			if (context == null)
				throw new ArgumentNullException(nameof(context));
			if (MainApplicationContext.CurrentInstance != null)
				throw new InvalidOperationException("Cannot acquire current thread because it has been already acquired");

			MainApplicationContext.CurrentInstance = context;
		}

		public void Dispose()
		{
			MainApplicationContext.CurrentInstance = null;
		}
	}
}