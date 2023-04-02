using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NpgsqlTypes;

namespace Olymp_Project.Models
{
    public class Area
    {
        [Key]
        public long Id { get; set; }
        public string Name { get; set; } = null!;

        [Column(TypeName = "polygon")]
        public NpgsqlPolygon Points { get; set; }
    }
}
