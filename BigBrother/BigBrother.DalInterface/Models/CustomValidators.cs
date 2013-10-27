using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigBrother.DalInterface.Models
{
    public static class CustomValidators
    {
        //public static ValidationResult ValidateZipCodeState(object value, ValidationContext validationContext) {
        //    bool isValid = false;
        //    try {
        //        if (value == null) {
        //            throw new ArgumentNullException("value");
        //        }

        //        if (validationContext == null) {
        //            throw new ArgumentNullException("validationContext");
        //        }

        //        var address = (Address)validationContext.ObjectInstance;

        //        if (address.ZipCode.Length < 3) {
        //            return new ValidationResult(Resources.ErrorZipCodeInvalidLength);
        //        }

        //        string stateName = address.State;
        //        State state = new StateRepository().GetAll().FirstOrDefault(c => c.Name == stateName);
        //        int zipCode = Convert.ToInt32(address.ZipCode.Substring(0, 3), CultureInfo.InvariantCulture);

        //        foreach (var range in state.ValidZipCodeRanges) {
        //            // If the first 3 digits of the Zip Code falls within the given range, it is valid.
        //            int minValue = Convert.ToInt32(range.Split('-')[0], CultureInfo.InvariantCulture);
        //            int maxValue = Convert.ToInt32(range.Split('-')[1], CultureInfo.InvariantCulture);

        //            isValid = zipCode >= minValue && zipCode <= maxValue;

        //            if (isValid) break;
        //        }
        //    }
        //    catch (ArgumentNullException) {
        //        isValid = false;
        //    }

        //    if (isValid) {
        //        return ValidationResult.Success;
        //    }
        //    else {
        //        return new ValidationResult(Resources.ErrorInvalidZipCodeInState);
        //    }
        //}    
    }
}
