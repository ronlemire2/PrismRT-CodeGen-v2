using PrismTable.DalEF4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrismTable.DalEF4.Repositories
{
    public class EntityMapper
    {
        #region Map DataAccessEntities to DalInterfaceEntities

        public static IEnumerable<DalInterface.Models.Entity> MapDataAccessEntitiesToDalInterfaceEntities(IEnumerable<Entity> daEntities) {
            List<DalInterface.Models.Entity> iEntities = new List<DalInterface.Models.Entity>();

            foreach (Entity daEntity in daEntities) {
                iEntities.Add(EntityMapper.MapDataAccessEntityToDalInterfaceEntity(daEntity));
            }

            return iEntities;
        }

        public static DalInterface.Models.Entity MapDataAccessEntityToDalInterfaceEntity(Entity daEntity) {
            DalInterface.Models.Entity iEntity = new DalInterface.Models.Entity();

			iEntity.Id = daEntity.Id;
			iEntity.FirstName = daEntity.FirstName;
			iEntity.LastName = daEntity.LastName;
			iEntity.MarkedToDelete = daEntity.MarkedToDelete.ToString();

            return iEntity;
        }
        
        #endregion

        #region Map DalInterfaceEntities to DataAccessEntities

        public static Entity MapDalInterfaceEntityToDataAccessEntity(DalInterface.Models.Entity iEntity) {
            Entity daEntity = new Entity();

			daEntity.Id = iEntity.Id;
			daEntity.FirstName = iEntity.FirstName;
			daEntity.LastName = iEntity.LastName;
			daEntity.MarkedToDelete = bool.Parse(iEntity.MarkedToDelete);

            return daEntity;
        }

        #endregion
    }
}
