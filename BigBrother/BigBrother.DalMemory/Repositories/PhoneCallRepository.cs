using BigBrother.DalInterface.Models;
using BigBrother.DalInterface.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigBrother.DalMemory.Repositories
{
    public class PhoneCallRepository : IPhoneCallRepository
    {
        private static List<PhoneCall> _phoneCalls = PopulatePhoneCalls();
        private static int _nextId = 3;

        private static List<PhoneCall> PopulatePhoneCalls() {
            var phoneCalls = new List<PhoneCall>();
            phoneCalls.Add(new PhoneCall
            {
                Id = 1
            });
            phoneCalls.Add(new PhoneCall
            {
                Id = 2
            });
            return phoneCalls;
        }

        public IEnumerable<PhoneCall> GetPhoneCalls() {
            lock (_phoneCalls) {
                // Return new collection so callers can iterate independently on separate threads
                return _phoneCalls.ToArray();
            }
        }

        public PhoneCall GetPhoneCall(int phoneCallId) {
            lock (_phoneCalls) {
                return _phoneCalls.FirstOrDefault(e => e.Id == phoneCallId);
            }
        }

        public PhoneCall Create(PhoneCall phoneCall) {
            if (phoneCall == null) {
                throw new ArgumentNullException("phoneCall");
            }
            phoneCall.Id = _nextId++;
            _phoneCalls.Add(phoneCall);
            return phoneCall;
        }

        public int Update(PhoneCall phoneCall) {
            if (phoneCall == null) {
                throw new ArgumentNullException("phoneCall");
            }
            int index = _phoneCalls.FindIndex(e => e.Id == phoneCall.Id);
            if (index == -1) {
                return 0;
            }
            _phoneCalls.RemoveAt(index);
            _phoneCalls.Add(phoneCall);
            return 1;
        }

        public int Delete(int phoneCallId) {
            return _phoneCalls.RemoveAll(e => e.Id == phoneCallId);
        }

        public void Reset() {
            throw new NotImplementedException();
        }
    }
}
