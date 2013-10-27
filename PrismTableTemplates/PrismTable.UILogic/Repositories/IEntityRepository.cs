using PrismTable.UILogic.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrismTable.UILogic.Repositories
{
    public interface IEntityRepository
    {
        Task<CrudResult> GetEntitiesAsync();
        Task<CrudResult> GetEntityAsync(int entityId);
        Task<CrudResult> CreateEntityAsync(Entity entity);
        Task<CrudResult> UpdateEntityAsync(Entity entity);
        Task<CrudResult> DeleteEntityAsync(int entityId);
    }
}