using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GildedRose.Entities
{
    /// <summary>
    /// Represents an entity that has a guid Id.
    /// </summary>
    public interface IEntity
    {
        Guid Id { get; set; }
    }
}
