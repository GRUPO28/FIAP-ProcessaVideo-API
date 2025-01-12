using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIAP_ProcessaVideo_API.Common.Entities;

public abstract class Entity(string  id)
{
    public string Id { get; } = !string.IsNullOrWhiteSpace(id) ? id : Guid.NewGuid().ToString().Substring(0, 5).ToUpper();


    protected abstract void Validate();
}
