using BigBrother.DalEF4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigBrother.DalEF4.Repositories
{
    public class RepositoryBase : IDisposable
    {
        /// <summary>
        /// Reference to data access instance (Entity Framework) and loads at instantiation.
        /// </summary>
        protected BigBrotherEntities _dbContext = new BigBrotherEntities();


        #region IDisposable Members

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
            if (disposing) {
                // free managed resources
                if (_dbContext != null) {
                    _dbContext.Dispose();
                    _dbContext = null;
                }
            }
        }

        #endregion
    }
}
