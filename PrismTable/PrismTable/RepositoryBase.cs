using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;

namespace PrismTable
{
    public class RepositoryBase : IDisposable
    {
        /// <summary>
        /// Reference to data access instance (Entity Framework) and loads at instantiation.
        /// </summary>
        /// 
        protected object _oDbContext;

        public RepositoryBase()
        {
            ObjectHandle handle = System.Activator.CreateInstance(Connect.settingsObject.DbContextAssembly, Connect.settingsObject.DbContextClass);
            _oDbContext = handle.Unwrap();
        }

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources
                if (_oDbContext != null)
                {
                    ((DbContext)_oDbContext).Dispose();
                    _oDbContext = null;
                }
            }
        }

        #endregion
    }
}
