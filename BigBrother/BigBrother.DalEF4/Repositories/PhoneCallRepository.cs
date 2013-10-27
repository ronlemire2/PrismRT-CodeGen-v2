using BigBrother.DalEF4;
using BigBrother.DalInterface.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigBrother.DalEF4.Repositories
{
    public class PhoneCallRepository : RepositoryBase, IPhoneCallRepository
    {
        // Get all PhoneCalls
        public IEnumerable<DalInterface.Models.PhoneCall> GetPhoneCalls() {
            var  daPhoneCalls = default(IEnumerable<PhoneCall>);

            daPhoneCalls = _dbContext.PhoneCalls
                .OrderBy(e => e.Id);
            IEnumerable<DalInterface.Models.PhoneCall> iPhoneCalls = PhoneCallMapper.MapDataAccessPhoneCallsToDalInterfacePhoneCalls(daPhoneCalls);

            return iPhoneCalls;
        }

        // Get a PhoneCall by Id
        public DalInterface.Models.PhoneCall GetPhoneCall(int phoneCallId) {
            PhoneCall daPhoneCall = default(PhoneCall);

            daPhoneCall = _dbContext.PhoneCalls
               .Where(e => e.Id == phoneCallId).FirstOrDefault();

            return PhoneCallMapper.MapDataAccessPhoneCallToDalInterfacePhoneCall(daPhoneCall);
        }

        // Create a new PhoneCall
        public DalInterface.Models.PhoneCall Create(DalInterface.Models.PhoneCall iPhoneCall) {
            var result = 0;

            PhoneCall daPhoneCall = PhoneCallMapper.MapDalInterfacePhoneCallToDataAccessPhoneCall(iPhoneCall);

            try {
                this._dbContext.PhoneCalls.Add(daPhoneCall);
                result = this._dbContext.SaveChanges();

                // DbContext sets Id to the new Identity value.
                iPhoneCall.Id = daPhoneCall.Id;
            }
            catch (System.Data.UpdateException ex) {
                if (ex.InnerException != null && ex.InnerException is System.Data.SqlClient.SqlException
                   && ((System.Data.SqlClient.SqlException)ex.InnerException).ErrorCode == 8152)
                    throw ex.InnerException;
                else
                    throw ex;
            }

            return iPhoneCall;
        }

        // Update an existing PhoneCall
        public int Update(DalInterface.Models.PhoneCall iPhoneCall) {
            var result = 0;
            PhoneCall daLoadedPhoneCall = null;
            PhoneCall daPhoneCall = PhoneCallMapper.MapDalInterfacePhoneCallToDataAccessPhoneCall(iPhoneCall);

            try {
                // Load object into context (entity framework) 
                daLoadedPhoneCall = _dbContext.PhoneCalls.Where(pc => pc.Id == iPhoneCall.Id).FirstOrDefault();

                if (daLoadedPhoneCall == null) { //not found?
                    throw new Exception("PhoneCall not found to update");
                }
                else {
                    // Update
                    _dbContext.Entry(daLoadedPhoneCall).CurrentValues.SetValues(daPhoneCall);
                }

                // Save in data access (entity framework)
                result = this._dbContext.SaveChanges();
            }
            catch (System.Data.UpdateException ex) {
                if (ex.InnerException != null && ex.InnerException is System.Data.SqlClient.SqlException
                   && ((System.Data.SqlClient.SqlException)ex.InnerException).ErrorCode == 8152)
                    throw ex.InnerException;
                else
                    throw ex;
            }

            return result;
        }

        // Delete an existing PhoneCall
        public int Delete(int iPhoneCallId) {
            var result = 0;
            PhoneCall daLoadedPhoneCall = null;

            try {
                // Load object into context (entity framework) 
                daLoadedPhoneCall = _dbContext.PhoneCalls.Where(pc => pc.Id == iPhoneCallId).FirstOrDefault();

                // Modify the context
                if (daLoadedPhoneCall == null) { //not found?
                    throw new Exception("PhoneCall not found to delete");
                }
                else {
                    // Delete
                    this._dbContext.PhoneCalls.Remove(daLoadedPhoneCall);
                }

                // Save in data access (entity framework)
                result = this._dbContext.SaveChanges();
            }
            catch (System.Data.UpdateException ex) {
                if (ex.InnerException != null && ex.InnerException is System.Data.SqlClient.SqlException
                   && ((System.Data.SqlClient.SqlException)ex.InnerException).ErrorCode == 8152)
                    throw ex.InnerException;
                else
                    throw ex;
            }

            return result;
        }

        public void Reset() {
            throw new NotImplementedException();
        }
    }
}
