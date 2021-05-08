using System;

namespace Ydi.NvmeMate
{
	public abstract class DisposableBase : IDisposable
	{
		~DisposableBase()
		{
			if (!IsDisposed)
			{
				OnDispose(false);
				IsDisposed = true;
			}
		}

		public bool IsDisposed { get; private set; }

		public void Dispose()
		{
			if (!IsDisposed)
			{
				OnDispose(true);
				IsDisposed = true;
			}
			GC.SuppressFinalize(this);
		}
		protected void ThrowIfDisposed()
		{
			if (IsDisposed)
			{
				throw new ObjectDisposedException(GetType().Name);
			}
		}
		protected abstract void OnDispose(bool disposing);
	}
}
