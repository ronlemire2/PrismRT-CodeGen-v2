using PrismTable.UILogic.Models;
using PrismTable.UILogic.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrismTable.UILogic.Repositories 
{
    public class EntityRepository : IEntityRepository
    {
        private readonly IEntityServiceProxy _entityServiceProxy;

        public EntityRepository(IEntityServiceProxy entityServiceProxy) {
            _entityServiceProxy = entityServiceProxy;
        }

        // Get all Entities
        public async Task<CrudResult> GetEntitiesAsync() {
            CrudResult crudResult = await _entityServiceProxy.GetEntitiesAsync();
            return crudResult;
        }

        // Get a Entity by Id
        public async Task<CrudResult> GetEntityAsync(int entityId) {
            CrudResult crudResult = await _entityServiceProxy.GetEntityAsync(entityId);
            return crudResult;
        }

        // Create a new Entity
        public async Task<CrudResult> CreateEntityAsync(Entity entity) {
            CrudResult crudResult = await _entityServiceProxy.CreateEntityAsync(entity);
            return crudResult;
        }

        // Update an existing Entity
        public async Task<CrudResult> UpdateEntityAsync(Entity entity) {
            CrudResult crudResult = await _entityServiceProxy.UpdateEntityAsync(entity);
            return crudResult;
        }

        // Delete an existing Entity
        public async Task<CrudResult> DeleteEntityAsync(int entityId) {
            CrudResult crudResult = await _entityServiceProxy.DeleteEntityAsync(entityId);
            return crudResult;
        }
    }
}

