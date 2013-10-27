using BigBrother.UILogic.Services;
using Microsoft.Practices.Prism.StoreApps;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BigBrother.UILogic.Models
{
    public class PhoneCall : ValidatableBindableBase
    {
		private int _Id;
		private string _SSN;
		private string _MarkedToDelete;

        public PhoneCall() {
            _MarkedToDelete = "false";
        }

		public int Id {
			get { return _Id; }
			set { SetProperty(ref _Id, value); }
		}

		[Required(ErrorMessageResourceType = typeof(ErrorMessagesHelper), ErrorMessageResourceName = "RequiredError")]
		[RegularExpression(Constants.SSN_REGEX_PATTERN, ErrorMessageResourceType = typeof(ErrorMessagesHelper), ErrorMessageResourceName = "RegexError")]
		public string SSN {
			get { return _SSN; }
			set { SetProperty(ref _SSN, value); }
		}

		public string MarkedToDelete {
			get { return _MarkedToDelete; }
			set { SetProperty(ref _MarkedToDelete, value); }
		}

    }
}
