namespace Olymp_Project.Models
{
    public partial class Account
    {
        public Account()
        {
            Animals = new HashSet<Animal>();
        }

        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;

        public virtual ICollection<Animal> Animals { get; set; }
    }
}
