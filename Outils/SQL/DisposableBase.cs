using System;

namespace Outils.SQL
{
    public abstract class DisposableBase : IDisposable
    {
        private bool _disposed;

        protected DisposableBase()
        {
            _disposed = false;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing) DisposeManagedResources();
                DisposeUnmanagedResources();
            }

            _disposed = true;
        }

        protected virtual void DisposeManagedResources()
        {
        }

        protected virtual void DisposeUnmanagedResources()
        {
        }

        ~DisposableBase()
        {
            Dispose(false);
        }
    }
}