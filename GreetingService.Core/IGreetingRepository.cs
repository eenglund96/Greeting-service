using GreetingService.API.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreetingService.Core;

public interface IGreetingRepository
{
    public Greeting Get(Guid id);
    public IEnumerable<Greeting> Get();
    public void Create(Greeting greeting);
    public void Update(Greeting greeting);
    public void Delete(Guid id);
    public void DeleteAll();
}
