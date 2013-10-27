using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrismStarter.UILogic.Models
{
    /// Documentation on validating user input is at http://go.microsoft.com/fwlink/?LinkID=288817&clcid=0x409
    /// 
    public class ModelValidationResult
    {
        public ModelValidationResult() {
            ModelState = new Dictionary<string, List<string>>();
        }

        public Dictionary<string, List<string>> ModelState { get; private set; }
    }
}