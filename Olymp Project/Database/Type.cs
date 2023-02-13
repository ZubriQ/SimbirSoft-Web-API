using System;
using System.Collections.Generic;

namespace Olymp_Project.Database
{
    public partial class Type
    {
        public Type()
        {
            Animals = new HashSet<Animal>();
        }

        public long Id { get; set; }
        public string Name { get; set; } = null!;

        public virtual ICollection<Animal> Animals { get; set; }
    }
}
