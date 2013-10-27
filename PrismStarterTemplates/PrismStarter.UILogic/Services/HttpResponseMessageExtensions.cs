using PrismStarter.UILogic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace PrismStarter.UILogic.Services
{
    public static class HttpResponseMessageExtensions
    {
        public static async Task EnsureSuccessWithValidationSupportAsync(this HttpResponseMessage response) {
            // If BadRequest, see if it contains a validation payload
            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest) {
                ModelValidationResult result = null;
                try {
                    result = await response.Content.ReadAsAsync<Models.ModelValidationResult>();
                }
                catch { } // Fall through logic will take care of it
                if (result != null) throw new ModelValidationException(result);

            }
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                throw new SecurityException();

            response.EnsureSuccessStatusCode(); // Will throw for any other service call errors
        }
    }
}