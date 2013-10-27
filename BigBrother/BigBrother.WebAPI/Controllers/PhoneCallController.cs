using BigBrother.DalEF4.Repositories;
using BigBrother.DalInterface.Models;
using BigBrother.DalInterface.Repositories;
using BigBrother.WebAPI.Strings.en_US;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace BigBrother.WebAPI.Controllers
{
    public class PhoneCallController : ApiController
    {
        
        private IPhoneCallRepository _phoneCallRepository;

        public PhoneCallController()
            : this(new PhoneCallRepository())
        { }

        public PhoneCallController(IPhoneCallRepository phoneCallRepository)
        {
            _phoneCallRepository = phoneCallRepository;
        }

        // GET /api/PhoneCall
        public HttpResponseMessage Get() {
            try {
                IEnumerable<PhoneCall> phoneCallList = _phoneCallRepository.GetPhoneCalls();
                return Request.CreateResponse<CrudResult>(HttpStatusCode.OK, new CrudResult(CrudStatusCode.Success, phoneCallList.Count(), phoneCallList));
            }
            catch (Exception ex) {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message);
            }
        }

        // GET /api/PhoneCall/Id 
        public HttpResponseMessage Get(int Id) {
            try {
                PhoneCall phoneCall = _phoneCallRepository.GetPhoneCall(Id);
                return Request.CreateResponse<CrudResult>(HttpStatusCode.OK, new CrudResult(CrudStatusCode.Success, 1, new List<PhoneCall> { phoneCall }));
            }
            catch (Exception ex) {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message);
            }
        }

        // POST /api/PhoneCall
        public HttpResponseMessage Post(PhoneCall phoneCall)
        {
            if (ModelState.IsValid) {
                try {
                    phoneCall = _phoneCallRepository.Create(phoneCall);
                    var response = Request.CreateResponse<CrudResult>(HttpStatusCode.Created, new CrudResult(CrudStatusCode.Success, 1, new List<PhoneCall> { phoneCall }));
                    string uri = Url.Link("DefaultApi", new { Id = phoneCall.Id });
                    response.Headers.Location = new Uri(uri);
                    return response;
                }
                catch (Exception ex) {
                    return Request.CreateErrorResponse(HttpStatusCode.NotModified, ex.Message);
                }
            }
            else {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);                
            }
        }

        // PUT /api/PhoneCall/3
        public HttpResponseMessage Put(int Id, PhoneCall phoneCall)
        {
            if (ModelState.IsValid) {
                try {
                    int numRowsAffected = _phoneCallRepository.Update(phoneCall);
                    var response = Request.CreateResponse<CrudResult>(HttpStatusCode.OK, new CrudResult(CrudStatusCode.Success, numRowsAffected, new List<PhoneCall> { phoneCall }));
                    string uri = Url.Link("DefaultApi", new { Id = phoneCall.Id });
                    response.Headers.Location = new Uri(uri);
                    return response;
                }
                catch (Exception ex) {
                    return Request.CreateErrorResponse(HttpStatusCode.NotModified, ex.Message);
                }
            }
            else {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

        // DELETE /api/PhoneCall/3
        public HttpResponseMessage Delete(int Id)
        {
            try {
                PhoneCall phoneCall = _phoneCallRepository.GetPhoneCall(Id);
                int numRowsAffected = _phoneCallRepository.Delete(Id);
                var response = Request.CreateResponse<CrudResult>(HttpStatusCode.OK, new CrudResult(CrudStatusCode.Success, numRowsAffected, new List<PhoneCall> { phoneCall }));
                string uri = Url.Link("DefaultApi", new { Id = phoneCall.Id });
                response.Headers.Location = new Uri(uri);
                return response;
            }
            catch (Exception ex) {
                return Request.CreateErrorResponse(HttpStatusCode.NotModified, ex.Message);
            }
        }
    }
}
