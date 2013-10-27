using BigBrother.DalEF4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigBrother.DalEF4.Repositories
{
    public class PhoneCallMapper
    {
        #region Map DataAccessEntities to DalInterfaceEntities

        public static IEnumerable<DalInterface.Models.PhoneCall> MapDataAccessPhoneCallsToDalInterfacePhoneCalls(IEnumerable<PhoneCall> daPhoneCalls) {
            List<DalInterface.Models.PhoneCall> iPhoneCalls = new List<DalInterface.Models.PhoneCall>();

            foreach (PhoneCall daPhoneCall in daPhoneCalls) {
                iPhoneCalls.Add(PhoneCallMapper.MapDataAccessPhoneCallToDalInterfacePhoneCall(daPhoneCall));
            }

            return iPhoneCalls;
        }

        public static DalInterface.Models.PhoneCall MapDataAccessPhoneCallToDalInterfacePhoneCall(PhoneCall daPhoneCall) {
            DalInterface.Models.PhoneCall iPhoneCall = new DalInterface.Models.PhoneCall();

			iPhoneCall.Id = daPhoneCall.Id;
			iPhoneCall.SSN = daPhoneCall.SSN;
			iPhoneCall.MarkedToDelete = daPhoneCall.MarkedToDelete.ToString();

            return iPhoneCall;
        }
        
        #endregion

        #region Map DalInterfaceEntities to DataAccessEntities

        public static PhoneCall MapDalInterfacePhoneCallToDataAccessPhoneCall(DalInterface.Models.PhoneCall iPhoneCall) {
            PhoneCall daPhoneCall = new PhoneCall();

			daPhoneCall.Id = iPhoneCall.Id;
			daPhoneCall.SSN = iPhoneCall.SSN;
			daPhoneCall.MarkedToDelete = bool.Parse(iPhoneCall.MarkedToDelete);

            return daPhoneCall;
        }

        #endregion
    }
}
