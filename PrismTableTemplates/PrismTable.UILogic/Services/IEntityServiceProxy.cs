using PrismTable.UILogic.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrismTable.UILogic.Services
{
    public interface IEntityServiceProxy
    {
        Task<CrudResult> GetEntitiesAsync();
        Task<CrudResult> GetEntityAsync(int entityId);
        Task<CrudResult> CreateEntityAsync(Entity entityId);
        Task<CrudResult> UpdateEntityAsync(Entity phoneCall);
        Task<CrudResult> DeleteEntityAsync(int entityId);
    }
}