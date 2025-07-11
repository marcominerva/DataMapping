namespace DataMapping.DataAccessLayer.Entities;

public class City
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Person> People { get; set; } = [];
}
