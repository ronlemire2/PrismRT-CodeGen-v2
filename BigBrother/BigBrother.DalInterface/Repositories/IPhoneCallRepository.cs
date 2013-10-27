using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BigBrother.DalInterface.Models;

namespace BigBrother.DalInterface.Repositories
{
    public interface IPhoneCallRepository
    {
        IEnumerable<PhoneCall> GetPhoneCalls();
        PhoneCall GetPhoneCall(int phoneCallId);
        PhoneCall Create(PhoneCall phoneCall);
        int Update(PhoneCall phoneCall);
        int Delete(int phoneCallId);
        void Reset();
    }
}
