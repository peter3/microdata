using events.dal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace events.dal.Services
{
    public interface IEventService
    {
        IEnumerable<Event> Get();
        Event Get(Guid id);
        Event Add(Event ev);
        Event Delete(Event ev);
    }
}
