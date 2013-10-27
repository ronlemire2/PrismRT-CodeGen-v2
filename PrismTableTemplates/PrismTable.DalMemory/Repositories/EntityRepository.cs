using PrismTable.DalInterface.Models;
using PrismTable.DalInterface.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrismTable.DalMemory.Repositories
{
    public class EntityRepository : IEntityRepository
    {
        private static List<Entity> _entities = PopulateEntities();
        private static int _nextId = 3;

        private static List<Entity> PopulateEntities() {
            var entities = new List<Entity>();
            entities.Add(new Entity
            {
                Id = 1
            });
            entities.Add(new Entity
            {
                Id = 2
            });
            return entities;
        }

        public IEnumerable<Entity> GetEntities() {
            lock (_entities) {
                // Return new collection so callers can iterate independently on separate threads
                return _entities.ToArray();
            }
        }

        public Entity GetEntity(int entityId) {
            lock (_entities) {
                return _entities.FirstOrDefault(e => e.Id == entityId);
            }
        }

        public Entity Create(Entity entity) {
            if (entity == null) {
                throw new ArgumentNullException("entity");
            }
            entity.Id = _nextId++;
            _entities.Add(entity);
            return entity;
        }

        public int Update(Entity entity) {
            if (entity == null) {
                throw new ArgumentNullException("entity");
            }
            int index = _entities.FindIndex(e => e.Id == entity.Id);
            if (index == -1) {
                return 0;
            }
            _entities.RemoveAt(index);
            _entities.Add(entity);
            return 1;
        }

        public int Delete(int entityId) {
            return _entities.RemoveAll(e => e.Id == entityId);
        }

        public void Reset() {
            throw new NotImplementedException();
        }
    }
}
