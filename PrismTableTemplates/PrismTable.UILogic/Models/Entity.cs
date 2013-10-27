using PrismTable.UILogic.Services;
using Microsoft.Practices.Prism.StoreApps;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace PrismTable.UILogic.Models
{
    public class Entity : ValidatableBindableBase
    {
		private int _Id;
		private string _FirstName;
		private string _LastName;
		private string _MarkedToDelete;

        public Entity() {
            _MarkedToDelete = "false";
        }

		public int Id {
			get { return _Id; }
			set { SetProperty(ref _Id, value); }
		}

		[Required(ErrorMessageResourceType = typeof(ErrorMessagesHelper), ErrorMessageResourceName = "RequiredError")]
		public string FirstName {
			get { return _FirstName; }
			set { SetProperty(ref _FirstName, value); }
		}

		[Required(ErrorMessageResourceType = typeof(ErrorMessagesHelper), ErrorMessageResourceName = "RequiredError")]
		public string LastName {
			get { return _LastName; }
			set { SetProperty(ref _LastName, value); }
		}

		public string MarkedToDelete {
			get { return _MarkedToDelete; }
			set { SetProperty(ref _MarkedToDelete, value); }
		}

    }
}
