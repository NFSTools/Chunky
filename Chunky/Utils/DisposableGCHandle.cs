using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace Chunky.Utils
{
    [SecurityPermissionAttribute(SecurityAction.InheritanceDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
    public class DisposableGcHandle : IDisposable
    {
        private GCHandle _handle;

        public DisposableGcHandle(object value)
        {
            _handle = GCHandle.Alloc(value, GCHandleType.Normal);
        }

        public DisposableGcHandle(object value, GCHandleType type)
        {
            _handle = GCHandle.Alloc(value, type);
        }

        public IntPtr AddrOfPinnedObject()
        {
            return _handle.AddrOfPinnedObject();
        }

        public void Free()
        {
            _handle.Free();
        }

        #region Properties

        public object Target
        {
            get => _handle.Target;
            set => _handle.Target = value;
        }

        public bool IsAllocated => _handle.IsAllocated;

        #endregion

        #region DisposePattern

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
                if (_handle.IsAllocated)
                    _handle.Free();

            GC.SuppressFinalize(this);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        public void Close()
        {
            Dispose(true);
        }

        ~DisposableGcHandle()
        {
            Dispose(false);
        }

        #endregion
    }
}