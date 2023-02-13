using System;
using System.Collections.Generic;

namespace Olymp_Project.Database
{
    public partial class VisitedLocation
    {
        public long Id { get; set; }
        public long LocationId { get; set; }
        public long AnimalId { get; set; }
        public DateTime VisitDateTime { get; set; }

        public virtual Animal Animal { get; set; } = null!;
        public virtual Location Location { get; set; } = null!;
    }
}
