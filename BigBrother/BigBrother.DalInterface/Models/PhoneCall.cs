using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using BigBrother.DalInterface.Strings.en_US;

namespace BigBrother.DalInterface.Models 
{
    public class PhoneCall 
    { 
        public PhoneCall() {
            MarkedToDelete = "false";
        }

		public int Id { get; set; }

		[Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "RequiredErrorServerSide")]
		[RegularExpression(Constants.SSN_REGEX_PATTERN, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "RegexErrorServerSide")]
		public string SSN { get; set; }

		public string MarkedToDelete { get; set; }

    }
}
