using PrismTable.DalEF4.Repositories;
using PrismTable.DalInterface.Models;
using PrismTable.DalInterface.Repositories;
using PrismTable.WebAPI.Strings.en_US;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace PrismTable.WebAPI.Controllers
{
    public class EntityController : ApiController
    {
        
        private IEntityRepository _entityRepository;

        public EntityController()
            : this(new EntityRepository())
        { }

        public EntityController(IEntityRepository entityRepository)
        {
            _entityRepository = entityRepository;
        }

        // GET /api/Entity
        public HttpResponseMessage Get() {
            try {
                IEnumerable<Entity> entityList = _entityRepository.GetEntities();
                return Request.CreateResponse<CrudResult>(HttpStatusCode.OK, new CrudResult(CrudStatusCode.Success, entityList.Count(), entityList));
            }
            catch (Exception ex) {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message);
            }
        }

        // GET /api/Entity/Id 
        public HttpResponseMessage Get(int Id) {
            try {
                Entity entity = _entityRepository.GetEntity(Id);
                return Request.CreateResponse<CrudResult>(HttpStatusCode.OK, new CrudResult(CrudStatusCode.Success, 1, new List<Entity> { entity }));
            }
            catch (Exception ex) {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message);
            }
        }

        // POST /api/Entity
        public HttpResponseMessage Post(Entity entity)
        {
            if (ModelState.IsValid) {
                try {
                    entity = _entityRepository.Create(entity);
                    var response = Request.CreateResponse<CrudResult>(HttpStatusCode.Created, new CrudResult(CrudStatusCode.Success, 1, new List<Entity> { entity }));
                    string uri = Url.Link("DefaultApi", new { Id = entity.Id });
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

        // PUT /api/Entity/3
        public HttpResponseMessage Put(int Id, Entity entity)
        {
            if (ModelState.IsValid) {
                try {
                    int numRowsAffected = _entityRepository.Update(entity);
                    var response = Request.CreateResponse<CrudResult>(HttpStatusCode.OK, new CrudResult(CrudStatusCode.Success, numRowsAffected, new List<Entity> { entity }));
                    string uri = Url.Link("DefaultApi", new { Id = entity.Id });
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

        // DELETE /api/Entity/3
        public HttpResponseMessage Delete(int Id)
        {
            try {
                Entity entity = _entityRepository.GetEntity(Id);
                int numRowsAffected = _entityRepository.Delete(Id);
                var response = Request.CreateResponse<CrudResult>(HttpStatusCode.OK, new CrudResult(CrudStatusCode.Success, numRowsAffected, new List<Entity> { entity }));
                string uri = Url.Link("DefaultApi", new { Id = entity.Id });
                response.Headers.Location = new Uri(uri);
                return response;
            }
            catch (Exception ex) {
                return Request.CreateErrorResponse(HttpStatusCode.NotModified, ex.Message);
            }
        }
    }
}
