using BigBrother.UILogic.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BigBrother.UILogic.Services
{
    public class PhoneCallServiceProxy : IPhoneCallServiceProxy
    {
        private string _phoneCallBaseUrl = string.Format(CultureInfo.InvariantCulture, "{0}/api/PhoneCall/", Constants.ServerAddress);
 
        // Get all CommonDataTypes
        public async Task<CrudResult> GetPhoneCallsAsync() {
            using (var httpClient = new HttpClient()) {
                var response = await httpClient.GetAsync(string.Format("{0}", _phoneCallBaseUrl));
                response.EnsureSuccessStatusCode();
                CrudResult crudResult = await response.Content.ReadAsAsync<CrudResult>();
                return crudResult;
            }
        }

        // Get a CommonDataType by Id        
        public async Task<CrudResult> GetPhoneCallAsync(int phoneCallId) {
            using (var httpClient = new HttpClient()) {
                var response = await httpClient.GetAsync(string.Format("{0}{1}", _phoneCallBaseUrl, phoneCallId.ToString()));
                response.EnsureSuccessStatusCode();
                CrudResult crudResult = await response.Content.ReadAsAsync<CrudResult>();
                return crudResult;
            }
        }

        // Create a new CommonDataType
        public async Task<CrudResult> CreatePhoneCallAsync(PhoneCall phoneCall) {
            using (HttpClientHandler handler = new HttpClientHandler { CookieContainer = new CookieContainer() }) {
                using (var httpClient = new HttpClient(handler)) {
                    string postUrl = string.Format("{0}", _phoneCallBaseUrl);
                    var response = await httpClient.PostAsJsonAsync<PhoneCall>(postUrl, phoneCall);
                    await response.EnsureSuccessWithValidationSupportAsync();
                    CrudResult crudResult = await response.Content.ReadAsAsync<CrudResult>();
                    return crudResult;
                }
            }
        }

        // Update an existing CommonDataType
        public async Task<CrudResult> UpdatePhoneCallAsync(PhoneCall phoneCall) {
            using (HttpClientHandler handler = new HttpClientHandler { CookieContainer = new CookieContainer() }) {
                using (var httpClient = new HttpClient()) {
                    string putUrl = string.Format("{0}{1}", _phoneCallBaseUrl, phoneCall.Id.ToString());
                    var response = await httpClient.PutAsJsonAsync<PhoneCall>(putUrl, phoneCall);
                    await response.EnsureSuccessWithValidationSupportAsync();
                    CrudResult crudResult = await response.Content.ReadAsAsync<CrudResult>();
                    return crudResult;
                }
            }
        }

        // Delete an existing CommonDataType
        public async Task<CrudResult> DeletePhoneCallAsync(int phoneCallId) {
            using (HttpClientHandler handler = new HttpClientHandler { CookieContainer = new CookieContainer() }) {
                using (var httpClient = new HttpClient()) {
                    string deleteUrl = string.Format("{0}{1}", _phoneCallBaseUrl, phoneCallId.ToString());
                    var response = await httpClient.DeleteAsync(deleteUrl);
                    await response.EnsureSuccessWithValidationSupportAsync();
                    CrudResult crudResult = await response.Content.ReadAsAsync<CrudResult>();
                    return crudResult;
                }
            }
        }
    }


    // Create a server-side error for testing
    //throw new HttpRequestException("GetCommonDataTypesAsync failed. Check log for details.");

    // Create a server-side error for testing
    //commonDataTypeId = 10001;

    // Create a server-side error for testing
    //throw new HttpRequestException("CreateCommonDataTypeAsync failed. Check log for details.");

    // Create a server-side error for testing
    //throw new HttpRequestException("UpdateCommonDataTypeAsync failed. Check log for details.");

    // Create a server-side error for testing
    //throw new HttpRequestException("DeleteCommonDataTypeAsync failed. Check log for details.");
}
