using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using PrismTable.DalInterface.Strings.en_US;

namespace PrismTable.DalInterface.Models 
{
    public class Entity 
    { 
        public Entity() {
            MarkedToDelete = "false";
        }

		public int Id { get; set; }

		[Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "RequiredErrorServerSide")]
		public string FirstName { get; set; }

		[Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "RequiredErrorServerSide")]
		public string LastName { get; set; }

		public string MarkedToDelete { get; set; }

    }
}