using BigBrother.UILogic.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigBrother.UILogic.Repositories
{
    public interface IPhoneCallRepository
    {
        Task<CrudResult> GetPhoneCallsAsync();
        Task<CrudResult> GetPhoneCallAsync(int phoneCallId);
        Task<CrudResult> CreatePhoneCallAsync(PhoneCall phoneCall);
        Task<CrudResult> UpdatePhoneCallAsync(PhoneCall phoneCall);
        Task<CrudResult> DeletePhoneCallAsync(int phoneCallId);
    }
}
