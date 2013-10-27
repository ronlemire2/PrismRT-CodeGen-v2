using PrismTable.UILogic.Models;
using Microsoft.Practices.Prism.PubSubEvents;

namespace PrismTable.UILogic.Events
{
    public class EntityDeletedEvent : PubSubEvent<Entity>
    {
    }
}