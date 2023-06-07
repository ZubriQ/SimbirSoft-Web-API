using System.ComponentModel.DataAnnotations;

namespace SimbirSoft_Web_API.Models
{
    public partial class Kind
    {
        public Kind()
        {
            Animals = new HashSet<Animal>();
        }

        [Key]
        public long Id { get; set; }
        public string Name { get; set; } = null!;

        public virtual ICollection<Animal> Animals { get; set; }
    }
}
