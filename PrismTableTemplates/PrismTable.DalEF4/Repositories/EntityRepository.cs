using PrismTable.DalEF4;
using PrismTable.DalInterface.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrismTable.DalEF4.Repositories
{
    public class EntityRepository : RepositoryBase, IEntityRepository
    {
        // Get all Entities
        public IEnumerable<DalInterface.Models.Entity> GetEntities() {
            var  daEntities = default(IEnumerable<Entity>);

            daEntities = _dbContext.Entities
                .OrderBy(e => e.Id);
            IEnumerable<DalInterface.Models.Entity> iEntities = EntityMapper.MapDataAccessEntitiesToDalInterfaceEntities(daEntities);

            return iEntities;
        }

        // Get a Entity by Id
        public DalInterface.Models.Entity GetEntity(int entityId) {
            Entity daEntity = default(Entity);

            daEntity = _dbContext.Entities
               .Where(e => e.Id == entityId).FirstOrDefault();

            return EntityMapper.MapDataAccessEntityToDalInterfaceEntity(daEntity);
        }

        // Create a new Entity
        public DalInterface.Models.Entity Create(DalInterface.Models.Entity iEntity) {
            var result = 0;

            Entity daEntity = EntityMapper.MapDalInterfaceEntityToDataAccessEntity(iEntity);

            try {
                this._dbContext.Entities.Add(daEntity);
                result = this._dbContext.SaveChanges();

                // DbContext sets Id to the new Identity value.
                iEntity.Id = daEntity.Id;
            }
            catch (System.Data.UpdateException ex) {
                if (ex.InnerException != null && ex.InnerException is System.Data.SqlClient.SqlException
                   && ((System.Data.SqlClient.SqlException)ex.InnerException).ErrorCode == 8152)
                    throw ex.InnerException;
                else
                    throw ex;
            }

            return iEntity;
        }

        // Update an existing Entity
        public int Update(DalInterface.Models.Entity iEntity) {
            var result = 0;
            Entity daLoadedEntity = null;
            Entity daEntity = EntityMapper.MapDalInterfaceEntityToDataAccessEntity(iEntity);

            try {
                // Load object into context (entity framework) 
                daLoadedEntity = _dbContext.Entities.Where(pc => pc.Id == iEntity.Id).FirstOrDefault();

                if (daLoadedEntity == null) { //not found?
                    throw new Exception("Entity not found to update");
                }
                else {
                    // Update
                    _dbContext.Entry(daLoadedEntity).CurrentValues.SetValues(daEntity);
                }

                // Save in data access (entity framework)
                result = this._dbContext.SaveChanges();
            }
            catch (System.Data.UpdateException ex) {
                if (ex.InnerException != null && ex.InnerException is System.Data.SqlClient.SqlException
                   && ((System.Data.SqlClient.SqlException)ex.InnerException).ErrorCode == 8152)
                    throw ex.InnerException;
                else
                    throw ex;
            }

            return result;
        }

        // Delete an existing Entity
        public int Delete(int iEntityId) {
            var result = 0;
            Entity daLoadedEntity = null;

            try {
                // Load object into context (entity framework) 
                daLoadedEntity = _dbContext.Entities.Where(pc => pc.Id == iEntityId).FirstOrDefault();

                // Modify the context
                if (daLoadedEntity == null) { //not found?
                    throw new Exception("Entity not found to delete");
                }
                else {
                    // Delete
                    this._dbContext.Entities.Remove(daLoadedEntity);
                }

                // Save in data access (entity framework)
                result = this._dbContext.SaveChanges();
            }
            catch (System.Data.UpdateException ex) {
                if (ex.InnerException != null && ex.InnerException is System.Data.SqlClient.SqlException
                   && ((System.Data.SqlClient.SqlException)ex.InnerException).ErrorCode == 8152)
                    throw ex.InnerException;
                else
                    throw ex;
            }

            return result;
        }

        public void Reset() {
            throw new NotImplementedException();
        }
    }
}
