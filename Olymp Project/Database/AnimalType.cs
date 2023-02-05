namespace Olymp_Project.Database
{
    public class AnimalType
    {
        public long Id { get; set; }
        public string Type { get; set; }

        public AnimalType(long id, string type)
        {
            Id = id;
            Type = type;
        }
    }
}
