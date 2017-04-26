using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace DynamicConnections.NutriStyle.CRM2011.CreateEntities
{
    public class Public : IDisposable
    {
        private IntPtr handle;
        private Component component = new Component();
        private bool disposed = false;

        public Public()
        {
        } // end - public ClassPublic()

        public Public(IntPtr handle)
        {
            this.handle = handle;
        } // end - public ClassPublic()

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        } // end - public void Dispose()

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    component.Dispose();
                }
                CloseHandle(handle);
                handle = IntPtr.Zero;
                disposed = true;
            }
        } // end - protected virtual void Dispose()

        [System.Runtime.InteropServices.DllImport("Kernel32")]
        private extern static Boolean CloseHandle(IntPtr handle);
        ~Public()
        {
            Dispose(false);
        } // end - private extern static Boolean CloseHandle()

        // more methods !!!
        // create more public methods here to be used for more classes!

    } // class ClassPublic
}
