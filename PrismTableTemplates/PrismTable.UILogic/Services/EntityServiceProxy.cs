using PrismTable.UILogic.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PrismTable.UILogic.Services
{
    public class EntityServiceProxy : IEntityServiceProxy
    {
        private string _entityBaseUrl = string.Format(CultureInfo.InvariantCulture, "{0}/api/Entity/", Constants.ServerAddress);
 
        // Get all CommonDataTypes
        public async Task<CrudResult> GetEntitiesAsync() {
            using (var httpClient = new HttpClient()) {
                var response = await httpClient.GetAsync(string.Format("{0}", _entityBaseUrl));
                response.EnsureSuccessStatusCode();
                CrudResult crudResult = await response.Content.ReadAsAsync<CrudResult>();
                return crudResult;
            }
        }

        // Get a CommonDataType by Id        
        public async Task<CrudResult> GetEntityAsync(int entityId) {
            using (var httpClient = new HttpClient()) {
                var response = await httpClient.GetAsync(string.Format("{0}{1}", _entityBaseUrl, entityId.ToString()));
                response.EnsureSuccessStatusCode();
                CrudResult crudResult = await response.Content.ReadAsAsync<CrudResult>();
                return crudResult;
            }
        }

        // Create a new CommonDataType
        public async Task<CrudResult> CreateEntityAsync(Entity entity) {
            using (HttpClientHandler handler = new HttpClientHandler { CookieContainer = new CookieContainer() }) {
                using (var httpClient = new HttpClient(handler)) {
                    string postUrl = string.Format("{0}", _entityBaseUrl);
                    var response = await httpClient.PostAsJsonAsync<Entity>(postUrl, entity);
                    await response.EnsureSuccessWithValidationSupportAsync();
                    CrudResult crudResult = await response.Content.ReadAsAsync<CrudResult>();
                    return crudResult;
                }
            }
        }

        // Update an existing CommonDataType
        public async Task<CrudResult> UpdateEntityAsync(Entity entity) {
            using (HttpClientHandler handler = new HttpClientHandler { CookieContainer = new CookieContainer() }) {
                using (var httpClient = new HttpClient()) {
                    string putUrl = string.Format("{0}{1}", _entityBaseUrl, entity.Id.ToString());
                    var response = await httpClient.PutAsJsonAsync<Entity>(putUrl, entity);
                    await response.EnsureSuccessWithValidationSupportAsync();
                    CrudResult crudResult = await response.Content.ReadAsAsync<CrudResult>();
                    return crudResult;
                }
            }
        }

        // Delete an existing CommonDataType
        public async Task<CrudResult> DeleteEntityAsync(int entityId) {
            using (HttpClientHandler handler = new HttpClientHandler { CookieContainer = new CookieContainer() }) {
                using (var httpClient = new HttpClient()) {
                    string deleteUrl = string.Format("{0}{1}", _entityBaseUrl, entityId.ToString());
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
