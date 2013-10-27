using BigBrother.UILogic.Models;
using BigBrother.UILogic.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigBrother.UILogic.Repositories 
{
    public class PhoneCallRepository : IPhoneCallRepository
    {
        private readonly IPhoneCallServiceProxy _phoneCallServiceProxy;

        public PhoneCallRepository(IPhoneCallServiceProxy phoneCallServiceProxy) {
            _phoneCallServiceProxy = phoneCallServiceProxy;
        }

        // Get all PhoneCalls
        public async Task<CrudResult> GetPhoneCallsAsync() {
            CrudResult crudResult = await _phoneCallServiceProxy.GetPhoneCallsAsync();
            return crudResult;
        }

        // Get a PhoneCall by Id
        public async Task<CrudResult> GetPhoneCallAsync(int phoneCallId) {
            CrudResult crudResult = await _phoneCallServiceProxy.GetPhoneCallAsync(phoneCallId);
            return crudResult;
        }

        // Create a new PhoneCall
        public async Task<CrudResult> CreatePhoneCallAsync(PhoneCall phoneCall) {
            CrudResult crudResult = await _phoneCallServiceProxy.CreatePhoneCallAsync(phoneCall);
            return crudResult;
        }

        // Update an existing PhoneCall
        public async Task<CrudResult> UpdatePhoneCallAsync(PhoneCall phoneCall) {
            CrudResult crudResult = await _phoneCallServiceProxy.UpdatePhoneCallAsync(phoneCall);
            return crudResult;
        }

        // Delete an existing PhoneCall
        public async Task<CrudResult> DeletePhoneCallAsync(int phoneCallId) {
            CrudResult crudResult = await _phoneCallServiceProxy.DeletePhoneCallAsync(phoneCallId);
            return crudResult;
        }
    }
}

